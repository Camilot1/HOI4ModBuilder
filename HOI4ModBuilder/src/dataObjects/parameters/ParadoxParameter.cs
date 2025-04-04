using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.dataObjects.parameters
{
    public interface IParadoxParameter
    {
        void Parse(ParadoxParser parser, Dictionary<string, ParadoxConstant> constants, LinkedLayer prevLayer, string name);
        void Save(StringBuilder sb, string outTab, string tab, string name);
    }

    public class ParadoxParameter<T> : IParadoxParameter
    {
        public string[] PrefComments { get; set; }
        public string PostComment { get; set; }

        private ParadoxConstant _constant;
        public ParadoxConstant Constant { get => _constant; set => _constant = value; }

        private T _value;
        public T Value
        {
            get => Constant != null ? Constant.GetValue<T>() : _value;
            set
            {
                _value = value;
                Constant = null;
            }
        }

        public void Parse(ParadoxParser parser, Dictionary<string, ParadoxConstant> constants, LinkedLayer prevLayer, string name)
        {
            var thisObj = this;
            Logger.WrapTokenCallbackExceptions(name, () =>
            {
                if (parser.CurrentToken == LexerToken.Quote)
                {
                    var value = parser.ReadString();

                    if (!constants.TryGetValue(value, out ParadoxConstant constant))
                    {
                        Logger.LogLayeredError(
                            prevLayer, name, EnumLocKey.LAYERED_LEVELS_PARAMETER_VALUE_CONTANT_NOT_FOUND,
                            new Dictionary<string, string> {
                                { "{name}", name },
                                { "{value}", value }
                            }
                        );
                    }

                    thisObj.Constant = constant;
                    return;
                }


            });
        }

        public void Save(StringBuilder sb, string outTab, string tab, string name)
        {
            if (PrefComments != null)
            {
                sb.Append(outTab).Append(Constants.NEW_LINE);

                foreach (var comment in PrefComments)
                {
                    sb.Append(outTab).Append(comment).Append(Constants.NEW_LINE);
                }
            }
            var value = Value;

            sb.Append(outTab)
                .Append(name)
                .Append(" = ");
            if (value is bool boolValue)
                sb.Append(boolValue ? "yes" : "no")
                    .Append(getPostComment());
            else if (value is byte || value is short || value is ushort || value is int || value is uint || value is string)
                sb.Append(value)
                    .Append(getPostComment());
            else if (value is float || value is double)
                sb.Append(("" + value).Replace(",", "."))
                    .Append(getPostComment());
            else if (value is IList<T> listValue)
            {
                sb.Append('{')
                    .Append(getPostComment());

                foreach (var o in listValue)
                {

                }
            }
            else if (Value is IParadoxObject paradoxObjectValue)
            {
                sb.Append(outTab)
                    .Append(name)
                    .Append(" = {")
                    .Append(Constants.NEW_LINE);
                paradoxObjectValue.Save(sb, outTab + tab, tab);

                sb.Append(outTab)
                    .Append('}');
            }



            sb.Append(Constants.NEW_LINE);

            string getPostComment()
            {
                if (PostComment == null || PostComment.Length == 0) return "";
                else return " " + PostComment;
            }
        }


    }
}
