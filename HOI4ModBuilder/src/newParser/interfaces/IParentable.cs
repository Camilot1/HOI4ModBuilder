using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.interfaces
{
    public interface IParentable
    {
        IParentable GetParent();
        void SetParent(IParentable parent);
        string AssemblePath();
        bool TryGetGameFile(out GameFile gameFile);
        GameFile GetGameFile();
    }
}
