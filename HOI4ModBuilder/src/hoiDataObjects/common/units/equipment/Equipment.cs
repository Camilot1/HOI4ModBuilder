
using HOI4ModBuilder.hoiDataObjects.common.resources;
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

        private static readonly string TOKEN_RESOURCES = "resources";
        private EquipmentResources _resources;
        private EquipmentResources Resources { get => _resources; set => Utils.Setter(ref _resources, ref value, ref _needToSave); }

        //TODO implement stats, modifiers

        public Equipment(string name)
        {
            _name = name;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new System.NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_name, () =>
            {
                LinkedLayer newLayer = new LinkedLayer(prevLayer, token);
                if (token == TOKEN_YEAR)
                    Logger.CheckValueOverride(newLayer, ref _year, parser.ReadInt32());
                else if (token == TOKEN_PICTURE)
                    Logger.CheckValueOverride(newLayer, ref _picture, parser.ReadString());
                else if (token == TOKEN_IS_ARCHETYPE)
                    Logger.CheckValueOverride(newLayer, ref _isArchetype, parser.ReadBool());
                else if (token == TOKEN_IS_BUILDABLE)
                    Logger.CheckValueOverride(newLayer, ref _isBuildable, parser.ReadBool());
                else if (token == TOKEN_IS_ACTIVE)
                    Logger.CheckValueOverride(newLayer, ref _isActive, parser.ReadBool());
                else if (token == TOKEN_ARCHETYPE)
                    Logger.CheckValueOverride(newLayer, ref _archetype, parser.ReadString());
                else if (token == TOKEN_PARENT)
                    Logger.CheckValueOverride(newLayer, ref _parent, parser.ReadString());
                else if (token == TOKEN_PRIORITY)
                    Logger.CheckValueOverride(newLayer, ref _priority, parser.ReadInt32());
                else if (token == TOKEN_VISUAL_LEVEL)
                    Logger.CheckValueOverride(newLayer, ref _visualLevel, parser.ReadInt32());
                else if (token == TOKEN_RESOURCES)
                {
                    if (_resources == null) _resources = new EquipmentResources();
                    Logger.ParseLayeredValue(newLayer, _resources, parser);
                }
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            throw new System.NotImplementedException();
        }
    }

    public class EquipmentResources : IParadoxObject
    {
        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new System.NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            throw new System.NotImplementedException();
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            throw new System.NotImplementedException();
        }
    }
}
