using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.map
{
    public class VictoryPoint : AbstractParseObject
    {
        public Province province;
        public uint value;

        public override IParseObject GetEmptyCopy() => new VictoryPoint();

        public override SaveAdapter GetSaveAdapter() => null;

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

        public override void Save(StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter)
        {
            if (province == null || value == 0)
                return;

            sb.Append(outIndent).Append(key).Append(" = { ")
                .Append(province.Id).Append(' ').Append(value)
                .Append(" }").Append(Constants.NEW_LINE);
        }
    }
}
