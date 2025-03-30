using HOI4ModBuilder.src.parser.objects;
using HOI4ModBuilder.src.parser.parameter;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;

namespace HOI4ModBuilder.src.parser
{
    public enum EnumDemiliter
    {
        NONE,
        EQUALS = '=',
        LESS_THAN = '<',
        GREATER_THAN = '>',
    }

    public enum TokenType
    {
        NONE,
        COMMENT,
        CONSTANT,
        EQUALS,
        LESS_THAN,
        GREATER_THAN,

    }

    public class GameParserReader
    {
        private StringBuilder _sbComments = new StringBuilder();
        private StringBuilder _sbData = new StringBuilder();

        private Dictionary<string, GameConstantOld> _constants;
        private char[] _data;
        private int _dataLength;
        private int _index;
        private int _indent;

        private bool _isReadingName;

        private EnumDemiliter _demiliter = EnumDemiliter.NONE;

        private char prevCh, ch, nextCh;

        public GameParserReader(Dictionary<string, GameConstantOld> constants, char[] data)
        {
            _constants = constants;
            _data = data;
            _dataLength = data.Length;
        }

        public void OldParse(IParseObjectOld obj)
        {
            while (_index < _dataLength)
            {
                prevCh = ch;
                ch = _data[_index];
                _index++;

                if (_isReadingName)
                {
                    EnsureEqualsDemiliter();
                    //ParseName();
                }
                else if (ch == '#')
                {
                    EnsureNoDemiliter();
                    ParseComment();
                }
                else if (ch == '@')
                {
                    if (_demiliter == EnumDemiliter.NONE)
                    {

                    }
                }
                else if (ch == '{')
                {
                    EnsureEqualsDemiliter();
                    _indent++;
                    ParseInsideBlock(obj);
                }
                else if (ch == '}')
                {
                    EnsureNoDemiliter();

                    _indent--;
                    if (_indent < 0)
                        throw new Exception("There was too much '}': " + _index);

                    return;
                }
                else if (ch == '"')
                {
                    if (prevCh == '\\')
                        throw new Exception("Not expection \\\" out of name parsing: " + _index);
                    else _isReadingName = true;
                }
                else if (ch == '=')
                    _demiliter = EnumDemiliter.EQUALS;
                else if (ch == '<')
                    _demiliter = EnumDemiliter.LESS_THAN;
                else if (ch == '>')
                    _demiliter = EnumDemiliter.GREATER_THAN;
                else if (char.IsWhiteSpace(ch))
                {

                }
            }
        }


        public void Parse(IParseObjectOld obj)
        {
            while (_index < _dataLength)
            {
                ParseNext(obj);
            }
        }

        private void ParseNext(IParseObjectOld obj)
        {
            SkipWhiteSpaces();

            if (ch == '#')
                ParseComment();
            else if (ch == '@')
                ParseConstant(obj);
            else
                ParseToken();
        }

        /** Пропускает все пустые символы
         * Если встречает не пустой символ, записывает его в переменную "ch" и возвращает false
         * Если доходит до конца данных, возвращает true
         **/
        private bool SkipWhiteSpaces()
        {
            while (_index < _dataLength)
            {
                ch = _data[_index];

                if (!char.IsWhiteSpace(ch))
                    return false;

                _index++;
            }

            return true;
        }

        private void ParseToken()
        {
            while (_index < _dataLength)
            {
                ch = _data[_index];
                _index++;

                if (ch == '"')
                    throw new Exception("Unexpected char " + ch + ": " + _index);
                else if (ch == '#' || char.IsWhiteSpace(ch) || IsDemiliterChar(ch))
                    return;
                else _sbData.Append(ch);
            }
        }

        private void ParseBlock(IParseObjectOld obj)
        {

        }

        private bool IsDemiliterChar(char ch)
        {
            if (ch == '=')
                _demiliter = EnumDemiliter.EQUALS;
            else if (ch == '<')
                _demiliter = EnumDemiliter.LESS_THAN;
            else if (ch == '>')
                _demiliter = EnumDemiliter.GREATER_THAN;
            else
                return false;

            return true;
        }

        private void ParseQuotedToken()
        {
            while (_index < _dataLength)
            {
                prevCh = ch;
                ch = _data[_index];
                _index++;

                if (ch == '"')
                {
                    if (prevCh == '\\')
                    {
                        _sbData.Append(prevCh);
                        _sbData.Append(ch);
                    }
                    else return;
                }
                else if (ch == '\n')
                    throw new Exception("New line while reading name: " + _index);
                else
                    _sbData.Append(ch);
            }
        }


        private void ParseComment()
        {
            _sbComments.Append(ch);
            while (_index < _dataLength)
            {
                ch = _data[_index];
                _index++;

                _sbComments.Append(ch);
                if (ch == '\n')
                    break;
            }
        }

        private void ParseConstant(IParseObjectOld obj)
        {
            if (!(obj is GameFileOld gameFile && !gameFile.IsAllowsConstants()))
                throw new Exception("Contant definition is not allowed in current context: " + _index);

            ParseToken();
            var contantName = _sbData.ToString();
            _sbData.Length = 0;

            SkipWhiteSpaces();

            if (ch != '=')
                throw new Exception("Invalide demiliter " + _demiliter + " for contant definition: " + _index);


            _index++;

        }

        private void EnsureNoDemiliter()
        {
            if (_demiliter != EnumDemiliter.NONE)
                throw new Exception("Has demiliter where it should not be: " + _index);
        }

        private void EnsureEqualsDemiliter()
        {
            if (_demiliter != EnumDemiliter.EQUALS)
                throw new Exception("Has demiliter where it should not be: " + _index);
        }

        private void ParseInsideBlock(IParseObjectOld obj)
        {
            var paramKey = _sbData.ToString();
            var staticAdapter = obj.GetStaticAdapter();

            if (staticAdapter.TryGetValue(paramKey, out var value))
                ParseStaticAdapter(obj, (Func<object, IGameParameterOld>)value);
            else if (!ParseDynamicAdapter(obj))
                throw new Exception("Unknown token " + paramKey + ": " + _index);
        }

        private void ParseStaticAdapter(IParseObjectOld obj, Func<object, IGameParameterOld> value)
        {
            var paramObj = value(obj);
            var paramObjPayload = paramObj.GetObject();

            if (paramObjPayload is IParseObjectOld parseObj)
                Parse(parseObj);
            else
                throw new Exception("Cannot parse primitive in '={}' block: " + _index);
        }

        private bool ParseDynamicAdapter(IParseObjectOld obj)
        {
            return true;
        }

    }
}
