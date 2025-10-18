using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.bookmarks
{
    class Bookmark : IParadoxRead, IComparable<Bookmark>
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public string name;
        public DateTime dateTimeStamp;

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "name":
                    name = parser.ReadString();
                    break;
                case "date":
                    dateTimeStamp = parser.ReadDateTime();
                    break;
            }
        }

        public int CompareTo(Bookmark other) => dateTimeStamp.CompareTo(other.dateTimeStamp);
    }
}
