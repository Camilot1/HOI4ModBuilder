using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers;
using HOI4ModBuilder.src.openTK;
using HOI4ModBuilder.src.utils;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public class BufferInfo
    {
        public readonly string key;
        public readonly Func<int[]> dataFunc;

        public BufferInfo(string key, Func<int[]> dataFunc)
        {
            this.key = key;
            this.dataFunc = dataFunc;
        }
    }
    public abstract class MapRendererGPUCompute : IMapRenderer
    {
        private static readonly TextureType computeTextureType = new TextureType(
            System.Drawing.Imaging.PixelFormat.Format32bppArgb,
            PixelInternalFormat.Rgba8,
            PixelFormat.Bgra,
            4
        );

        private ComputeShader _computeShader;
        private Texture2D _outputTexture;
        private int _outputWidth;
        private int _outputHeight;
        private bool _computeDisabled;
        public bool IsComputeDisabled() => _computeDisabled;

        protected bool TryRenderWithCompute()
        {
            if (MapManager.MapMainLayer == null || MapManager.ProvincesPixels == null)
                return false;

            if (!ComputeShader.IsSupported())
            {
                _computeDisabled = true;
                return false;
            }

            try
            {
                EnsureResources();

                var bufferInfoList = GetBufferInfoList();

                try
                {
                    _computeShader.ActiveProgram();
                    _computeShader.SetUniform1("mapWidth", MapManager.MapSize.x);
                    _computeShader.SetUniform1("mapHeight", MapManager.MapSize.y);

                    for (int i = 0; i < bufferInfoList.Count; i++)
                    {
                        var bufferInfo = bufferInfoList[i];
                        var buffer = MapRendererBuffersManager.GetOrCreateIntBuffer(bufferInfo.key, bufferInfo.dataFunc);
                        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, i, buffer.BufferId);
                    }
                    GL.BindImageTexture(0, _outputTexture.TextureId, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba8);

                    int groupsX = (MapManager.MapSize.x + 15) / 16;
                    int groupsY = (MapManager.MapSize.y + 15) / 16;
                    _computeShader.Dispatch(groupsX, groupsY, 1, MemoryBarrierFlags.AllBarrierBits);

                    MapManager.MapMainLayer.Texture = _outputTexture;
                    return true;
                }
                finally
                {
                    GL.BindImageTexture(0, 0, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba8);
                    for (int i = 0; i <= bufferInfoList.Count; i++)
                        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, i, 0);
                    _computeShader.DeactiveProgram();
                }
            }
            catch (Exception ex)
            {
                _computeDisabled = true;
                Logger.LogException(ex);
                return false;
            }
        }

        private void EnsureResources()
        {
            if (_computeShader == null)
                _computeShader = new ComputeShader(GetShaderPath());

            int width = MapManager.MapSize.x;
            int height = MapManager.MapSize.y;

            if (_outputTexture != null && (_outputWidth != width || _outputHeight != height))
            {
                _outputTexture.Dispose();
                _outputTexture = null;
            }

            if (_outputTexture == null)
            {
                _outputTexture = new Texture2D(computeTextureType, false, null, width, height, null);
                _outputWidth = width;
                _outputHeight = height;
            }
        }


        protected abstract string GetShaderPath();
        protected abstract List<BufferInfo> GetBufferInfoList();
        public abstract MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue);
        public abstract bool TextRenderRecalculate(string parameter, string value);
    }
}
