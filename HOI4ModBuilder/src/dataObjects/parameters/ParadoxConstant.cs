using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.dataObjects.parameters
{
    public class ParadoxConstant
    {
        public string[] PrefComments { get; set; }
        public string PostComment { get; set; }
        private string value;

        private static readonly Dictionary<Type, Func<string, object>> _mapping = new Dictionary<Type, Func<string, object>>
        {
            { typeof(bool), (v) => {
                var val = v.ToLower();
                if (val == "yes") return true;
                else if (val == "no") return false;
                else throw new FormatException(val);
            } },
            { typeof(byte), (v) => byte.Parse(v) },
            { typeof(ushort), (v) => ushort.Parse(v) },
            { typeof(short), (v) => short.Parse(v) },
            { typeof(int), (v) => int.Parse(v) },
            { typeof(uint), (v) => uint.Parse(v) },
            { typeof(float), (v) => float.Parse(v) },
            { typeof(double), (v) => double.Parse(v) },
            { typeof(string), (v) => v },
        };

        public T GetValue<T>()
        {
            return (T)_mapping[typeof(T)](value);
        }

        public void Save(StringBuilder sb, string outTab, string name)
        {
            if (PrefComments != null && PrefComments.Length > 0)
            {
                sb.Append(outTab).Append(Constants.NEW_LINE);
                foreach (var comment in PrefComments)
                {
                    sb.Append(outTab).Append(comment).Append(Constants.NEW_LINE);
                }
            }

            sb.Append(outTab).Append(name).Append(" = ").Append(value);

            if (PostComment != null && PostComment.Length > 0)
            {
                sb.Append(' ').Append(PostComment);
            }

            sb.Append(Constants.NEW_LINE);
        }
    }
}
