using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text
{
    public class FontAtlas : IDisposable
    {
        public readonly int TextureID;
        public readonly int LineHeight;
        private readonly Dictionary<int, Glyph> _glyphs;
        private readonly Library _ft;
        private readonly Face _face;
        private bool _isDisposed;

        public FontAtlas(string fontPath, uint pxSize, char[] chars)
        {
            _ft = new Library();
            _face = new Face(_ft, fontPath);

            // Set character size – FreeType uses 26.6 fixed‑point. DPI = 96.
            _face.SetPixelSizes(0, pxSize);
            LineHeight = _face.Size.Metrics.Height.Value >> 6; // Convert 26.6 → px

            // Preload glyph bitmaps & determine atlas dimensions.
            var glyphBitmaps = new List<(int Code, FTBitmap Bitmap, GlyphSlot Slot)>();
            int rowW = 0, rowH = 0, atlasW = 0, atlasH = 0;

            foreach (char ch in chars)
            {
                _face.LoadChar((uint)ch, LoadFlags.Render, LoadTarget.Normal);

                var slot = _face.Glyph;
                var bmp = slot.Bitmap;
                var width = bmp.Width;
                var height = bmp.Rows;

                if (rowW + width + 1 >= 1024)
                {
                    atlasW = Math.Max(atlasW, rowW);
                    atlasH += rowH;
                    rowW = 0;
                    rowH = 0;
                }

                rowW += width + 1;
                rowH = Math.Max(rowH, height);
                glyphBitmaps.Add((ch, bmp, slot));
            }

            atlasW = Math.Max(atlasW, rowW);
            atlasH += rowH;

            // Create empty texture atlas (single channel, 8‑bit alpha)
            TextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, atlasW, atlasH, 0,
                           PixelFormat.Red, PixelType.UnsignedByte, IntPtr.Zero);

            // Standard sampling params – clamp to edge & linear filter.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Upload glyphs row by row.
            int ox = 0, oy = 0, rowMaxH = 0;
            _glyphs = new Dictionary<int, Glyph>(chars.Length + 1);
            foreach (var (code, bmp, slot) in glyphBitmaps)
            {
                if (ox + bmp.Width + 1 >= atlasW)
                {
                    oy += rowMaxH;
                    rowMaxH = 0;
                    ox = 0;
                }

                // Transfer bitmap to atlas
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, ox, oy, bmp.Width, bmp.Rows,
                                  PixelFormat.Red, PixelType.UnsignedByte, bmp.BufferData);

                // Store glyph metrics
                var glyph = new Glyph
                {
                    Size = new Vector2(bmp.Width, bmp.Rows),
                    Bearing = new Vector2(slot.BitmapLeft, slot.BitmapTop),
                    Advance = (slot.Advance.X.Value >> 6),
                    UVMin = new Vector2((float)ox / atlasW, (float)oy / atlasH),
                    UVMax = new Vector2((float)(ox + bmp.Width) / atlasW, (float)(oy + bmp.Rows) / atlasH)
                };
                _glyphs.Add(code, glyph);

                ox += bmp.Width + 1;
                rowMaxH = Math.Max(rowMaxH, bmp.Rows);
            }
            // Optional: generate mipmaps for large ‑> small downsizing (slower at init, faster at runtime)
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public bool TryGetGlyph(int code, out Glyph glyph)
            => _glyphs.TryGetValue(code, out glyph);

        public void Dispose()
        {
            if (_isDisposed)
                return;

            GL.DeleteTexture(TextureID);
            _face.Dispose();
            _ft.Dispose();
            _isDisposed = true;
        }
    }
}
