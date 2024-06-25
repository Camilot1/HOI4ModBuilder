using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using OpenTK.Graphics.OpenGL;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.adjacencies
{
    class AdjacenciesManager
    {
        public static bool NeedToSaveAdjacencyRules { get; set; }
        public static bool NeedToSaveAdjacencies { get; set; }
        private static string _header;
        private static Adjacency _selectedSeaCross = null;
        private static Dictionary<string, AdjacencyRule> _adjacencyRules = new Dictionary<string, AdjacencyRule>();
        private static List<Adjacency> _adjacencies = new List<Adjacency>();
        private static List<Adjacency> _borderAdjacencies = new List<Adjacency>();
        private static List<Adjacency> _seaAdjacencies = new List<Adjacency>();

        public static void Init()
        {
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Delete, (sender, e) => HandleDelete());
            MainForm.SubscribeTabKeyEvent(EnumTabPage.MAP, Keys.Escape, (sender, e) => HandleEscape());
        }

        public static void Save(Settings settings)
        {
            if (NeedToSaveAdjacencies)
            {
                string adjacenciesPath = settings.modDirectory + @"map\adjacencies.csv";
                var sb = new StringBuilder();
                sb.Append(_header).Append(Constants.NEW_LINE);
                foreach (Adjacency adjacency in _adjacencies) adjacency.Save(sb);
                File.WriteAllText(adjacenciesPath, sb.ToString());
            }

            if (NeedToSaveAdjacencyRules)
            {
                string adjacencyRulesPath = settings.modDirectory + @"map\adjacency_rules.txt";
                StringBuilder sb = new StringBuilder();
                foreach (var rule in _adjacencyRules.Values) rule.Save(sb, "\t");
                File.WriteAllText(adjacencyRulesPath, sb.ToString());
            }
        }

        public static void Load(Settings settings)
        {
            _adjacencyRules = new Dictionary<string, AdjacencyRule>();
            _adjacencies = new List<Adjacency>();
            _borderAdjacencies = new List<Adjacency>();
            _seaAdjacencies = new List<Adjacency>();
            _selectedSeaCross = null;

            var fileInfos = FileManager.ReadMultiFileInfos(settings, @"map\");

            if (!fileInfos.TryGetValue("adjacency_rules.txt", out FileInfo rulesFileInfo)) throw new FileNotFoundException("adjacency_rules.txt");
            NeedToSaveAdjacencyRules = rulesFileInfo.needToSave;

            var adjRules = new AdjacencyRuleList(_adjacencyRules);
            using (var fs = new FileStream(rulesFileInfo.filePath, FileMode.Open))
                ParadoxParser.Parse(fs, adjRules);

            MainForm.Instance.SetAdjacencyRules(_adjacencyRules.Keys);

            if (!fileInfos.TryGetValue("adjacencies.csv", out FileInfo adjacenciesFileInfo)) throw new FileNotFoundException("adjacencies.csv");
            NeedToSaveAdjacencies = adjacenciesFileInfo.needToSave;

            string[] data = File.ReadAllLines(adjacenciesFileInfo.filePath);

            foreach (var adjacency in _adjacencies) adjacency.RemoveFromProvinces();

            _adjacencies.Capacity = data.Length;

            _header = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                try
                {
                    var trimmedString = data[i].Trim();
                    if (trimmedString.Length == 0 || trimmedString[0] == '#') continue;

                    var adjacency = new Adjacency();
                    adjacency.Load(i, data[i], _adjacencyRules);
                    _adjacencies.Add(adjacency);
                    adjacency.AddToProvinces();

                    if (!adjacency.CanBeDrawn()) continue;
                    if (adjacency.GetEnumType() != EnumAdjaciencyType.IMPASSABLE) _seaAdjacencies.Add(adjacency);
                    else _borderAdjacencies.Add(adjacency);
                }
                catch (Exception ex)
                {
                    Logger.LogException(
                        EnumLocKey.EXCEPTION_WHILE_ADJACENCIES_LOADING,
                        new Dictionary<string, string>
                        {
                            { "adjacencyId", $"{i}" },
                            { "{exceptionMessage}", ex.Message }
                        },
                        ex
                    );
                }
            }
        }

        public static void Draw(bool showSeaAdjacencies, bool showBorderAdjacencies)
        {
            if (showSeaAdjacencies)
            {
                GL.Color4(1f, 0f, 0f, 1f);
                GL.LineWidth((float)(500 * MapManager.zoomFactor));
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple((int)(150 * MapManager.zoomFactor), 0b1100_1110_0111_0011);

                GL.Begin(PrimitiveType.Lines);

                foreach (var seaAdjacency in _seaAdjacencies)
                {
                    seaAdjacency.GetLine(out Point2F start, out Point2F end);
                    GL.Vertex2(start.x, start.y);
                    GL.Vertex2(end.x, end.y);
                }

                GL.End();

                if (_selectedSeaCross != null)
                {
                    _selectedSeaCross.GetLine(out Point2F start, out Point2F end);

                    //Красная подложка линии
                    GL.Color4(1f, 0f, 0f, 1f);
                    GL.LineWidth((float)(750 * MapManager.zoomFactor));

                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(start.x, start.y);
                    GL.Vertex2(end.x, end.y);
                    GL.End();

                    //Синяя линия
                    GL.Color4(0f, 0f, 1f, 1f);
                    GL.LineWidth((float)(500 * MapManager.zoomFactor));

                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(start.x, start.y);
                    GL.Vertex2(end.x, end.y);
                    GL.End();

                    //Отображение первой провинции

                    GL.Color4(1f, 0f, 0f, 1f);
                    GL.PointSize(20f);
                    GL.Begin(PrimitiveType.Points);

                    _selectedSeaCross.GetProvinces(out Province startProvince, out Province endProvince, out Province throughProvince);

                    GL.Vertex2(startProvince.center.x, startProvince.center.y);

                    if (throughProvince != null)
                    {
                        GL.Color4(0f, 1f, 0f, 1f);
                        GL.Vertex2(throughProvince.center.x, throughProvince.center.y);
                    }

                    GL.Color4(0f, 0f, 1f, 1f);

                    GL.Vertex2(endProvince.center.x, endProvince.center.y);

                    GL.End();

                    GL.PointSize(15f);
                    GL.Begin(PrimitiveType.Points);
                    GL.Color4(1f, 0f, 0f, 1f);
                    GL.Vertex2(start.x, start.y);
                    GL.Color4(0f, 0f, 1f, 1f);
                    GL.Vertex2(end.x, end.y);
                    GL.End();

                }

                GL.Disable(EnableCap.LineStipple);

                if (_selectedSeaCross != null)
                {
                    //Отображение границ требуемых по правилу провинций
                    var requiredProvinces = _selectedSeaCross.GetRuleRequiredProvinces();
                    if (requiredProvinces != null && requiredProvinces.Count > 0)
                    {
                        GL.Color4(0f, 0.3f, 0.75f, 1f);
                        GL.LineWidth((float)(325f * MapManager.zoomFactor));

                        foreach (var p in requiredProvinces)
                        {
                            foreach (var b in p.borders)
                            {
                                GL.Begin(PrimitiveType.LineStrip);
                                foreach (var pixel in b.pixels)
                                {
                                    GL.Vertex2(pixel.x, pixel.y);
                                }
                                GL.End();
                            }
                        }

                        GL.Color4(0f, 0.5f, 1f, 1f);
                        GL.LineWidth((float)(100f * MapManager.zoomFactor));

                        foreach (var p in requiredProvinces)
                        {
                            foreach (var b in p.borders)
                            {
                                GL.Begin(PrimitiveType.LineStrip);
                                foreach (var pixel in b.pixels)
                                {
                                    GL.Vertex2(pixel.x, pixel.y);
                                }
                                GL.End();
                            }
                        }
                    }
                }
            }

        }

        public static Adjacency GetSelectedSeaCross()
        {
            return _selectedSeaCross;
        }


        public static Adjacency SelectSeaCross(Point2D point)
        {
            if (ProvinceManager.SelectedProvince == null) return null;

            Func<Adjacency, Adjacency> func = (seaCross) =>
            {
                if (seaCross.GetLine(out Point2F f, out Point2F s) && point.IsOnLine(f, s, 1.02f))
                    return seaCross;
                else return null;
            };

            _selectedSeaCross = ProvinceManager.SelectedProvince.ForEachAdjacency(func);
            if (_selectedSeaCross != null) return _selectedSeaCross;

            foreach (var borderProvince in ProvinceManager.SelectedProvince.GetBorderProvinces())
            {
                _selectedSeaCross = borderProvince.ForEachAdjacency(func);
                if (_selectedSeaCross != null) return _selectedSeaCross;
            }

            _selectedSeaCross = null;
            return _selectedSeaCross;
        }

        public static List<Adjacency> GetAdjacencies() => _adjacencies;

        public static void CreateAdjacency(Province start, Province end, EnumAdjaciencyType type)
        {
            if (start == null || end == null || start.Id == end.Id) return;
            if (start.HasSeaConnectionWith(end)) return;

            NeedToSaveAdjacencies = true;

            ushort lastId = (ushort)_adjacencies.Count;
            var adjacency = new Adjacency(lastId, true, start, end, type, new Value2I() { x = -1, y = -1 }, new Value2I() { x = -1, y = -1 });
            adjacency.AddToProvinces();

            _adjacencies.Add(adjacency);
            if (adjacency.GetEnumType() != EnumAdjaciencyType.IMPASSABLE) _seaAdjacencies.Add(adjacency);
            else _borderAdjacencies.Add(adjacency);
        }

        public static void RemoveAdjacency(Adjacency adjacency)
        {
            if (adjacency == null) return;
            if (_selectedSeaCross == adjacency) _selectedSeaCross = null;


            adjacency.RemoveFromProvinces();
            _adjacencies.Remove(adjacency);
            for (int i = 0; i < _adjacencies.Count; i++) _adjacencies[i].id = (ushort)(i + 1);

            if (adjacency.GetEnumType() != EnumAdjaciencyType.IMPASSABLE) _seaAdjacencies.Remove(adjacency);
            else _borderAdjacencies.Remove(adjacency);

            NeedToSaveAdjacencies = true;
        }

        public static void HandleCursor(MouseButtons button, Point2D pos)
        {
            if (button == MouseButtons.Left)
            {
                SelectSeaCross(pos);
            }
        }

        private static void HandleDelete()
        {
            RemoveAdjacency(_selectedSeaCross);
        }

        private static void HandleEscape()
        {
            _selectedSeaCross = null;
        }
    }
}
