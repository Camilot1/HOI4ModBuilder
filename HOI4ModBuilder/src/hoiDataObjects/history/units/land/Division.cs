using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class Division : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "division";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _divisionName != null && _divisionName.NeedToSave ||
                _location != null && _location.HasChangedId ||
                _forcedEquipmentVariants != null && _forcedEquipmentVariants.NeedToSave ||
                _divisionOfficer != null && _divisionOfficer.NeedToSave;
        }

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private DivisionName _divisionName;
        public DivisionName DivisionName { get => _divisionName; set => Utils.Setter(ref _divisionName, ref value, ref _needToSave); }

        private static readonly string TOKEN_LOCATION = "location";
        private Province _location;
        public Province Location { get => _location; set => Utils.Setter(ref _location, ref value, ref _needToSave); }

        private static readonly string TOKEN_DIVISION_TEMPLATE = "division_template";
        private string _divisionTemplate; //TODO Implement DivisionTemplateManager
        public string DivisionTemplate { get => _divisionTemplate; set => Utils.Setter(ref _divisionTemplate, ref value, ref _needToSave); }

        private static readonly string TOKEN_START_EXPERIENCE_FACTOR = "start_experience_factor";
        //private static readonly float DEFAULT_START_EXPERIENCE_FACTOR = 0;
        private float? _startExperienceFactor;
        public float? StartExperienceFactor { get => _startExperienceFactor; set => Utils.Setter(ref _startExperienceFactor, ref value, ref _needToSave); }

        private static readonly string TOKEN_START_EQUIPMENT_FACTOR = "start_equipment_factor";
        //private static readonly float DEFAULT_START_EQUIPMENT_FACTOR = 1f;
        private float? _startEquipmentFactor;
        public float? StartEquipmentFactor { get => _startEquipmentFactor; set => Utils.Setter(ref _startEquipmentFactor, ref value, ref _needToSave); }

        private static readonly string TOKEN_START_MANPOWER_FACTOR = "start_manpower_factor";
        private float? _startManpowerFactor;
        public float? StartManpowerFactor { get => _startManpowerFactor; set => Utils.Setter(ref _startManpowerFactor, ref value, ref _needToSave); }

        private ForcedEquipmentVariants _forcedEquipmentVariants;
        public ForcedEquipmentVariants ForcedEquipmentVariants { get => _forcedEquipmentVariants; set => Utils.Setter(ref _forcedEquipmentVariants, ref value, ref _needToSave); }

        private DivisionOfficer _divisionOfficer;
        public DivisionOfficer DivisionOfficer { get => _divisionOfficer; set => Utils.Setter(ref _divisionOfficer, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            var newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_NAME, _name);
            _divisionName?.Save(sb, newOutTab, tab);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_LOCATION, _location?.Id);
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_DIVISION_TEMPLATE, _divisionTemplate);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_START_EXPERIENCE_FACTOR, _startExperienceFactor); // DEFAULT_START_EXPERIENCE_FACTOR);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_START_EQUIPMENT_FACTOR, _startEquipmentFactor); // DEFAULT_START_EQUIPMENT_FACTOR);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_START_MANPOWER_FACTOR, _startManpowerFactor);

            _forcedEquipmentVariants?.Save(sb, newOutTab, tab);
            _divisionOfficer?.Save(sb, newOutTab, tab);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions($"{BLOCK_NAME} ({TOKEN_NAME} = \"{_name}\")", () =>
            {
                //Mandatory params
                if (token == TOKEN_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                else if (token == DivisionName.BLOCK_NAME)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _divisionName, parser, new DivisionName());
                else if (token == TOKEN_LOCATION)
                {
                    var provinceId = parser.ReadUInt16();
                    if (ProvinceManager.TryGet(provinceId, out Province newProvince))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _location, newProvince);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.PROVINCE_NOT_FOUND,
                            new Dictionary<string, string> { { "{provinceId}", "" + provinceId } }
                        );
                }
                else if (token == TOKEN_DIVISION_TEMPLATE)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _divisionTemplate, parser.ReadString());

                //Optional params
                else if (token == TOKEN_START_EXPERIENCE_FACTOR)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startExperienceFactor, parser.ReadFloat());
                else if (token == TOKEN_START_EQUIPMENT_FACTOR)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startEquipmentFactor, parser.ReadFloat());
                else if (token == TOKEN_START_MANPOWER_FACTOR)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startManpowerFactor, parser.ReadFloat());
                else if (token == ForcedEquipmentVariants.BLOCK_NAME)
                {
                    if (_forcedEquipmentVariants == null) _forcedEquipmentVariants = new ForcedEquipmentVariants();
                    Logger.ParseLayeredValue(prevLayer, token, _forcedEquipmentVariants, parser);
                }
                else if (token == DivisionOfficer.BLOCK_NAME)
                {
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _divisionOfficer, parser, new DivisionOfficer());
                }
                else if (token == "bonus") //Paradox moment :)))
                    parser.ReadInt32();
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) //TODO Remake
        {
            bool result = true;

            var currentLayer = new LinkedLayer(prevLayer, $"({TOKEN_NAME} = \"{_name}\")");

            CheckAndLogUnit.WARNINGS
                //.HasAtLeastOneMandatory(
                //    ref result, prevLayer, BLOCK_NAME,
                //    new string[] { TOKEN_NAME, DivisionName.BLOCK_NAME },
                //    _name != null || _divisionName != null
                //)
                /*
                .HasOnlyOneMutuallyExclusiveMandatory(
                    ref result, prevLayer, BLOCK_NAME,
                    new string[] { TOKEN_NAME, DivisionName.BLOCK_NAME },
                    new bool[] { _name != null, _divisionName != null }
                )
                */
                .HasMandatory(ref result, currentLayer, TOKEN_LOCATION, ref _location)
                .HasMandatory(ref result, currentLayer, TOKEN_DIVISION_TEMPLATE, ref _divisionTemplate)
                .CheckRangeAndClamp(
                    ref result, currentLayer, BLOCK_NAME,
                    (old, min, max) => Utils.ClampIfNeeded(old, min, max),
                    ref _startExperienceFactor, 0, 1
                )
                .CheckRangeAndClamp(
                    ref result, currentLayer, BLOCK_NAME,
                    (old, min, max) => Utils.ClampIfNeeded(old, min, max),
                    ref _startEquipmentFactor, 0, 1
                )
                .CheckRangeAndClamp(
                    ref result, currentLayer, BLOCK_NAME,
                    (old, min, max) => Utils.ClampIfNeeded(old, min, max),
                    ref _startManpowerFactor, 0, 1
                );

            return result;
        }
    }
}
