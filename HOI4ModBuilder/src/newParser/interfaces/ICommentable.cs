using HOI4ModBuilder.src.newParser.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.interfaces
{
    public interface ICommentable
    {
        GameComments GetComments();
        void SetComments(GameComments comments);
    }
}
