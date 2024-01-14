using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.managers
{
    class ActionHistoryManager
    {
        private bool prevRedo;
        private static int _currentPair = -1;
        private static List<ActionPair> _history = new List<ActionPair>();

        public ActionHistoryManager()
        {
            MainForm.SubscribeGlobalKeyEvent(Keys.Z, (sender, e) =>
            {
                if (e.Modifiers == Keys.Control) Undo();
            });
            MainForm.SubscribeGlobalKeyEvent(Keys.Y, (sender, e) =>
            {
                if (e.Modifiers == Keys.Control) Redo();
            });
        }

        public ActionHistoryManager(TabPage tabPage)
        {
            MainForm.SubscribeTabKeyEvent(tabPage, Keys.Z, (sender, e) =>
            {
                if (e.Modifiers == Keys.Control) Undo();
            });
            MainForm.SubscribeTabKeyEvent(tabPage, Keys.Y, (sender, e) =>
            {
                if (e.Modifiers == Keys.Control) Redo();
            });
        }

        public void Add(Action undoAction, Action redoAction)
        {
            Add(new ActionPair(undoAction, redoAction), false);
        }

        public void SilentAdd(Action undoAction, Action redoAction)
        {
            Add(new ActionPair(undoAction, redoAction), true);
        }

        public void Add(ActionPair pair, bool isSilent)
        {
            Logger.TryOrLog(() =>
            {
                //TODO FIX После ctrl+z и добавления нового действия ломается очередь
                if (_currentPair >= 0 && _currentPair < _history.Count)
                {
                    _history.RemoveRange(_currentPair + 1, _history.Count - _currentPair - 1);
                }

                _history.Add(pair);
                if (!isSilent) pair.redo();

                if (_history.Count > SettingsManager.settings.actionHistorySize) _history.RemoveAt(0);
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
    }

    class ActionPair
    {
        public Action undo, redo;

        public ActionPair(Action undo, Action redo)
        {
            this.undo = undo;
            this.redo = redo;
        }
    }
}
