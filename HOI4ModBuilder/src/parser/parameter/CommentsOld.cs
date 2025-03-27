namespace HOI4ModBuilder.src.parser.parameter
{
    public class CommentsOld
    {
        private bool _needToSave;
        public bool NeedToSave => _needToSave;

        private string _previous;
        public string Previous { get => _previous; set => Utils.Setter(ref _previous, ref value, ref _needToSave); }

        private string _inline;
        public string Inline { get => _inline; set => Utils.Setter(ref _inline, ref value, ref _needToSave); }
    }
}
