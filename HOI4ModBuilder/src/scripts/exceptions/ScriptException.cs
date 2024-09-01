using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.scripts
{
    public class ScriptException : Exception
    {
        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + lineIndex },
                        { "{args}", string.Join(" ", args) }
                    }
                )
            )
        { }
        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args, Exception ex)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + lineIndex },
                        { "{args}", string.Join(" ", args) }
                    }
                ),
                ex
            )
        { }

        public ScriptException(EnumLocKey enumLoc, int lineIndex, string[] args, int[] allowedCounts)
            : base(
                GuiLocManager.GetLoc(
                    enumLoc,
                    new Dictionary<string, string> {
                        { "{lineIndex}", "" + lineIndex },
                        { "{args}", string.Join(" ", args) },
                        { "{allowedCounts}", string.Join(", ", allowedCounts) }
                    }
                )
            )
        { }
    }
}
