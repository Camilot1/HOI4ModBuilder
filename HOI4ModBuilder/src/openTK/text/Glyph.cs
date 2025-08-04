using OpenTK;

namespace HOI4ModBuilder.src.openTK.text
{
    public struct Glyph
    {
        public Vector2 Size;      // width / height (px)
        public Vector2 Bearing;   // offset from baseline to left/top (px)
        public int Advance;       // Advance.X as 1/64th ‑ already bit‑shifted
        public Vector2 UVMin;     // lower‑left UV (normalized 0‑1)
        public Vector2 UVMax;     // upper‑right UV (normalized 0‑1)
    }
}
