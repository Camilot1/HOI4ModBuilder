using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.Pdoxcl2Sharp
{
    class AdvancedParadoxSaver
    {
        private readonly StringBuilder _sb;
        private readonly string _indent;

        private readonly SaveInfoPattern _pattern;
        private readonly Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        private readonly bool[] LastParagraphHasAnyInfo = new bool[] { false };

        public AdvancedParadoxSaver(SaveInfoPattern pattern, StringBuilder sb, string indent)
        {
            _pattern = pattern;
            _sb = sb;
            _indent = indent;
        }

        public bool Execute()
        {
            if (_pattern == null)
                throw new Exception($"AdvancedParadoxSaver does not contain SaveInfoPattern instance");

            void spacingAction()
            {
                _sb.Append(_indent).Append(Constants.NEW_LINE);
                LastParagraphHasAnyInfo[0] = false;
            }

            return _pattern.Execute(_actions, spacingAction, LastParagraphHasAnyInfo);
        }

        public void Clear()
        {
            _actions.Clear();
            LastParagraphHasAnyInfo[0] = false;
        }

        public AdvancedParadoxSaver Save(string parameter, Action<bool[]> action)
        {
            _actions[parameter] = () => action(LastParagraphHasAnyInfo);
            return this;
        }

        public AdvancedParadoxSaver Save<T>(string parameter, T value, T defaultValue)
        {
            _actions[parameter] = () => LastParagraphHasAnyInfo[0] |= ParadoxUtils.Save(_sb, _indent, parameter, value, defaultValue);
            return this;
        }

        public AdvancedParadoxSaver SaveQuoted<T>(string parameter, T value, T defaultValue)
        {
            _actions[parameter] = () => LastParagraphHasAnyInfo[0] |= ParadoxUtils.SaveQuoted(_sb, _indent, parameter, value, defaultValue);
            return this;
        }

        public AdvancedParadoxSaver Save<T>(string parameter, T value)
        {
            _actions[parameter] = () => LastParagraphHasAnyInfo[0] |= ParadoxUtils.Save(_sb, _indent, parameter, value);
            return this;
        }

        public AdvancedParadoxSaver SaveQuoted<T>(string parameter, T value)
        {
            _actions[parameter] = () => LastParagraphHasAnyInfo[0] |= ParadoxUtils.SaveQuoted(_sb, _indent, parameter, value);
            return this;
        }

        public AdvancedParadoxSaver SaveInline<T>(string parameter, T value, T defaultValue)
        {
            _actions[parameter] = () => LastParagraphHasAnyInfo[0] |= ParadoxUtils.SaveInline(_sb, _indent, parameter, value, defaultValue);
            return this;
        }
        public AdvancedParadoxSaver SaveQuotedInline<T>(string parameter, T value, T defaultValue)
        {
            _actions[parameter] = () => LastParagraphHasAnyInfo[0] |= ParadoxUtils.SaveQuotedInline(_sb, _indent, parameter, value, defaultValue);
            return this;
        }

        public AdvancedParadoxSaver SaveInline<T>(string parameter, T value)
        {
            _actions[parameter] = () => LastParagraphHasAnyInfo[0] |= ParadoxUtils.SaveInline(_sb, _indent, parameter, value);
            return this;
        }

        public AdvancedParadoxSaver SaveQuotedInline<T>(string parameter, T value)
        {
            _actions[parameter] = () => LastParagraphHasAnyInfo[0] |= ParadoxUtils.SaveQuotedInline(_sb, _indent, parameter, value);
            return this;
        }
    }

    class SaveInfoParameter
    {
        [JsonProperty("parameter")]
        public string Name { get; private set; }
        [JsonProperty("useSpacing")]
        public bool UseSpacing { get; private set; }
    }

    class SaveInfoPattern
    {
        [JsonProperty("saveOrder")]
        private List<SaveInfoParameter> _saveOrder;

        public bool Execute(Dictionary<string, Action> actions, Action spacingAction, bool[] LastParagraphHasAnyInfo)
        {
            if (_saveOrder == null)
                throw new Exception("SaveInfoPattern does not have \"saveOrder\"");

            bool flag = false;

            foreach (var parameter in _saveOrder)
            {
                if (!actions.TryGetValue(parameter.Name, out var action)) continue;

                if (LastParagraphHasAnyInfo[0] && parameter.UseSpacing) spacingAction();
                action();

                flag |= LastParagraphHasAnyInfo[0];
            }

            return flag;
        }
    }
}
