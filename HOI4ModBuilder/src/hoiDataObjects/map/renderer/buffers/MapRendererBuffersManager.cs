using HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    class MapRendererBuffersManager
    {
        public static readonly MapRendererBuffersManager INSTANCE = new MapRendererBuffersManager();
        private const int MAX_IDLE_FRAMES = 2;

        public const string PixelsToProvinceIdsKey = "pixels_to_province_ids";
        public const string ProvinceDataByIdKey = "province_data_by_id";
        public const string TerrainIdsToColorsKey = "terrain_ids_to_colors";
        public const string ContinentIdsToColorsKey = "continent_ids_to_colors";
        public const string StateDataByIdKey = "state_data_by_id";
        public const string StateCategoryDataByIdKey = "state_category_data_by_id";
        public const string StrategicRegionDataByIdKey = "strategic_region_data_by_id";

        private static readonly Dictionary<string, IntBuffer> _intBuffers = new Dictionary<string, IntBuffer>();
        private static readonly HashSet<int> _pendingDeleteBufferIds = new HashSet<int>();
        private static readonly Dictionary<int, string> _pendingDeleteBufferLogs = new Dictionary<int, string>();
        private static readonly Dictionary<string, HashSet<string>> _bufferDependents = new Dictionary<string, HashSet<string>>();
        private static int _currentFrame;

        static MapRendererBuffersManager()
        {
            AddDependency(PixelsToProvinceIdsKey, ProvinceDataByIdKey);

            AddDependency(ProvinceDataByIdKey, TerrainIdsToColorsKey);
            AddDependency(ProvinceDataByIdKey, ContinentIdsToColorsKey);

            AddDependency(StateDataByIdKey, StateCategoryDataByIdKey);
        }

        public static IntBuffer GetOrCreateIntBuffer(string key, Func<int[]> builder)
        {
            if (_intBuffers.TryGetValue(key, out IntBuffer buffer))
            {
                buffer.Touch(_currentFrame);
                return buffer;
            }

            ReleasePendingBuffers();

            int[] data = builder();
            int bufferId = CreateShaderStorageBuffer(data);
            buffer = new IntBuffer(bufferId, data, _currentFrame);
            _intBuffers[key] = buffer;
            return buffer;
        }

        public static bool TryGetIntBuffer(string key, out IntBuffer buffer)
        {
            if (!_intBuffers.TryGetValue(key, out buffer))
                return false;

            buffer.Touch(_currentFrame);
            return true;
        }

        public static void ReplaceIntBuffer(string key, int[] data)
        {
            ReleasePendingBuffers();

            if (!_intBuffers.TryGetValue(key, out IntBuffer buffer))
            {
                _intBuffers[key] = new IntBuffer(CreateShaderStorageBuffer(data), data, _currentFrame);
                return;
            }

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer.BufferId);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, new IntPtr(data.Length * sizeof(int)), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
            buffer.Replace(data);
            buffer.Touch(_currentFrame);
        }

        public static bool UpdateIntBufferRange(string key, int startIndex, int[] data)
        {
            if (!_intBuffers.TryGetValue(key, out IntBuffer buffer))
                return false;

            if (data == null || data.Length == 0)
                return true;

            if (startIndex < 0 || startIndex + data.Length > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            Array.Copy(data, 0, buffer.Data, startIndex, data.Length);

            ReleasePendingBuffers();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer.BufferId);
            GL.BufferSubData(
                BufferTarget.ShaderStorageBuffer,
                new IntPtr(startIndex * sizeof(int)),
                new IntPtr(data.Length * sizeof(int)),
                data
            );
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
            buffer.Touch(_currentFrame);
            return true;
        }

        public static void FinishFrame()
        {
            _currentFrame++;
            CollectUnusedBuffers();
            ReleasePendingBuffers();
        }

        public static void InvalidateBuffer(string key)
            => InvalidateBufferRecursive(key, new HashSet<string>(), true);

        public static void EvictBuffer(string key)
            => EvictBufferInternal(key, "evicted");

        private static void InvalidateBufferRecursive(string key, HashSet<string> visitedKeys, bool wasDependencyChainInvalidated)
        {
            if (!visitedKeys.Add(key))
                return;

            bool currentBufferInvalidated = EvictBufferInternal(key, "invalidated");

            if (!wasDependencyChainInvalidated && !currentBufferInvalidated)
                return;

            if (!_bufferDependents.TryGetValue(key, out var dependentKeys))
                return;

            foreach (string dependentKey in dependentKeys)
                InvalidateBufferRecursive(dependentKey, visitedKeys, true);
        }

        public static void InvalidateAll()
        {
            foreach (var buffer in _intBuffers.Values)
                MarkBufferForDelete(buffer.BufferId, "invalidated all");

            _intBuffers.Clear();
            ReleasePendingBuffers();
        }

        private static int CreateShaderStorageBuffer(int[] data)
        {
            int bufferId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, bufferId);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, new IntPtr(data.Length * sizeof(int)), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
            return bufferId;
        }

        private static void ReleasePendingBuffers()
        {
            if (_pendingDeleteBufferIds.Count == 0)
                return;

            foreach (int bufferId in _pendingDeleteBufferIds)
            {
                GL.DeleteBuffer(bufferId);
                if (_pendingDeleteBufferLogs.TryGetValue(bufferId, out string log))
                    Console.WriteLine(log);
                else
                    Console.WriteLine("Released bufferID " + bufferId);
            }

            _pendingDeleteBufferIds.Clear();
            _pendingDeleteBufferLogs.Clear();
        }

        private static void CollectUnusedBuffers()
        {
            if (_intBuffers.Count == 0)
                return;

            List<string> keysToInvalidate = null;
            foreach (var entry in _intBuffers)
            {
                bool isIdleByFrame = _currentFrame - entry.Value.LastAccessFrame > MAX_IDLE_FRAMES;
                if (!isIdleByFrame)
                    continue;

                if (keysToInvalidate == null)
                    keysToInvalidate = new List<string>();

                keysToInvalidate.Add(entry.Key);
            }

            if (keysToInvalidate == null)
                return;

            foreach (string key in keysToInvalidate)
                EvictBuffer(key);
        }

        private static bool EvictBufferInternal(string key, string reason)
        {
            if (!_intBuffers.TryGetValue(key, out IntBuffer buffer))
                return false;

            MarkBufferForDelete(buffer.BufferId, reason + " key=" + key);
            _intBuffers.Remove(key);
            return true;
        }

        private static void MarkBufferForDelete(int bufferId, string reason)
        {
            _pendingDeleteBufferIds.Add(bufferId);
            _pendingDeleteBufferLogs[bufferId] = "Released bufferID " + bufferId + " (" + reason + ")";
        }

        private static void AddDependency(string dependentKey, string sourceKey)
        {
            if (!_bufferDependents.TryGetValue(sourceKey, out var dependentKeys))
            {
                dependentKeys = new HashSet<string>();
                _bufferDependents[sourceKey] = dependentKeys;
            }

            dependentKeys.Add(dependentKey);
        }
    }
}
