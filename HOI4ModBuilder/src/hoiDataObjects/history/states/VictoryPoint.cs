using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.map
{
    public class VictoryPoint : AbstractParseObject, IComparable<VictoryPoint>
    {
        public Province province;
        public uint value;

        public override IParseObject GetEmptyCopy() => new VictoryPoint();

        public override SavePattern GetSavePattern() => null;

        public VictoryPoint() { }
        public VictoryPoint(ushort provinceId)
        {
            if (!ProvinceManager.TryGetProvince(provinceId, out var province))
                throw new System.Exception("Province not found: " + provinceId);
            this.province = province;
        }

        public override void ParseCallback(GameParser parser)
        {
            parser.SkipWhiteSpaces();

            parser.ParseUnquotedValue();
            var value = parser.PullParsedDataString();
            this.value = ParserUtils.Parse<uint>(value);
        }

        public override void Save(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter)
        {
            if (province == null || value == 0)
                return;

            sb.Append(outIndent).Append(key).Append(" = { ")
                .Append(province.Id).Append(' ').Append(value)
                .Append(" }").Append(Constants.NEW_LINE);
        }

        public override string ToString() => $"VictoryPoint( province = {province?.Id}; value = {value} )";

        public override bool Equals(object obj)
        {
            return obj is VictoryPoint point &&
                   EqualityComparer<Province>.Default.Equals(province, point.province) &&
                   value == point.value;
        }

        public override int GetHashCode()
        {
            int hashCode = 1838666069;
            hashCode = hashCode * -1521134295 + EqualityComparer<Province>.Default.GetHashCode(province);
            hashCode = hashCode * -1521134295 + value.GetHashCode();
            return hashCode;
        }

        public int CompareTo(VictoryPoint other)
        {
            if (other == null)
                return 0;
            return
                province.Id - other.province.Id;
        }
    }
}
