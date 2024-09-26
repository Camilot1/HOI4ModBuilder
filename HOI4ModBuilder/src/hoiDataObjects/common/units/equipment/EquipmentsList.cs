using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units.equipment
{
    public class EquipmentsList : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "equipments";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                foreach (var equipment in _equipments)
                {
                    if (equipment.NeedToSave) return true;
                }

                return false;
            }
        }

        private readonly Dictionary<string, Equipment> _allEquipments;
        private readonly List<Equipment> _equipments;

        public EquipmentsList(Dictionary<string, Equipment> allEquipments)
        {
            _allEquipments = allEquipments;
            _equipments = new List<Equipment>();
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!_needToSave) return false;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);
            string innerTab = outTab + tab;
            foreach (var equipment in _equipments)
            {
                equipment.Save(sb, innerTab, tab);
            }
            ParadoxUtils.EndBlock(sb, outTab);
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                Logger.ParseLayeredValue(prevLayer, token, parser, new Equipment(token));
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            throw new NotImplementedException();
        }
    }
}
