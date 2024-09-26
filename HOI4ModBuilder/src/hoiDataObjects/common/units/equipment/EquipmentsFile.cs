using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units.equipment
{
    public class EquipmentsFile : IParadoxObject
    {

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave;
        public bool NeedToSave { get => _needToSave || _equipmentsList.NeedToSave; }

        public FileInfo FileInfo { get; set; }
        private EquipmentsList _equipmentsList;

        public EquipmentsFile(FileInfo fileInfo, Dictionary<string, Equipment> allEquipments)
        {
            FileInfo = fileInfo;
            _equipmentsList = new EquipmentsList(allEquipments);
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                if (token == EquipmentsList.BLOCK_NAME)
                    Logger.ParseLayeredValue(prevLayer, token, parser, _equipmentsList);
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            return true;
        }
    }
}
