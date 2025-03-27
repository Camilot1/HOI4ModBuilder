using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.parser;
using HOI4ModBuilder.src.parser.objects;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using SharpFont.Fnt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        SEMICOLON = 0b1000_0000_0000_0000_0000,
    }

    public class GameParser
    {
        private GameFile _file;
        private char[] _data;
        private int _dataLength;
        private int _index;

        private int _indent;
        public int Indent => _indent;

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
        public string GetCursorInfo() => "[" + _lineIndex + ":" + _lineCharIndex + "] (" + _index + ")";

        /** Метод считывания данных из указанного файла */
        public void ParseFile(GameFile file)
        {
            _file = file;
            var data = File.ReadAllText(file.FilePath);
            _data = data.ToCharArray();
            _dataLength = data.Length;
            _prevChar = default;
            _currentChar = default;

            Parse(file);
        }


        /** Метод считывания данных в объект */
        public void Parse(IParseObject obj)
        {
            while (_index < _dataLength)
            {
                SkipWhiteSpaces();

                if (_token == Token.COMMENT)
                    ParseComment();
                else if (_token == Token.CONSTANT)
                {
                    var constant = ParseConstant();
                    var constants = obj.GetConstants();

                    if (constants.ContainsKey(constant.Key))
                        throw new Exception("Constant with key " + constant.Key + " already defined in this block: " + GetCursorInfo());

                    constants[constant.Key] = constant;
                }
                else if (_token == Token.UNTYPED)
                    obj.ParseCallback(this);
                else if (_token == Token.LEFT_CURLY)
                {
                    _indent++;
                    NextChar();
                }
                else if (_token == Token.RIGHT_CURLY)
                {
                    _indent--;
                    if (_indent < 0)
                        throw new Exception("Invalid indentation (< 0): " + GetCursorInfo());
                    NextChar();
                }
                else
                    throw new Exception("Character " + _currentChar + " is not allowed: " + GetCursorInfo());
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
                    if (_token == Token.BACK_SLASH)
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
                case '\\': _token = Token.BACK_SLASH; break;
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
                case ':':
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
                ParseUnquoted();

            var value = PullParsedDataString();
            if (value.Length == 0)
                throw new Exception("Invalid Constant structure: " + GetCursorInfo());

            var constant = new GameConstant("@" + key, value);

            constant.SetComments(ParseAndPullComments());

            return constant;
        }

        public GameComments ParseAndPullComments()
        {
            var prevComments = PullParsedCommentsString();
            var inlineComments = "";

            var nextLine = SkipWhiteSpaces();
            if (_token == Token.COMMENT && !nextLine)
            {
                ParseComment();
                inlineComments = PullParsedDataString();
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

            ParseUntil(FLAGS_QUOTED_UNTIL);
        }

        public void ParseComment()
        {
            if (_token != Token.COMMENT)
                throw new Exception("Invalid Comment structure: " + GetCursorInfo());

            while (_index < _dataLength)
            {
                NextChar();
                _sbComments.Append(_currentChar);

                if (((int)_token & (int)Token.NEW_LINE) != 0)
                {
                    _index--;
                    return;
                }

            }
        }

        private static readonly int FLAGS_UNQUOTED_UNTIL = (int)(
            Token.EQUALS | Token.LESS_THAN | Token.GREATER_THAN | Token.QUOTE |
            Token.LEFT_CURLY | Token.RIGHT_CURLY | Token.LEFT_PARANTHESIS | Token.RIGHT_PARANTHESIS |
            Token.COMMENT | Token.MINUS | Token.PLUS | Token.DOT | Token.SLASH | Token.BACK_SLASH |
            Token.SPACE | Token.NEW_LINE | Token.SHIELDED_QUOTE | Token.SEMICOLON
            );
        public void ParseUnquoted() => ParseUntil(FLAGS_UNQUOTED_UNTIL);

        public void ParseUntil(int flags)
        {
            while (_index < _dataLength)
            {
                NextChar(); //TODO change to peek (in other places too)

                if (((int)_token & flags) != 0)
                {
                    _index--;
                    return;
                }

                _sbData.Append(_currentChar);
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
