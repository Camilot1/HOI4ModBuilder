using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.bookmarks
{
    class Bookmark : IParadoxRead
    {
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
    }
}
