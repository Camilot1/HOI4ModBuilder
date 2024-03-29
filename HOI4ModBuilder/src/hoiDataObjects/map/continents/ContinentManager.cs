﻿using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.managers;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects
{
    class ContinentManager : IParadoxRead
    {
        public static ContinentManager Instance { get; private set; }
        private static src.FileInfo _currentFile;
        private static List<string> _continents = new List<string>();
        public static List<string> GetContinents => _continents;

        private static int[] _colors = new int[0];

        private static Action _guiReinitAction = null;

        public static void Load(Settings settings)
        {
            Instance = new ContinentManager();
            var fileInfos = FileManager.ReadMultiFileInfos(settings, @"map\");

            if (fileInfos.ContainsKey("continent.txt"))
            {
                _currentFile = fileInfos["continent.txt"];
                _continents = new List<string> { "" };

                MainForm.Instance.InvokeAction(() => MainForm.Instance.ToolStripComboBox_Map_Province_Continent.Items.Clear());
                using (var fs = new FileStream(_currentFile.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, Instance);

                if (_guiReinitAction == null)
                {
                    _guiReinitAction = () =>
                    {
                        foreach (var continentName in GetContinents)
                            MainForm.Instance.ToolStripComboBox_Map_Province_Continent.Items.Add(continentName);
                    };
                    MainForm.SubscribeGuiReinitAction(_guiReinitAction);
                }

                MainForm.Instance.InvokeAction(() => _guiReinitAction());

                _colors = new int[_continents.Count + 1];

                for (int i = 1; i < _continents.Count + 1; i++)
                {
                    Random random = new Random(i);
                    _colors[i] = Utils.ArgbToInt(
                        255,
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256)
                   );
                }
            }
        }

        public static bool AddContinent(string name)
        {
            if (_continents.Contains(name)) return false;

            _continents.Add(name);
            //TODO needToSave
            return true;
        }

        public static int GetContinentsCount()
        {
            return _continents.Count;
        }

        public static string GetContinentById(int id)
        {
            if (id < 0 || id >= _continents.Count) return "";
            return _continents[id];
        }

        public static int GetColorById(int id)
        {
            if (id < _colors.Count()) return _colors[id];
            else return _colors[0];
        }

        public static int GetContinentId(string text)
        {
            for (int i = 0; i < _continents.Count; i++)
            {
                if (_continents[i].Equals(text)) return i;
            }
            return -1;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "continents")
            {
                _continents.AddRange(parser.ReadStringList());
            }
        }
    }
}
