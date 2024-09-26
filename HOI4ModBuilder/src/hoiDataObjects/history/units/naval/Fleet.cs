using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class Fleet : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "fleet";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave ||
                    _navalBase != null && _navalBase.HasChangedId)
                {
                    return true;
                }

                foreach (var fleetTaskForce in _taskForces)
                    if (fleetTaskForce.NeedToSave) return true;

                return false;
            }
        }

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private static readonly string TOKEN_NAVAL_BASE = "naval_base";
        private Province _navalBase;
        public Province NavalBase { get => _navalBase; set => Utils.Setter(ref _navalBase, ref value, ref _needToSave); }

        private List<TaskForce> _taskForces = new List<TaskForce>();

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            string newOutTab = outTab + tab;
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_NAME, _name);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions($"{BLOCK_NAME} ({TOKEN_NAME} = \"{_name}\")", () =>
            {
                if (token == TOKEN_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                else if (token == TOKEN_NAVAL_BASE)
                {
                    var provinceId = parser.ReadUInt16();
                    if (!ProvinceManager.TryGetProvince(provinceId, out Province newProvince))
                    {
                        Logger.WrapException(token, new ProvinceNotFoundException(provinceId));

                        if (!newProvince.CanBeNavalBaseForShips())
                            Logger.LogLayeredWarning(
                                prevLayer, token, EnumLocKey.PROVINCE_CANT_BE_A_NAVAL_BASE_FOR_SHIPS,
                                new Dictionary<string, string> { { "{provinceId}", "" + provinceId } }
                            );
                    }
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _navalBase, newProvince);
                }
                else if (token == TaskForce.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, ref _taskForces, parser, new TaskForce());
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            var currentLayer = new LinkedLayer(prevLayer, $"({TOKEN_NAME} = \"{_name}\")");

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_NAME, ref _name)
                .HasMandatory(ref result, currentLayer, TOKEN_NAVAL_BASE, ref _navalBase)
                .Check(
                    ref result, currentLayer,
                    _taskForces != null && _taskForces.Count > 0,
                    EnumLocKey.FLEET_MUST_HAVE_AT_LEAST_ONE_TASK_FORCE
                );

            return result;
        }
    }

}
