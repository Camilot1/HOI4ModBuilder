using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.managers.texture
{

    public struct MapPair
    {
        public bool needToSave;
        public Texture2D texture;
        private Bitmap _bitmap;
        public Bitmap GetBitmap() => _bitmap;


        public MapPair(bool needToSave, Bitmap bitmap, Texture2D texture)
        {
            this.needToSave = needToSave;
            this._bitmap = bitmap;
            this.texture = texture;
            TextureManager.AddTexture(texture);
        }

        public void SetColor(int x, int y, int color)
        {
            if (GetColor(x, y) == color)
                return;
            _bitmap.SetPixel(x, y, Color.FromArgb(color));
            needToSave = true;
        }
        public void SetColor(Point2D point, int color)
            => SetColor((int)point.x, (int)point.y, color);

        public void WriteByte(int x, int y, byte value)
        {
            var bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            var scan0 = bitmapData.Scan0;

            byte prevValue = Marshal.ReadByte(scan0, y * bitmapData.Stride + x);
            if (prevValue == value)
            {
                _bitmap.UnlockBits(bitmapData);
                return;
            }
            Marshal.WriteByte(scan0, y * bitmapData.Stride + x, value);
            _bitmap.UnlockBits(bitmapData);
            needToSave = true;
        }
        public void WriteByte(Point2D point, byte value) => WriteByte((int)point.x, (int)point.y, value);

        public byte GetByte(Point2D point) => GetByte((int)point.x, (int)point.y);

        public byte GetByte(int x, int y)
        {
            var bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            var scan0 = bitmapData.Scan0;

            byte value = Marshal.ReadByte(scan0, y * bitmapData.Stride + x);
            _bitmap.UnlockBits(bitmapData);
            return value;
        }

        public int GetColor(Point2D point) => _bitmap.GetPixel((int)point.x, (int)point.y).ToArgb();
        public int GetColor(int x, int y) => _bitmap.GetPixel(x, y).ToArgb();

        public bool GetIndex(int color, out byte index)
        {
            //TODO может быть оптимизировать? (сделать через словарь-палитру)
            Color[] entries = _bitmap.Palette.Entries;
            Color c = Color.FromArgb(color);
            index = 0;
            for (byte i = 0; i < entries.Length; i++)
            {
                if (entries[i] == c)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        public void RGBFill(object rawPixels, HashSet<Value2US> positions, int fillColor, TextureType textureType)
        {
            if (positions.Count == 0)
                return;

            int width = _bitmap.Width;
            int height = _bitmap.Height;

            byte[] values = Utils.BitmapToArray(_bitmap, ImageLockMode.ReadOnly, textureType);

            if (textureType == TextureManager._24bppRgb)
            {
                var pixels = (int[])rawPixels;
                foreach (var pos in positions)
                {
                    int i = (pos.x + pos.y * width);
                    int byteI = i * textureType.bytesPerPixel;
                    pixels[i] = fillColor;
                    values[byteI] = (byte)fillColor;
                    values[byteI + 1] = (byte)(fillColor >> 8);
                    values[byteI + 2] = (byte)(fillColor >> 16);
                }
            }
            else if (textureType == TextureManager._8bppGrayscale)
            {
                var pixels = (byte[])rawPixels;
                foreach (var pos in positions)
                {
                    int i = (pos.x + pos.y * width);
                    pixels[i] = (byte)fillColor;
                    values[i] = (byte)fillColor;
                }
            }
            else
                throw new Exception("Unsupported texture type: " + textureType.ToString());


            Utils.ArrayToBitmap(values, _bitmap, ImageLockMode.WriteOnly, width, height, textureType);

            texture.Update(textureType, 0, 0, width, height, values);

            needToSave = true;
        }

        public HashSet<Value2US> GetRGBPositions(ushort x, ushort y, TextureType textureType)
        {
            var poses = new HashSet<Value2US>();
            var nextPoses = new Queue<Value2US>();
            int width = _bitmap.Width;
            int height = _bitmap.Height;

            //Проверять x < 0 && y < 0 нет смысла, т.к. они они ushort и будут x > width или y > height
            if (x >= width || y >= height) return poses;

            var color = _bitmap.GetPixel(x, y);
            var pos = new Value2US(x, y);
            nextPoses.Enqueue(pos);

            byte[] values = Utils.BitmapToArray(_bitmap, ImageLockMode.ReadOnly, textureType);

            Func<int, bool> colorChecker;
            if (textureType == TextureManager._24bppRgb)
                colorChecker = (i) => values[i] == color.B && values[i + 1] == color.G && values[i + 2] == color.R;
            else if (textureType == TextureManager._8bppGrayscale)
                colorChecker = (i) => values[i] == color.B;
            else
                throw new Exception("Unsupported texture type: " + textureType.ToString());

            byte[] state = new byte[1];

            const byte stateDo = 0;
            const byte stateWait = 1;
            const byte stateContinue = 2;
            const byte stateCancel = 3;
            const int maxSteps = 500_000;

            while (nextPoses.Count > 0)
            {
                while (!(state[0] == stateDo || state[0] == stateContinue))
                {
                    Thread.Sleep(50);
                    if (state[0] == stateCancel)
                        throw new Exception(GuiLocManager.GetLoc(EnumLocKey.ACTION_WAS_CANCELED));
                }

                pos = nextPoses.Dequeue();

                //Проверять x < 0 && y < 0 нет смысла, т.к. они они ushort и будут x > width или y > height
                if (poses.Contains(pos) || pos.x >= width || pos.y >= height)
                    continue;

                int i = (pos.x + pos.y * width) * textureType.bytesPerPixel;

                if (colorChecker(i))
                {
                    poses.Add(pos);

                    if (state[0] == stateDo && poses.Count > maxSteps)
                    {
                        state[0] = stateWait; // Ожидание действия
                        Task.Run(() =>
                        {
                            var title = GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION);
                            var text = GuiLocManager.GetLoc(
                                EnumLocKey.WARNINGS_ACTION_WILL_AFFECT_MORE_THAN_X_PIXELS,
                                new Dictionary<string, string> { { "{count}", "" + maxSteps } }
                            );

                            if (MessageBoxUtils.ShowWarningChooseAction(text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                                state[0] = stateContinue; // Продолжение
                            else
                                state[0] = stateCancel; // Отмена
                        }); ;
                    }

                    nextPoses.Enqueue(new Value2US((ushort)(pos.x - 1), pos.y));
                    nextPoses.Enqueue(new Value2US(pos.x, (ushort)(pos.y + 1)));
                    nextPoses.Enqueue(new Value2US((ushort)(pos.x + 1), pos.y));
                    nextPoses.Enqueue(new Value2US(pos.x, (ushort)(pos.y - 1)));
                }
            }
            return poses;
        }

    }

}
