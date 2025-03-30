using HOI4ModBuilder.src.newParser.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameComments : INeedToSave
    {
        public readonly static GameComments DEFAULT = new GameComments();
        private bool _needToSave;
        public bool IsNeedToSave() => _needToSave;
        public void SetNeedToSave(bool needToSave) => _needToSave = needToSave;

        private string _previous;
        public string Previous { get => _previous; set => Utils.Setter(ref _previous, ref value, ref _needToSave); }

        private string _inline;
        public string Inline { get => _inline; set => Utils.Setter(ref _inline, ref value, ref _needToSave); }

        public GameComments()
        {
            _previous = "";
            _inline = "";
        }

        public GameComments(string previous, string inline) : this()
        {
            _previous = previous;
            _inline = inline;
        }
    }
}
