using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json.Linq;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.history.countries
{
    class CountryManager
    {
        private static readonly string COUNTRIES_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "countries" });
        private static readonly string COUNTRY_TAGS_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "country_tags" });
        private static FileInfo _currentFile;
        private static Dictionary<FileInfo, Country> _countriesByFiles = new Dictionary<FileInfo, Country>(0);
        private static Dictionary<string, Country> _contriesByTag = new Dictionary<string, Country>(0);
        private static Dictionary<string, CountryGraphics> _countryGraphicsDictionary = new Dictionary<string, CountryGraphics>(0);
        private static Dictionary<string, CountryColors> _countryColors = new Dictionary<string, CountryColors>(0);
        private static Dictionary<string, CountryColors> _cosmeticCountryColors = new Dictionary<string, CountryColors>(0);

        public static void Load(Settings settings)
        {
            _countriesByFiles = new Dictionary<FileInfo, Country>();
            _contriesByTag = new Dictionary<string, Country>();
            _countryGraphicsDictionary = new Dictionary<string, CountryGraphics>();
            _countryColors = new Dictionary<string, CountryColors>();
            _cosmeticCountryColors = new Dictionary<string, CountryColors>();

            var fileInfosPairs = FileManager.ReadFileInfos(settings, COUNTRIES_FOLDER_PATH, FileManager.TXT_FORMAT);

            if (fileInfosPairs.TryGetValue("colors.txt", out FileInfo colorsFileInfo))
            {
                Logger.Log($"Loading CountryGraphics \"colors.txt\"");
                using (var fs = new FileStream(colorsFileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, new CountryColorsList(colorsFileInfo.filePath, _countryColors));
            }
            //else throw new Exception(@"Отсутствует файл common\countries\colors.txt");

            if (fileInfosPairs.TryGetValue("cosmetic.txt", out FileInfo cosmeticFileInfo))
            {
                Logger.Log($"Loading CountryGraphics \"cosmetic.txt\"");
                using (var fs = new FileStream(cosmeticFileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, new CountryColorsList(cosmeticFileInfo.filePath, _cosmeticCountryColors));
            }
            //else throw new Exception(@"Отсутствует файл common\countries\cosmetic.txt");

            fileInfosPairs.Remove("colors.txt");
            fileInfosPairs.Remove("cosmetic.txt");

            foreach (var fileInfo in fileInfosPairs.Values)
            {
                _currentFile = fileInfo;

                var countryGraphics = new CountryGraphics(COUNTRIES_FOLDER_PATH + fileInfo.fileName);
                Logger.Log($"Loading CountryGraphics = {fileInfo.fileName}");

                Logger.TryOrCatch(
                    () => countryGraphics.ParseFile(fileInfo.filePath),
                    (ex) => Logger.LogException(
                        EnumLocKey.EXCEPTION_WHILE_COUNTRY_GRAPHICS_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{fileName}", fileInfo.fileName },
                            { "{exceptionMessage}", ex.Message }
                        },
                        ex
                    )
                );

                _countryGraphicsDictionary[fileInfo.fileName] = countryGraphics;
            }

            fileInfosPairs = FileManager.ReadFileInfos(settings, COUNTRY_TAGS_FOLDER_PATH, FileManager.TXT_FORMAT);

            foreach (var fileInfo in fileInfosPairs.Values)
            {
                _currentFile = fileInfo;
                var countryTagList = new CountryTagList(_contriesByTag, _countryGraphicsDictionary, _countryColors);
                countryTagList.ParseFile(fileInfo.filePath);
            }
        }

        public static bool TryGetCountry(string tag, out Country country) => _contriesByTag.TryGetValue(tag, out country);
        public static Country GetCountry(string tag) => _contriesByTag[tag];
        public static bool HasCountry(string tag) => _contriesByTag.ContainsKey(tag);
        public static List<string> GetCountryTags() => new List<string>(_contriesByTag.Keys);
        public static List<string> GetCountryTagsSorted()
        {
            var list = GetCountryTags();
            list.Sort();
            return list;
        }
        public static List<string> GetCountryTagsSortedStartingWith(string first)
        {
            var list = GetCountryTagsSorted();
            list.Insert(0, "");
            return list;
        }

        public static void UpdateByDateTimeStamp(DateTime dateTime)
        {
            foreach (Country country in _contriesByTag.Values)
            { //TODO Доделать
                country.ClearStates();
            }
        }

        internal static void RemoveCountryByTag(string tag)
        {
            //TODO
            throw new NotImplementedException();
        }

        internal static void AddCountryByTag(string tag, Country country)
        { //TODO
            throw new NotImplementedException();
        }
    }

    class CountryTagList
    {
        private Dictionary<string, Country> _contriesByTag;
        private Dictionary<string, CountryGraphics> _countryGraphics;
        private Dictionary<string, CountryColors> _countryColors;

        public CountryTagList(Dictionary<string, Country> contriesByTag, Dictionary<string, CountryGraphics> countryGraphics, Dictionary<string, CountryColors> countryColors)
        {
            _contriesByTag = contriesByTag;
            _countryGraphics = countryGraphics;
            _countryColors = countryColors;
        }

        public void ParseFile(string filePath)
        {
            var sb = new StringBuilder();
            foreach (string line in File.ReadAllLines(filePath))
            {
                string result = Utils.RemoveComments(sb, line);
                if (result.Length == 0) continue;

                string[] pair = result.Split('=');
                if (pair.Length != 2)
                {
                    Logger.LogWarning(
                        EnumLocKey.WARNING_COUNTRY_TAGS_PARSING_INCORRECT_LINE,
                        new Dictionary<string, string> {
                            { "{line}", line },
                            { "{filePath}", filePath }
                        }
                    );
                    continue;
                }

                string key = Regex.Replace(pair[0], @"\s+", " ").Trim();
                if (key.Length != 3) continue;

                string value = (pair[1].Replace('"', ' ').Trim()).Split('/')[1];

                var country = new Country(key);
                if (_countryGraphics.TryGetValue(value, out CountryGraphics graphics))
                {
                    country.graphicalCulture = graphics.graphicalCulture;
                    country.graphicalCulture2d = graphics.graphicalCulture2d;
                    country.color = graphics.color;
                }
                else
                {
                    string inlineFilePath = $"common\\{pair[1].Replace('"', ' ').Trim()}";
                    Logger.LogWarning(
                        EnumLocKey.WARNING_COUNTRY_TAGS_PARSING_INLINE_FILEPATH_FILE_DOESNT_EXISTS,
                        new Dictionary<string, string> {
                            { "{inlineFilePath}", inlineFilePath },
                            { "{countryTag}", country.Tag },
                            { "{filePath}", filePath }
                        }
                    );
                }

                if (_countryColors.TryGetValue(key, out CountryColors colors))
                {
                    country.color = colors.color;
                    country.colorUI = colors.colorUI;
                }

                _contriesByTag[key] = country;
            }
        }
    }

    class CountryGraphics
    {
        public string filePath;
        public string graphicalCulture;
        public string graphicalCulture2d;
        public int color;
        public int colorUI;

        public CountryGraphics(string filePath)
        {
            this.filePath = filePath;
        }

        public void ParseFile(string filePath)
        {
            var sb = new StringBuilder();
            foreach (string line in File.ReadAllLines(filePath))
            {
                string result = Utils.RemoveComments(sb, line);
                if (result.Length > 0)
                {
                    string[] pair = result.Split('=');
                    if (pair.Length != 2) continue;

                    string key = pair[0].Trim();
                    string value = pair[1].Trim();
                    string prefixText, resultText;
                    string[] list;

                    switch (key)
                    {
                        case "graphical_culture":
                            graphicalCulture = value;
                            break;
                        case "graphical_culture_2d":
                            graphicalCulture = value;
                            break;
                        case "color":
                            Utils.ReadInsideBraces(value, out prefixText, out resultText);
                            prefixText = Regex.Replace(prefixText, @"\s+", " ").Trim();
                            list = Regex.Replace(resultText.Trim(), @"\s+", " ").Split(' ');
                            color = CountryColors.ParseColor(prefixText, list.ToList());
                            break;
                        case "color_ui":
                            Utils.ReadInsideBraces(value, out prefixText, out resultText);
                            prefixText = Regex.Replace(prefixText, @"\s+", " ").Trim();
                            list = Regex.Replace(resultText.Trim(), @"\s+", " ").Split(' ');
                            color = CountryColors.ParseColor(prefixText, list.ToList());
                            break;
                    }
                }
            }
        }
    }

    class CountryColorsList : IParadoxRead
    {
        public string filePath;
        public Dictionary<string, CountryColors> countryColors;

        public CountryColorsList(string filePath, Dictionary<string, CountryColors> countryColors)
        {
            this.filePath = filePath;
            this.countryColors = countryColors;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            var colors = new CountryColors();
            countryColors[token] = colors;
            try
            {
                parser.Parse(colors);
            }
            catch (Exception ex)
            {
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_COUNTRY_COLORS_INCORRECT_COLOR_FORMAT_IN_FILE,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", filePath },
                        { "{countryName}", token }
                    }
                ), ex);
            }
        }
    }

    class CountryColors : IParadoxRead
    {
        public int color, colorUI;

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "color":
                    color = ParseColor(parser);
                    break;
                case "color_ui":
                    colorUI = ParseColor(parser);
                    break;
            }
        }

        public int ParseColor(ParadoxParser parser)
        {
            string type = parser.ReadString();
            List<string> colors;

            if (parser.NextIsBracketed())
            {
                colors = (List<string>)parser.ReadStringList();
            }
            else
            {
                colors = new List<string>
                {
                    parser.ReadString(),
                    parser.ReadString(),
                    parser.ReadString()
                };
            }
            return ParseColor(type, colors);
        }

        public static int ParseColor(string format, List<string> colors)
        {
            byte r, g, b;
            format = format.ToLower();

            if (format == "" || format == "rgb")
            {
                if (colors.Count != 3)
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_INCORRECT_COUNTRY_COLOR_PARAMS_COUNT,
                        new Dictionary<string, string> {
                            { "{currentCount}", $"{colors.Count}"},
                            { "{correctCount}", "3"}
                        }
                    ));

                r = (byte)Utils.Clamp(int.Parse(colors[0]), 0, 255);
                g = (byte)Utils.Clamp(int.Parse(colors[1]), 0, 255);
                b = (byte)Utils.Clamp(int.Parse(colors[2]), 0, 255);
            }
            else if (format == "hsv")
            {
                if (colors.Count != 3)
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_INCORRECT_COUNTRY_COLOR_PARAMS_COUNT,
                        new Dictionary<string, string> {
                            { "{currentCount}", $"{colors.Count}"},
                            { "{correctCount}", "3"}
                        }
                    ));

                Utils.HsvToRgb(
                    Utils.ParseDouble(colors[0]),
                    Utils.ParseDouble(colors[1]),
                    Utils.ParseDouble(colors[2]),
                    out r, out g, out b);
            }
            else if (format == "hex")
            {
                if (colors.Count != 1)
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_INCORRECT_COUNTRY_COLOR_PARAMS_COUNT,
                        new Dictionary<string, string> {
                            { "{currentCount}", $"{colors.Count}"},
                            { "{correctCount}", "1"}
                        }
                    ));

                string[] data = colors[0].Split('x');
                int color = Convert.ToInt32(data[data.Length - 1], 16);
                r = (byte)((color & 0xFF0000) >> 16);
                g = (byte)((color & 0xFF00) >> 8);
                b = (byte)(color & 0xFF);
            }
            else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_INCORRECT_COUNTRY_COLOR_FORMAT,
                        new Dictionary<string, string> { { "{format}", format } }
                    ));

            return Utils.ArgbToInt(255, r, g, b);
        }
    }
}
