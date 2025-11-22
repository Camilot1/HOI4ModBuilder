using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System.Text;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameConstant : ICommentable
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public GameConstant(string key, string value)
        {
            Key = key;
            Value = value;
        }

        private GameComments _comments;
        public GameComments GetComments() => _comments;

        public void SetComments(GameComments comments) => _comments = comments;


        public T GetValue<T>() => ParserUtils.Parse<T>(Value);
        public void SetParserValue(string value) => Value = value;

        public void Save(StringBuilder sb, string outIndent)
        {
            _comments?.SavePrevComments(sb, outIndent);

            sb.Append(outIndent).Append('@').Append(Key).Append(" = ").Append(Value).Append(' ');

            if (_comments != null && _comments.Inline.Length > 0)
                sb.Append(_comments.Inline).Append(' ');

            sb.Append(Constants.NEW_LINE);
        }

        public SavePattern GetSavePattern() => null;
    }
}
