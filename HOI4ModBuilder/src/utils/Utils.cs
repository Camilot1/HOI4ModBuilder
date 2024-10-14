using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.utils;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder
{
    class Utils
    {
        public static Random random = new Random();
        private static readonly string[] dateTimeFormats = new string[]
        {
            "y.M.d",
            "yy.M.d",
            "yyy.M.d",
            "yyyy.M.d",
            "yyyyy.M.d",
            "y.M.d.H",
            "yy.M.d.H",
            "yyy.M.d.H",
            "yyyy.M.d.H",
            "yyyyy.M.d.H",
        };

        public static bool TryParseDateTimeStamp(string value, out DateTime dateTime)
        {
            return DateTime.TryParseExact(value, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        public static int RgbToInt(byte red, byte green, byte blue)
        {
            return (red << 16) | (green << 8) | blue;
        }
        public static int ArgbToInt(byte alpha, byte red, byte green, byte blue)
        {
            return (alpha << 24) | (red << 16) | (green << 8) | blue;
        }

        public static void IntToRgb(int argb, out byte r, out byte g, out byte b)
        {
            r = (byte)((argb >> 16) & 0xff);
            g = (byte)((argb >> 8) & 0xff);
            b = (byte)(argb & 0xff);
        }

        public static void IntToArgb(int argb, out byte a, out byte r, out byte g, out byte b)
        {
            a = (byte)(argb >> 24);
            r = (byte)((argb >> 16) & 0xff);
            g = (byte)((argb >> 8) & 0xff);
            b = (byte)(argb & 0xff);
        }

        public static string DateStampToString(DateTime date)
        {
            return $"{date.Year}.{date.Month}.{date.Day}";
        }

        public static string DateTimeStampToString(DateTime date)
        {
            return $"{date.Year}.{date.Month}.{date.Day}.{date.Hour}";
        }

        public static Stream ToStream(string text)
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        }

        public static void Setter<T>(ref T parameter, ref T value, ref bool needToSave)
        {
            if (parameter != null && parameter.Equals(value) || parameter == null && value == null) return;

            parameter = value;
            needToSave = true;
        }

        public static void Setter<T>(ref T parameter, ref T value, ref bool needToSave, ref bool hasChanged)
        {
            if (parameter != null && parameter.Equals(value) || parameter == null && value == null) return;

            parameter = value;
            needToSave = true;
            hasChanged = true;
        }

        public static string DictionaryToString(Dictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("dictionary = { ");
            if (dict == null) sb.Append("NULL ");
            else foreach (var pair in dict)
                    sb.Append("{\"").Append(pair.Key).Append("\" = \"").Append(pair.Value).Append("\", ");
            sb.Append('}');
            return sb.ToString();
        }

        public static byte[] BitmapToArray(Bitmap bitmap, ImageLockMode lockMode, TextureType textureType)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            var data = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                lockMode, textureType.imagePixelFormat
            );

            int bytesCount = height * data.Stride;
            byte[] bytes = new byte[bytesCount];
            var ptr = data.Scan0;

            Marshal.Copy(ptr, bytes, 0, bytesCount);
            bitmap.UnlockBits(data);

            return bytes;
        }

        public static void ArrayToBitmap(byte[] values, Bitmap bitmap, ImageLockMode lockMode, int width, int height, TextureType textureType)
        {
            var data = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                lockMode, textureType.imagePixelFormat
            );
            int bytesCount = height * data.Stride;
            var ptr = data.Scan0;
            Marshal.Copy(values, 0, ptr, bytesCount);
            bitmap.UnlockBits(data);
        }

        public static byte[] ArgbToBrg(int[] data)
        {
            byte[] values = new byte[data.Length * 3];

            for (int i = 0; i < data.Length; i++)
            {
                int value = data[i];
                int index = i * 3;
                values[index] = (byte)value;
                values[index + 1] = (byte)(value >> 8);
                values[index + 2] = (byte)(value >> 16);
            }

            return values;
        }
        public static byte[] ArgbToBrga(int[] data)
        {
            byte[] values = new byte[data.Length * 4];

            for (int i = 0; i < data.Length; i++)
            {
                int value = data[i];
                int index = i * 4;
                values[index] = (byte)value;
                values[index + 1] = (byte)(value >> 8);
                values[index + 2] = (byte)(value >> 16);
                values[index + 3] = (byte)(value >> 24);
            }

            return values;
        }

        public static float? ClampIfNeeded(float? value, float? min, float? max)
        {
            if (ClampIfNeeded(value, (float)min, (float)max, out float? newValue)) return newValue;
            else return null;
        }

        public static bool ClampIfNeeded(float? value, float min, float max, out float? newValue)
        {
            if (value == null)
            {
                newValue = value;
                return false;
            }
            if (value < min)
            {
                newValue = min;
                return true;
            }
            else if (value > max)
            {
                newValue = max;
                return true;
            }
            else
            {
                newValue = value;
                return false;
            }
        }

        public static bool ClampIfNeeded(float value, float min, float max, out float newValue)
        {
            if (value < min)
            {
                newValue = min;
                return true;
            }
            else if (value > max)
            {
                newValue = max;
                return true;
            }
            else
            {
                newValue = value;
                return false;
            }
        }

        public static void GetFileNameAndFormat(in string path, out string fileName, out string fileFormat)
        {
            string[] pathParts = path.Split('\\');
            string[] fileNameParts = pathParts[pathParts.Length - 1].Split('.');

            fileName = null;
            fileFormat = null;
            if (fileNameParts.Length == 0) return;
            else if (fileNameParts.Length == 1) fileName = fileNameParts[0];
            else if (fileNameParts.Length == 2)
            {
                fileName = fileNameParts[0];
                fileFormat = fileNameParts[1];
            }
            else
            {
                var sb = new StringBuilder();
                for (int i = 0; i < fileNameParts.Length - 1; i++)
                {
                    sb.Append(fileNameParts[i]).Append('.');
                }
                sb.Length--;
                fileName = sb.ToString();
                fileFormat = fileNameParts[fileNameParts.Length - 1];
            }
        }

        public static List<string> GetFileNamesWithAllFormats(string[] files, string[] formats)
        {
            var outputNames = new List<string>();
            var counts = new Dictionary<string, int>();
            var formatsSet = new HashSet<string>(formats);

            foreach (string file in files)
            {
                GetFileNameAndFormat(file, out string fileName, out string fileFormat);
                if (fileName != null && fileFormat != null && formatsSet.Contains(fileFormat))
                {
                    counts.TryGetValue(fileName, out int value);
                    counts[fileName] = value + 1;
                }
            }

            foreach (string fileName in counts.Keys)
            {
                if (counts[fileName] == formats.Length) outputNames.Add(fileName);
            }
            outputNames.Sort();

            return outputNames;

        }

        public static Tuple<EnumFileScale, int> GetFileScaleFromName(string fileName)
        {
            string[] parts = fileName.Split('_');
            string part = parts[parts.Length - 1];
            string[] entry = part.Split('=');
            if (entry[0] == "h") return new Tuple<EnumFileScale, int>(EnumFileScale.HEIGHT, int.Parse(entry[1]));
            else if (entry[0] == "z") return new Tuple<EnumFileScale, int>(EnumFileScale.HEIGHT, int.Parse(entry[1]));
            else throw new InvalidDataException("Неподдерживаемый ключ размерности \"" + entry[0] + "\"");
        }

        public static FolderBrowserDialog PrepareFolderDialog(string dirPath)
        {
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return new FolderBrowserDialog
            {
                SelectedPath = dirPath
            };
        }

        public static void PrepareFileDialog(FileDialog fd, string dirPath, string filter)
        {
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            fd.InitialDirectory = dirPath;
            fd.Filter = filter;
        }
        public static void PrepareFileDialog(FileDialog fd, string title, string dirPath, string filter)
        {
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            fd.InitialDirectory = dirPath;
            fd.Filter = filter;
            fd.Title = title;
        }

        public static string RemoveComments(StringBuilder sb, string text)
        {
            foreach (char ch in text.ToCharArray())
            {
                if (ch != '#') sb.Append(ch);
                else break;
            }
            string result = sb.ToString();
            sb.Length = 0;
            return result;
        }

        public static string ReadInsideBraces(StringBuilder sb, string text)
        {
            bool read = false;
            foreach (char ch in text.ToCharArray())
            {
                if (ch == '{') read = true;
                else if (ch == '}') break;
                else if (read) sb.Append(ch);
            }
            string result = sb.ToString();
            sb.Length = 0;
            return result;
        }

        public static void ReadInsideBraces(in string text, out string prefix, out string result)
        {
            var sbPrefix = new StringBuilder();
            var sbResult = new StringBuilder();

            bool read = false;
            foreach (char ch in text.ToCharArray())
            {
                if (ch == '{') read = true;
                else if (ch == '}') break;
                else if (read) sbResult.Append(ch);
                else sbPrefix.Append(ch);
            }
            prefix = sbPrefix.ToString();
            result = sbResult.ToString();
        }

        public static bool IsProvincesListSorted(List<Province> provinces)
        {
            if (provinces.Count <= 1) return true;
            for (int i = 1; i < provinces.Count; i++)
            {
                if (provinces[i - 1].Id > provinces[i].Id) return false;
            }
            return true;
        }

        public static bool RemoveDuplicateProvinces(List<Province> provinces)
        {
            bool hasRemovedAny = false;
            var indexes = new List<int>();
            if (provinces.Count > 1)
            {
                for (int i = 1; i < provinces.Count; i++)
                {
                    if (provinces[i].Id == provinces[i - 1].Id) indexes.Add(i);
                }

                if (indexes.Count > 0)
                {
                    hasRemovedAny = true;
                    indexes.Reverse();
                }

                foreach (int index in indexes) provinces.RemoveAt(index);
            }
            return hasRemovedAny;
        }


        public static string FloatToString(float value)
        {
            return ("" + value).Replace(',', '.');
        }

        public static bool ParseIntPositionFromString(string str, out int x, out int y)
        {
            x = -1;
            y = -1;
            string[] data = str.Split(';');
            if (data.Length != 2) return false;

            if (!int.TryParse(data[0].Trim(), out x)) return false;
            if (!int.TryParse(data[1].Trim(), out y)) return false;

            return true;
        }

        public static void HsvToRgb(double h, double S, double V, out byte r, out byte g, out byte b)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = (byte)Clamp((int)(R * 255.0), 0, 255);
            g = (byte)Clamp((int)(G * 255.0), 0, 255);
            b = (byte)Clamp((int)(B * 255.0), 0, 255);
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        public static int Clamp(int i, int min, int max)
        {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
        public static float Clamp(float i, float min, float max)
        {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
        public static double Clamp(double i, double min, double max)
        {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }

        public static void CleanUpMemory()
        {
            float bytesToMBytesDivider = 1024f * 1024f;
            long memoryBeforeCleaning = GC.GetTotalMemory(false);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long memoryAfterCleaning = GC.GetTotalMemory(false);

            Logger.Log(
                $"{memoryBeforeCleaning / bytesToMBytesDivider} MB -> " +
                $"{memoryAfterCleaning / bytesToMBytesDivider} MB " +
                $"({(memoryAfterCleaning - memoryBeforeCleaning) / bytesToMBytesDivider} MB)"
            );
        }

    }
}
