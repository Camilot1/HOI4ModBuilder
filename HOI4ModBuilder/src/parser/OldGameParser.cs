
using HOI4ModBuilder.src.parser.objects;
using System;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.parser
{
    public enum Token
    {
        UNTYPED = 0b0000_0000_0000_0000, // Любой символ, тип которого не указан отдельно
        EQUALS = 0b0000_0000_0000_0001,
        LESS_THAN = 0b0000_0000_0000_0010,
        GREATER_THAN = 0b0000_0000_0000_0100,
        QUOTE = 0b0000_0000_0000_1000,
        LEFT_CURLY = 0b0000_0000_0001_0000,
        RIGHT_CURLY = 0b0000_0000_0010_0000,
        LEFT_PARANTHESIS = 0b0000_0000_0100_0000,
        RIGHT_PARANTHESIS = 0b0000_0000_1000_0000,
        COMMENT = 0b0000_0001_0000_0000,
        CONSTANT = 0b0000_0010_0000_0000,
        MINUS = 0b0000_0100_0000_0000,
        DOT = 0b0000_1000_0000_0000,
        SLASH = 0b0001_0000_0000_0000,
        SPACE = 0b0010_0000_0000_0000,
        NEW_LINE = 0b0100_0000_0000_0000,
        SHIELDED_QUOTE = 0b1000_0000_0000_0000
    }

    public class OldGameParser
    {
        private FileInfo _fileInfo;
        public FileInfo FileInfo => _fileInfo;
        private char[] _data;
        private int _dataLength;
        private int _index;

        private int _lineIndex;
        public int LineIndex => _lineIndex;

        private int _lineCharIndex;
        public int LineCharIndex => _lineCharIndex;

        private char _prevChar;
        public char PrevChar => _prevChar;

        private char _currentChar;
        public char CurrentChar => _currentChar;

        private Token _token = Token.UNTYPED;
        public Token CurrentToken => _token;

        private StringBuilder _sbComments = new StringBuilder();
        private StringBuilder _sbData = new StringBuilder();
        public string GetComments() => _sbComments.ToString();
        public void ClearComments() => _sbComments.Length = 0;
        public string GetString() => _sbData.ToString();

        public static readonly int MASK_WHITE_SPACE =
            (int)(Token.SPACE | Token.NEW_LINE);

        public static readonly int MASK_INVALID_FOR_KEY =
            ~(int)(Token.UNTYPED);

        public static readonly int MASK_ANY_VALUE_NOT_ALLOWED_FIRST_CHAR =
            ~(int)(Token.QUOTE | Token.MINUS);

        public static readonly int MASK_KEY_ALLOWED_CHARS = (int)Token.UNTYPED;
        public static readonly int MASK_VALUE_ALLOWED_CHARS = (int)(Token.UNTYPED | Token.QUOTE | Token.CONSTANT | Token.MINUS | Token.DOT | Token.SLASH | Token.SHIELDED_QUOTE);
        public static readonly int MASK_CONSTANT_VALUE_ALLOWED_START_CHAR = (int)(Token.UNTYPED | Token.QUOTE | Token.MINUS);
        public static readonly int QUOTED_VALUE_ALLOWER_CHARS = 0; //TODO //~(int)(Token.MINUS | Token.DOT | Token.SLASH | Token.SPACE);

        public string GetCursorInfo() => "[" + _lineIndex + ":" + _lineCharIndex + "] (" + _index + ")";

        /** Метод считывания данных из указанного файла */
        public void ParseFile(GameFileOld file)
        {
            _fileInfo = file.FileInfo;
            var data = File.ReadAllText(file.FilePath);
            _data = data.ToCharArray();
            _dataLength = data.Length;
            _prevChar = default;
            _currentChar = default;

            Parse(file);
        }

        /** Метод считывания данных в объект */
        public void Parse(IParseCallbacksOld obj)
        {
            while (_index < _dataLength)
            {
                SkipWhiteSpaces();

                if (_token == Token.COMMENT)
                    ParseComment();
                else if (_token == Token.UNTYPED)
                    obj.ParseCallback(this);
                else
                    throw new Exception("Character " + _currentChar + " is not allowed: " + _index);
            }
        }

        /** Метод получения следующего символа из данных */
        private void NextChar()
        {
            _prevChar = _currentChar;
            _currentChar = _data[_index];
            _index++;
            _lineCharIndex++;

            switch (_currentChar)
            {
                case '=': _token = Token.EQUALS; break;
                case '<': _token = Token.LESS_THAN; break;
                case '>': _token = Token.GREATER_THAN; break;
                case '"':
                    if (_prevChar == '\\')
                        _token = Token.SHIELDED_QUOTE;
                    else
                        _token = Token.QUOTE;
                    break;
                case '{': _token = Token.LEFT_CURLY; break;
                case '}': _token = Token.RIGHT_CURLY; break;
                case ')': _token = Token.LEFT_PARANTHESIS; break;
                case '(': _token = Token.RIGHT_PARANTHESIS; break;
                case '#': _token = Token.COMMENT; break;
                case '@': _token = Token.CONSTANT; break;
                case '-': _token = Token.MINUS; break;
                case ',':
                case '.': _token = Token.DOT; break;
                case '\\':
                case '/': _token = Token.SLASH; break;
                case '\t':
                case ' ': _token = Token.SPACE; break;
                case '\n':
                case '\f':
                case '\v':
                case '\r':
                    _token = Token.NEW_LINE;
                    _lineIndex++;
                    _lineCharIndex = 0;
                    break;
                default: _token = Token.UNTYPED; break;
            }
        }

        private void ParseKeyToken(IParseCallbacksOld obj)
        {
            _sbData.Append(_currentChar);
            while (_index < _dataLength)
            {
                NextChar();

                bool isValidForKeyToken = ((int)_token & MASK_INVALID_FOR_KEY) == 0;
                if (isValidForKeyToken)
                    _sbData.Append(_currentChar);
                else
                    return;
            }

            //ParseAdapter(obj);
        }

        /** Метод считывания комментария */
        public void ParseComment()
        {
            //_sbComments.Append(_currentChar);
            while (_index < _dataLength)
            {
                NextChar();
                _sbComments.Append(_currentChar);

                if (_token == Token.NEW_LINE)
                    return;
            }
        }

        /** Метод парсинга статических и динамических полей объекта */
        private void ParseAdapter(IParseObjectOld obj)
        {
            var paramKey = _sbData.ToString();
            _sbData.Length = 0;

            var staticAdapter = obj.GetStaticAdapter();

            if (staticAdapter != null && staticAdapter.TryGetValue(paramKey, out var staticParamProvider))
            {
                var iGameParameter = staticParamProvider(obj);


            }

            var dynamicAdapter = obj.GetDynamicAdapter();


        }

        private void ParseValue()
        {
            if (_token == Token.QUOTE)
            {
                ParseQuotedValue();
                return;
            }


        }

        public Token PeekNextToken()
        {
            SkipWhiteSpaces();
            return _token;
        }

        public string ParseValue(int allowedMask)
        {
            int disallowedMask = ~allowedMask;

            _sbData.Length = 0;
            //_sbData.Append(_currentChar);

            while (_index < _dataLength)
            {
                NextChar();

                if (((int)_token & disallowedMask) != 0)
                    return _sbData.ToString();

                _sbData.Append(_currentChar);
            }

            return _sbData.ToString();
        }

        public bool CheckCurrentToken(int allowedMask) => ((int)_token & ~allowedMask) != 0;

        private void ParseQuotedValue()
        {
            while (_index < _dataLength)
            {
                NextChar();

                if (_token == Token.QUOTE)
                    return;

                if (_token == Token.NEW_LINE)
                    throw new Exception("Invalid quoted value structure: " + _index);

                _sbData.Append(_currentChar);
            }
        }

        /** Returns TRUE if parses newLine char
         */
        public bool SkipWhiteSpaces()
        {
            bool nextLine = false;

            _sbData.Length = 0;
            while (_index < _dataLength)
            {
                NextChar();

                nextLine |= _token == Token.NEW_LINE;

                bool isWhiteSpace = ((int)_token & MASK_WHITE_SPACE) != 0;
                if (!isWhiteSpace)
                {
                    _index--;
                    return nextLine;
                }
            }

            return nextLine;
        }

    }
}
