using HOI4ModBuilder.src.newParser.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameConstant : ICommentable
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public GameConstant(string key, string value)
        {
            Key = key;
            Value = value;
        }

        private GameComments _comments;
        public GameComments GetComments() => _comments;

        public void SetComments(GameComments comments) => _comments = comments; //TODO isNeedToSave;
    }
}
