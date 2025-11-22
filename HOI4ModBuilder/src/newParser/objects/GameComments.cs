using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameComments : INeedToSave
    {
        private bool _needToSave;
        public bool IsNeedToSave() => _needToSave;
        public void SetNeedToSave(bool needToSave) => _needToSave = needToSave;

        protected string[] _previous = Array.Empty<string>();
        public string[] Previous { get => _previous; set => Utils.Setter(ref _previous, ref value, ref _needToSave); }

        protected string _inline = string.Empty;
        public string Inline { get => _inline; set => Utils.Setter(ref _inline, ref value, ref _needToSave); }

        public GameComments()
        {
        }

        public GameComments(string[] previous, string inline) : this()
        {
            _previous = previous ?? Array.Empty<string>();
            _inline = inline ?? string.Empty;
        }

        public void CopyTo(GameComments obj)
        {
            obj.Previous = Utils.CopyArray(Previous);
            obj.Inline = Inline;
        }

        public bool HasAnyData()
            => _previous.Length > 0 || !string.IsNullOrEmpty(_inline);

        public void SavePrevComments(StringBuilder sb, string indent)
        {
            foreach (var prevComment in Previous)
                sb.Append(indent).Append(prevComment).Append(Constants.NEW_LINE);
        }
    }
}
