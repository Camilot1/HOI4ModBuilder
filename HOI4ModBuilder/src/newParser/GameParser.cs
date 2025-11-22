using HOI4ModBuilder.src.dataObjects.argBlocks.info;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.utils;
using System;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.newParser
{
    public enum Token
    {
        UNTYPED = 0b1, // Любой символ, тип которого не указан отдельно
        EQUALS = 0b10,
        LESS_THAN = 0b100,
        GREATER_THAN = 0b1000,
        QUOTE = 0b1_0000,
        LEFT_CURLY = 0b10_0000,
        RIGHT_CURLY = 0b100_0000,
        LEFT_PARANTHESIS = 0b1000_0000,
        RIGHT_PARANTHESIS = 0b1_0000_0000,
        COMMENT = 0b10_0000_0000,
        CONSTANT = 0b100_0000_0000,
        MINUS = 0b1000_0000_0000,
        PLUS = 0b1_0000_0000_0000,
        DOT = 0b10_0000_0000_0000,
        SLASH = 0b100_0000_0000_0000,
        BACK_SLASH = 0b1000_0000_0000_0000,
        SPACE = 0b1_0000_0000_0000_0000,
        NEW_LINE = 0b10_0000_0000_0000_0000,
        SHIELDED_QUOTE = 0b100_0000_0000_0000_0000,
        COLON = 0b1000_0000_0000_0000_0000,
        SEMICOLON = 0b1_0000_0000_0000_0000_0000,
    }

    public class GameParser
    {
        private GameFile _file;
        private char[] _data;
        private int _dataLength;
        private int _index;

        private int _indent;
        public int Indent => _indent;

        private bool _ignorIndentChange;

        private int _lineIndex;
        public int LineIndex => _lineIndex;

        private int _lineCharIndex;
        public int LineCharIndex => _lineCharIndex;

        private char _prevChar;
        public char PrevChar => _prevChar;

        private char _currentChar;
        public char CurrentChar => _currentChar;

        private Token _token = Token.SPACE;
        public Token CurrentToken => _token;

        private StringBuilder _sbComments = new StringBuilder();
        private StringBuilder _sbData = new StringBuilder();
        public string GetComments() => _sbComments.ToString();
        public void ClearComments() => _sbComments.Length = 0;
        public string GetString() => _sbData.ToString();
        public string GetCursorInfo() => "[" + (_lineIndex + 1) + ":" + (_lineCharIndex + 1) + "] (" + (_index + 1) + ")";

        /** Метод считывания данных из указанного файла */
        public void ParseFile(GameFile file)
        {
            var data = File.ReadAllText(file.FilePath);
            ParseFileFromString(file, data);
        }

        public void ParseFileFromString(GameFile file, string data)
        {
            _file = file;

            _data = data.ToCharArray();
            _dataLength = data.Length;
            _prevChar = default;
            _currentChar = default;

            _index = -1;
            _indent = 0;
            _ignorIndentChange = false;
            _lineIndex = 0;
            _lineCharIndex = 0;
            _token = Token.SPACE;

            Parse(file);

            if (_indent != 0)
                throw new Exception("Invalid file indent: " + _indent + ": " + GetCursorInfo() + ": " + file.FilePath);

            file.Validate(null);
        }


        /** Метод считывания данных в объект */
        public void Parse(IParseObject obj)
        {
            while (_index < _dataLength)
            {
                SkipWhiteSpaces();

                if (_token == Token.COMMENT)
                    ParseComments();
                else if (_token == Token.CONSTANT)
                {
                    var constant = ParseConstant();
                    obj.InitConstantsIfNull();
                    var constants = obj.GetConstants();

                    if (constants.ContainsKey(constant.Key))
                        throw new Exception("Constant with key " + constant.Key + " already defined in this block: " + GetCursorInfo());

                    constants[constant.Key] = constant;
                }
                else if (_token == Token.UNTYPED)
                    obj.ParseCallback(this);
                else if (_token == Token.LEFT_CURLY)
                {
                    NextChar();

                    var comments = ParseAndPullComments();
                    var parent = obj.GetParent();
                    if (parent is ICommentable commentable)
                        commentable.SetComments(comments);
                }
                else if (_token == Token.RIGHT_CURLY)
                {
                    if (_index < _dataLength - 1)
                        NextChar();
                    return;
                }
                else if (_index == _dataLength - 1)
                    return;
                else
                    throw new Exception("Character " + _currentChar + " is not allowed: " + GetCursorInfo());
            }
        }

        public void SkipInsideBlock()
        {
            int targetIndent = _indent - 1;
            if (_token == Token.LEFT_CURLY)
                NextChar();
            else
                throw new Exception("SkipInsideBlock must start at '{' char: " + GetCursorInfo());

            int flags = (int)(Token.COMMENT | Token.QUOTE | Token.NEW_LINE);
            while (_index < _dataLength - 1 && targetIndent != _indent)
            {
                if (_token == Token.COMMENT)
                    SkipComments();
                else if (_token == Token.QUOTE)
                    SkipQuoted();
                else if (_token == Token.NEW_LINE)
                    SkipWhiteSpaces();
                else
                    SkipUntil(flags);
            }

            if (_token == Token.RIGHT_CURLY && _index < _dataLength - 1)
                NextChar();
        }

        public void ParseInsideBlock(Action<GameComments> commentsConsumer, Func<GameComments, string, bool> tokensConsumer)
        {
            if (_token == Token.LEFT_CURLY)
                NextChar();
            else
                throw new Exception("ParseInsideBlock must start at '{' char: " + GetCursorInfo());

            var comments = ParseAndPullComments();
            if (commentsConsumer != null)
                commentsConsumer.Invoke(comments);

            while (_index < _dataLength)
            {
                SkipWhiteSpaces();

                string data;
                if (_token == Token.QUOTE)
                {
                    ParseQuoted();
                    data = PullParsedDataString();
                }
                else if (_token == Token.COMMENT)
                {
                    ParseComments();
                    continue;
                }
                else if (_token == Token.RIGHT_CURLY)
                {
                    if (_index < _dataLength - 1)
                        NextChar();
                    return;
                }
                else
                {
                    ParseUnquotedValue();
                    data = PullParsedDataString();
                }

                if (data.Length == 0)
                    throw new Exception("Invalid parse inside block structure: " + GetCursorInfo());

                comments = ParseAndPullComments();
                if (tokensConsumer.Invoke(comments, data))
                    return;
            }
        }

        public string PullParsedDataString()
        {
            var data = _sbData.ToString();
            _sbData.Length = 0;
            return data;
        }

        public string PullParsedCommentsString()
        {
            var data = _sbComments.ToString();
            _sbComments.Length = 0;
            return data;
        }

        /** Метод получения следующего символа из данных */
        public void NextChar()
        {
            _prevChar = _currentChar;
            _index++;

            if (_index == _dataLength)
                return;

            _currentChar = _data[_index];
            _lineCharIndex++;

            ParseCharToken(_currentChar);
        }

        private void ParseCharToken(char ch)
        {
            switch (ch)
            {
                case '=': _token = Token.EQUALS; break;
                case '<': _token = Token.LESS_THAN; break;
                case '>': _token = Token.GREATER_THAN; break;
                case '"':
                    if (_token == Token.BACK_SLASH)
                        _token = Token.SHIELDED_QUOTE;
                    else
                        _token = Token.QUOTE;
                    break;
                case '{':
                    _token = Token.LEFT_CURLY;
                    if (!_ignorIndentChange)
                        _indent++;
                    break;
                case '}':
                    _token = Token.RIGHT_CURLY;
                    if (!_ignorIndentChange)
                        _indent--;

                    if (_indent < 0)
                        throw new Exception("Invalid indentation (< 0): " + GetCursorInfo());
                    break;
                case '(': _token = Token.LEFT_PARANTHESIS; break;
                case ')': _token = Token.RIGHT_PARANTHESIS; break;
                case '#': _token = Token.COMMENT; break;
                case '@': _token = Token.CONSTANT; break;
                case '-': _token = Token.MINUS; break;
                case '+': _token = Token.PLUS; break;
                case ',':
                case '.': _token = Token.DOT; break;
                case '\\': _token = Token.BACK_SLASH; break;
                case '/': _token = Token.SLASH; break;
                case '\t':
                case ' ': _token = Token.SPACE; break;
                case '\n':
                    _lineIndex++;
                    _lineCharIndex = 0;
                    _token = Token.NEW_LINE;
                    break;
                case '\f':
                case '\v':
                case '\r':
                    _token = Token.NEW_LINE;
                    break;
                case ':': _token = Token.COLON; break;
                case ';': _token = Token.SEMICOLON; break;
                default: _token = Token.UNTYPED; break;
            }
        }


        public GameConstant ParseConstant()
        {
            if (_token != Token.CONSTANT)
                throw new Exception("Invalid Constant structure: " + GetCursorInfo());

            if (!_file.IsAllowsConstants())
                throw new Exception("Constants are not supported in this file " + GetCursorInfo());

            NextChar(); // Пропускаем первый символ, содержащий @

            ParseUnquoted();
            var key = PullParsedDataString();

            if (key.Length == 0 || key.IndexOf('@') != -1)
                throw new Exception("Invalid Constant structure: " + GetCursorInfo());

            if (SkipWhiteSpaces())
                throw new Exception("Invalid Constant structure: " + GetCursorInfo());

            ParseDemiliters();
            var demiliter = PullParsedDataString();
            if (demiliter.Length != 1 || demiliter[0] != '=')
                throw new Exception("Invalid Constant structure: " + GetCursorInfo());

            if (SkipWhiteSpaces())
                throw new Exception("Invalid Constant structure: " + GetCursorInfo());

            if (_token == Token.QUOTE)
                ParseQuoted();
            else
                ParseUnquotedValue();

            var value = PullParsedDataString();
            if (value.Length == 0)
                throw new Exception("Invalid Constant structure: " + GetCursorInfo());

            var constant = new GameConstant(key, value);

            constant.SetComments(ParseAndPullComments());

            return constant;
        }

        public GameComments ParseAndPullComments()
        {
            var prevCommentsString = PullParsedCommentsString();
            var prevComments = prevCommentsString.Length > 0 ? prevCommentsString.Split('\n') : new string[0];
            var inlineComments = "";

            var nextLine = SkipWhiteSpaces();
            if (_token == Token.COMMENT && !nextLine)
            {
                ParseComment();
                inlineComments = PullParsedCommentsString();
            }

            if (prevComments.Length != 0 || inlineComments.Length != 0)
                return new GameComments(prevComments, inlineComments);

            return null;
        }


        private static readonly int FLAGS_QUOTED_UNTIL = (int)(Token.QUOTE | Token.NEW_LINE);
        public void ParseQuoted()
        {
            if (_token != Token.QUOTE)
                throw new Exception("Invalid Quoted value structure: " + GetCursorInfo());


            _ignorIndentChange = true;

            _sbData.Append(_currentChar);
            NextChar();

            ParseUntil(FLAGS_QUOTED_UNTIL);
            _ignorIndentChange = false;

            _sbData.Append(_currentChar);
            NextChar();
        }

        public void SkipQuoted()
        {
            if (_token != Token.QUOTE)
                throw new Exception("Invalid Quoted value structure: " + GetCursorInfo());

            _ignorIndentChange = true;
            NextChar();
            SkipUntil(FLAGS_QUOTED_UNTIL);
            _ignorIndentChange = false;
            NextChar();
        }

        public void ParseComment()
        {
            if (_token != Token.COMMENT)
                return;

            _ignorIndentChange = true;
            while (_index < _dataLength)
            {
                if (((int)_token & (int)Token.NEW_LINE) != 0)
                    break;

                _sbComments.Append(_currentChar);
                NextChar();
            }
            _ignorIndentChange = false;

            SkipWhiteSpaces();
        }

        public void ParseComments()
        {
            while (_index < _dataLength && _token == Token.COMMENT)
            {
                _ignorIndentChange = true;
                if (_sbComments.Length > 0 && _sbComments[_sbComments.Length - 1] != '\n')
                    _sbComments.Append('\n');

                while (_index < _dataLength)
                {
                    if (((int)_token & (int)Token.NEW_LINE) != 0)
                        break;

                    _sbComments.Append(_currentChar);
                    NextChar();
                }
                _ignorIndentChange = false;

                SkipWhiteSpaces();
            }
        }

        public void SkipComments()
        {
            while (_index < _dataLength && _token == Token.COMMENT)
            {
                _ignorIndentChange = true;
                while (_index < _dataLength)
                {
                    if (((int)_token & (int)Token.NEW_LINE) != 0)
                        break;

                    NextChar();
                }
                _ignorIndentChange = false;

                SkipWhiteSpaces();
            }
        }

        /*
        private static readonly int FLAGS_UNQUOTED_UNTIL = (int)(
            Token.EQUALS | Token.LESS_THAN | Token.GREATER_THAN | Token.QUOTE |
            Token.LEFT_CURLY | Token.RIGHT_CURLY | Token.LEFT_PARANTHESIS | Token.RIGHT_PARANTHESIS |
            Token.COMMENT | Token.MINUS | Token.PLUS | Token.DOT | Token.SLASH | Token.BACK_SLASH |
            Token.SPACE | Token.NEW_LINE | Token.SHIELDED_QUOTE | Token.SEMICOLON
            ); */
        private static readonly int FLAGS_UNQUOTED_UNTIL = (int)(
            Token.EQUALS | Token.LESS_THAN | Token.GREATER_THAN | Token.QUOTE |
            Token.LEFT_CURLY | Token.RIGHT_CURLY | Token.LEFT_PARANTHESIS | Token.RIGHT_PARANTHESIS |
            Token.COMMENT | Token.PLUS | Token.SLASH | Token.BACK_SLASH |
            Token.SPACE | Token.NEW_LINE | Token.SHIELDED_QUOTE | Token.SEMICOLON
            );
        public void ParseUnquoted() => ParseUntil(FLAGS_UNQUOTED_UNTIL);

        private static readonly int FLAGS_UNQUOTED_VALUE_UNTIL = (int)(
            Token.EQUALS | Token.LESS_THAN | Token.GREATER_THAN | Token.QUOTE |
            Token.LEFT_CURLY | Token.RIGHT_CURLY | Token.LEFT_PARANTHESIS | Token.RIGHT_PARANTHESIS |
            Token.COMMENT | Token.SLASH | Token.BACK_SLASH |
            Token.SPACE | Token.NEW_LINE | Token.SHIELDED_QUOTE | Token.SEMICOLON
            );
        public void ParseUnquotedValue() => ParseUntil(FLAGS_UNQUOTED_VALUE_UNTIL);

        public void ParseUntil(int flags)
        {
            while (_index < _dataLength)
            {
                if (((int)_token & flags) != 0)
                    return;

                _sbData.Append(_currentChar);

                NextChar();
            }
        }

        public void SkipUntil(int flags)
        {
            while (_index < _dataLength - 1)
            {
                if (((int)_token & flags) != 0)
                    return;

                NextChar();
            }
        }

        public static readonly int MASK_DEMILITERS = (int)(Token.EQUALS | Token.LESS_THAN | Token.GREATER_THAN);
        public void ParseDemiliters() => ParseWhile(MASK_DEMILITERS);

        public void ParseWhile(int flags)
        {
            flags = ~flags;
            ParseUntil(flags);
        }


        public static readonly int MASK_WHITE_SPACE = (int)(Token.NEW_LINE | Token.SPACE);
        /** Returns TRUE if parses newLine char */
        public bool SkipWhiteSpaces()
        {
            bool nextLine = false;

            _sbData.Length = 0;
            while (_index < _dataLength)
            {
                nextLine |= _token == Token.NEW_LINE;

                bool isWhiteSpace = ((int)_token & MASK_WHITE_SPACE) != 0;
                if (!isWhiteSpace)
                    return nextLine;

                if (_index == _dataLength - 1)
                    return nextLine;
                NextChar();
            }

            return nextLine;
        }


    }
}
