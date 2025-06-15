using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.common.stateCategory
{
    public class StateCategory : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public string name;
        public int color;
        public uint localBuildingsSlots;
        public List<DataArgsBlock> modifiers = new List<DataArgsBlock>(0);

        public StateCategory() { }
        public StateCategory(string name) : this()
        {
            this.name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "color":
                    var colorList = parser.ReadIntList();
                    if (colorList.Count == 3) color = Utils.ArgbToInt(255, (byte)colorList[0], (byte)colorList[1], (byte)colorList[2]);
                    break;
                case "local_building_slots":
                    localBuildingsSlots = parser.ReadUInt32();
                    break;
                default:
                    try
                    {
                        DataArgsBlocksManager.ParseDataArgsBlocks(parser, token, modifiers);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_STATE_CATEGORY_LOADING,
                            new Dictionary<string, string>
                            {
                                { "{name}", name },
                                { "{token}", token }
                            }
                        ), ex);
                    }
                    break;
            }
        }
    }
}
