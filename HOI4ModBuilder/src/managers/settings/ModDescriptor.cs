using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.managers.settings
{

    public class ModDescriptor : IParadoxRead
    {
        public string name;
        public HashSet<string> replacePathers = new HashSet<string>(0);
        public IList<string> tags = new List<string>(0);
        public string picture;
        public string supportedVersion;
        public string removeFileId;

        public ModDescriptor Load(string descriptorPath)
        {
            using (var fs = new FileStream(descriptorPath, FileMode.Open))
                ParadoxParser.Parse(fs, this);
            return this;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "name":
                    name = parser.ReadString();
                    break;
                case "replace_path":
                    replacePathers.Add(parser.ReadString().Replace('/', '\\') + '\\');
                    break;
                case "tags":
                    tags = parser.ReadStringList();
                    break;
                case "picture":
                    picture = parser.ReadString();
                    break;
                case "supported_version":
                    supportedVersion = parser.ReadString();
                    break;
                case "remote_file_id":
                    removeFileId = parser.ReadString();
                    break;
            }
        }
    }
}
