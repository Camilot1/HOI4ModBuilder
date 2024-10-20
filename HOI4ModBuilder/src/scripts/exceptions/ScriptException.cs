using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.scripts
{
    public class ScriptException : Exception
    {
        public ScriptException(EnumLocKey enumLoc, int lineIndex)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + (lineIndex + 1) }
                    }
                )
            )
        { }
        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + (lineIndex + 1) },
                        { "{args}", string.Join(" ", args) }
                    }
                )
            )
        { }
        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args, Dictionary<string, string> otherPairs)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Func<Dictionary<string, string>>(() =>
                    {
                        var dictionary = new Dictionary<string, string> {
                            { "{lineIndex}", "" + (lineIndex + 1) },
                            { "{args}", string.Join(" ", args) }
                        };

                        foreach (var entry in otherPairs)
                            dictionary[entry.Key] = entry.Value;

                        return dictionary;
                    })()
                )
            )
        { }
        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args, int argIndex)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + (lineIndex + 1) },
                        { "{args}", string.Join(" ", args) },
                        { "{argIndex}", "" + argIndex }
                    }
                )
            )
        { }
        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args, object value, int argIndex)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + (lineIndex + 1) },
                        { "{args}", string.Join(" ", args) },
                        { "{value}", "" + value },
                        { "{argIndex}", "" + argIndex }
                    }
                )
            )
        { }

        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args, object value)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + (lineIndex + 1) },
                        { "{args}", string.Join(" ", args) },
                        { "{value}", "" + value }
                    }
                )
            )
        { }

        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args, Exception ex)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + (lineIndex + 1) },
                        { "{args}", string.Join(" ", args) }
                    }
                ),
                ex
            )
        { }

        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args, object value, int[] allowedCounts)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + (lineIndex + 1) },
                        { "{args}", string.Join(" ", args) },
                        { "{value}", "" + value },
                        { "{allowedCounts}", string.Join(", ", allowedCounts) }
                    }
                )
            )
        { }
    }
}
