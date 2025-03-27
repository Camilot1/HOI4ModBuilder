using HOI4ModBuilder.src.dataObjects.parameters;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.utils
{
    class ParadoxUtils
    {
        public static ParserFlags<T> Register<T>(Dictionary<string, ParserFlags<T>> register, ParserFlags<T> flags)
        {
            register.Add(flags.TOKEN, flags);
            return flags;
        }
        public static void NewLineIfNeeded(StringBuilder sb, string indent, ref bool flag)
        {
            if (flag)
            {
                sb.Append(indent).Append(Constants.NEW_LINE);
                flag = false;
            }
        }

        public static void StartBlock(StringBuilder sb, string indent, string blockName)
        {
            sb.Append(indent).Append(blockName).Append(" = {").Append(Constants.NEW_LINE);
        }

        public static void StartInlineBlock(StringBuilder sb, string indent, string blockName)
        {
            sb.Append(indent).Append(blockName).Append(" = {");
        }

        public static void EndBlock(StringBuilder sb, string indent)
        {
            sb.Append(indent).Append('}').Append(Constants.NEW_LINE);
        }

        public static void EndInlineBlock(StringBuilder sb, string indent)
        {
            sb.Append(indent).Append("}");
        }

        public static bool Save<T>(StringBuilder sb, string indent, string parameter, T value, T defaultValue)
        {
            if (SaveInline(sb, indent, parameter, value, defaultValue))
            {
                sb.Append(Constants.NEW_LINE);
                return true;
            }
            else return false;
        }

        public static bool SaveQuoted<T>(StringBuilder sb, string indent, string parameter, T value, T defaultValue)
        {
            if (SaveQuotedInline(sb, indent, parameter, value, defaultValue))
            {
                sb.Append(Constants.NEW_LINE);
                return true;
            }
            else return false;
        }

        public static bool Save<T>(StringBuilder sb, string indent, string parameter, T value)
        {
            if (SaveInline(sb, indent, parameter, value))
            {
                sb.Append(Constants.NEW_LINE);
                return true;
            }
            else return false;
        }
        public static bool SaveWithDemiliter<T>(StringBuilder sb, string indent, string parameter, string demiliter, T value)
        {
            if (SaveInlineWithDemiliter(sb, indent, parameter, demiliter, value))
            {
                sb.Append(Constants.NEW_LINE);
                return true;
            }
            else return false;
        }

        public static bool SaveQuoted<T>(StringBuilder sb, string indent, string parameter, T value)
        {
            if (SaveQuotedInline(sb, indent, parameter, value))
            {
                sb.Append(Constants.NEW_LINE);
                return true;
            }
            else return false;
        }
        public static bool SaveQuotedWithDemiliter<T>(StringBuilder sb, string indent, string parameter, string demiliter, T value)
        {
            if (SaveQuotedInlineWithDemiliter(sb, indent, parameter, demiliter, value))
            {
                sb.Append(Constants.NEW_LINE);
                return true;
            }
            else return false;
        }

        public static bool SaveInline<T>(StringBuilder sb, string indent, string parameter, T value, T defaultValue)
        {
            if (value == null || value.Equals(defaultValue)) return false;
            else return SaveInline(sb, indent, parameter, value);
        }
        public static bool SaveQuotedInline<T>(StringBuilder sb, string indent, string parameter, T value, T defaultValue)
        {
            if (value == null || value.Equals(defaultValue)) return false;
            else return SaveQuotedInline(sb, indent, parameter, value);
        }

        public static bool SaveInline<T>(StringBuilder sb, string indent, string parameter, T value)
        {
            return SaveInlineWithDemiliter(sb, indent, parameter, "=", value);
        }

        public static bool SaveInlineWithDemiliter<T>(StringBuilder sb, string indent, string parameter, string demiliter, T value)
        {
            if (value == null) return false;

            sb.Append(indent).Append(parameter).Append(' ').Append(demiliter).Append(' ');

            if (value is bool boolVal) sb.Append(boolVal ? "yes" : "no");
            else if (value is float floatValue) sb.Append(Utils.FloatToString(floatValue));
            else if (value is List<object> list) Utils.ListToString(list, sb);
            else sb.Append(value);

            return true;
        }

        public static bool SaveQuotedInline<T>(StringBuilder sb, string indent, string parameter, T value)
        {
            return SaveQuotedInlineWithDemiliter(sb, indent, parameter, "=", value);
        }
        public static bool SaveQuotedInlineWithDemiliter<T>(StringBuilder sb, string indent, string parameter, string demiliter, T value)
        {
            if (value == null) return false;

            sb.Append(indent).Append(parameter).Append(' ').Append(demiliter).Append(' ');

            sb.Append('\"');

            if (value is bool boolVal) sb.Append(boolVal ? "yes" : "no");
            else if (value is float floatValue) sb.Append(Utils.FloatToString(floatValue));
            else if (value is string sValue) sb.Append(sValue.Replace("\"", "\\\""));
            else sb.Append(value);

            sb.Append('\"');

            return true;
        }
    }
}
