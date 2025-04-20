using HOI4ModBuilder.src.newParser.interfaces;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameComments : INeedToSave
    {
        private bool _needToSave;
        public bool IsNeedToSave() => _needToSave;
        public void SetNeedToSave(bool needToSave) => _needToSave = needToSave;

        protected string _previous = "";
        public string Previous { get => _previous; set => Utils.Setter(ref _previous, ref value, ref _needToSave); }

        protected string _inline = "";
        public string Inline { get => _inline; set => Utils.Setter(ref _inline, ref value, ref _needToSave); }

        public GameComments()
        {
        }

        public GameComments(string previous, string inline) : this()
        {
            _previous = previous;
            _inline = inline;
        }
    }
}
