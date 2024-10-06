using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.src.managers
{
    class ActionHistoryManager
    {
        private bool prevRedo;
        private int _currentPair = -1;
        private List<ActionPair> _history = new List<ActionPair>();

        public ActionHistoryManager()
        {
            MainForm.SubscribeGlobalKeyEvent(Keys.Z, (sender, e) =>
            {
                if (e.Control && !(e.Shift || e.Alt)) Undo();
            });
            MainForm.SubscribeGlobalKeyEvent(Keys.Y, (sender, e) =>
            {
                if (e.Control && !(e.Shift || e.Alt)) Redo();
            });
        }

        public ActionHistoryManager(EnumTabPage tabPage)
        {
            MainForm.SubscribeTabKeyEvent(tabPage, Keys.Z, (sender, e) =>
            {
                if (e.Control && !(e.Shift || e.Alt)) Undo();
            });
            MainForm.SubscribeTabKeyEvent(tabPage, Keys.Y, (sender, e) =>
            {
                if (e.Control && !(e.Shift || e.Alt)) Redo();
            });
        }

        public void Add(Action redoAction, Action undoAction)
            => Add(new ActionPair(redoAction, undoAction), false);

        public void SilentAdd(Action redoAction, Action undoAction)
            => Add(new ActionPair(redoAction, undoAction), true);

        private void Add(ActionPair pair, bool isSilent)
        {
            Logger.TryOrLog(() =>
            {
                if (_currentPair >= 0 && _currentPair < _history.Count)
                    _history.RemoveRange(_currentPair + 1, _history.Count - _currentPair - 1);

                _history.Add(pair);
                if (!isSilent) pair.redo();

                if (_history.Count > SettingsManager.Settings.actionHistorySize) _history.RemoveAt(0);
                else _currentPair++;
            });
        }

        public void Undo()
        {
            Logger.TryOrLog(() =>
            {
                if (_currentPair > -1)
                {
                    _history[_currentPair].undo();
                    _currentPair--;
                }
            });
        }

        public void Redo()
        {
            Logger.TryOrLog(() =>
            {
                if (_currentPair < _history.Count - 1)
                {
                    _currentPair++;
                    _history[_currentPair].redo();
                }
            });
        }

        public void Clear()
        {
            _currentPair = -1;
            _history.Clear();
        }

        public ActionsBatch CreateNewActionBatch() => new ActionsBatch(this);

        public class ActionsBatch
        {
            private readonly ActionHistoryManager _manager;
            private readonly List<ActionPair> _pairs;

            public bool Enabled { get; set; }

            public ActionsBatch(ActionHistoryManager manager)
            {
                _manager = manager;
                _pairs = new List<ActionPair>();
            }

            public void AddWithExecute(Action redo, Action undo)
            {
                if (Enabled)
                {
                    redo();
                    _pairs.Add(new ActionPair(redo, undo));
                }
            }
            public void Add(Action redo, Action undo)
            {
                if (Enabled)
                {
                    _pairs.Add(new ActionPair(redo, undo));
                }
            }

            public void Execute()
            {
                if (_pairs.Count == 0) return;

                var list = new List<ActionPair>(_pairs);
                _manager.Add(
                    () => list.ForEach(pair => pair.redo()),
                    () => list.ForEach(pair => pair.undo())
                );
                _pairs.Clear();
                _pairs.Capacity = 0;
            }
        }

        class ActionPair
        {
            public Action redo, undo;

            public ActionPair(Action redo, Action undo)
            {
                this.redo = redo;
                this.undo = undo;
            }
        }
    }
}
