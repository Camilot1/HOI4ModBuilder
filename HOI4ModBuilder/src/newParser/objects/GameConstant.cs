using HOI4ModBuilder.src.newParser.interfaces;

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
    }
}
