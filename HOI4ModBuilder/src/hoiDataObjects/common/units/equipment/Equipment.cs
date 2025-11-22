
using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.Properties;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units.equipment
{
    public class Equipment : IParadoxObject
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        private string _name;
        private string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }


        private static readonly string TOKEN_YEAR = "year";
        private int? _year;
        private int? Year { get => _year; set => Utils.Setter(ref _year, ref value, ref _needToSave); }

        private static readonly string TOKEN_PICTURE = "picture";
        private string _picture;
        private string Picture { get => _picture; set => Utils.Setter(ref _picture, ref value, ref _needToSave); }


        private static readonly string TOKEN_IS_ARCHETYPE = "is_archetype";
        private bool? _isArchetype;
        private bool? IsArchetype { get => _isArchetype; set => Utils.Setter(ref _isArchetype, ref value, ref _needToSave); }

        private static readonly string TOKEN_IS_BUILDABLE = "is_buildable";
        private bool? _isBuildable;
        private bool? IsBuildable { get => _isBuildable; set => Utils.Setter(ref _isBuildable, ref value, ref _needToSave); }

        private static readonly string TOKEN_IS_ACTIVE = "active";
        private bool? _isActive;
        private bool? IsActive { get => _isActive; set => Utils.Setter(ref _isActive, ref value, ref _needToSave); }

        private static readonly string TOKEN_ARCHETYPE = "archetype";
        private string _archetype;
        private Equipment Archetype
        {
            get => EquipmentManager.GetEquipment(_archetype);
            set
            {
                string valueString = null;
                if (value != null)
                    valueString = value.Name;

                Utils.Setter(ref _archetype, ref valueString, ref _needToSave);
            }
        }

        private static readonly string TOKEN_PARENT = "parent";
        private string _parent;
        private Equipment Parent
        {
            get => EquipmentManager.GetEquipment(_parent);
            set
            {
                string valueString = null;
                if (value != null)
                    valueString = value.Name;

                Utils.Setter(ref _parent, ref valueString, ref _needToSave);
            }
        }

        private static readonly string TOKEN_PRIORITY = "priority";
        private int? _priority;
        private int? Priority { get => _priority; set => Utils.Setter(ref _priority, ref value, ref _needToSave); }

        private static readonly string TOKEN_VISUAL_LEVEL = "visual_level";
        private int? _visualLevel;
        private int? VisualLevel { get => _visualLevel; set => Utils.Setter(ref _visualLevel, ref value, ref _needToSave); }

        //TODO implement type, group_by, interface_category

        private EquipmentResources _resources;
        private EquipmentResources Resources { get => _resources; set => Utils.Setter(ref _resources, ref value, ref _needToSave); }

        //TODO implement stats, modifiers

        public Equipment(string name)
        {
            _name = name;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.StartBlock(sb, outTab, _name);
            string innerTab = outTab + tab;
            ParadoxUtils.Save(sb, innerTab, TOKEN_YEAR, _year);
            ParadoxUtils.Save(sb, innerTab, TOKEN_PICTURE, _picture);
            ParadoxUtils.Save(sb, innerTab, TOKEN_IS_ARCHETYPE, _isArchetype);
            ParadoxUtils.Save(sb, innerTab, TOKEN_IS_BUILDABLE, _isBuildable);
            ParadoxUtils.Save(sb, innerTab, TOKEN_IS_ACTIVE, _isActive);
            ParadoxUtils.Save(sb, innerTab, TOKEN_ARCHETYPE, _archetype);
            ParadoxUtils.Save(sb, innerTab, TOKEN_PARENT, _parent);
            ParadoxUtils.Save(sb, innerTab, TOKEN_PRIORITY, _priority);
            ParadoxUtils.Save(sb, innerTab, TOKEN_VISUAL_LEVEL, _visualLevel);
            _resources?.Save(sb, outTab, tab);
            ParadoxUtils.EndBlock(sb, outTab);
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                if (token == TOKEN_YEAR)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _year, parser.ReadInt32());
                else if (token == TOKEN_PICTURE)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _picture, parser.ReadString());
                else if (token == TOKEN_IS_ARCHETYPE)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _isArchetype, parser.ReadBool());
                else if (token == TOKEN_IS_BUILDABLE)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _isBuildable, parser.ReadBool());
                else if (token == TOKEN_IS_ACTIVE)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _isActive, parser.ReadBool());
                else if (token == TOKEN_ARCHETYPE)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _archetype, parser.ReadString());
                else if (token == TOKEN_PARENT)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _parent, parser.ReadString());
                else if (token == TOKEN_PRIORITY)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _priority, parser.ReadInt32());
                else if (token == TOKEN_VISUAL_LEVEL)
                    Logger.CheckValueOverrideAndSet(prevLayer, token, ref _visualLevel, parser.ReadInt32());
                else if (token == EquipmentResources.BLOCK_NAME)
                {
                    if (_resources == null) _resources = new EquipmentResources();
                    Logger.ParseLayeredValue(new LinkedLayer(prevLayer, token), _resources, parser);
                }
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    public class EquipmentResources : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "resources";

        private readonly Dictionary<Resource, uint> _values = new Dictionary<Resource, uint>();

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            string innerTab = outTab + tab;
            foreach (var entry in _values)
            {
                ParadoxUtils.Save(sb, innerTab, entry.Key.tag, entry.Value);
            }

            ParadoxUtils.EndBlock(sb, outTab);
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                if (ResourceManager.TryGet(token, out var resource))
                {
                    if (_values.ContainsKey(resource))
                        Logger.LogLayeredWarning(
                            prevLayer,
                            EnumLocKey.WARNING_RESOURCE_COUNT_DUPLICATE_DEFINITION,
                            new Dictionary<string, string> { { "{resourceName}", resource.tag } }
                        );

                    _values[resource] = parser.ReadUInt32();
                }
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
