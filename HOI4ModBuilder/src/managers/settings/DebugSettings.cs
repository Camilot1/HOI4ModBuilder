using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.managers.settings
{
    public enum EnumDebugValue
    {
        TEXT_RENDER_CHUNKS,
        TEXT_DISABLE_DISTANCE_CUTOFF,
        TEXT_DISABLE_VIEWPORT_CUTOFF
    }
    public class DebugSettings
    {
        public bool isEnabled;
        public DebugTextSettings text;

        public bool CheckDebugValue(EnumDebugValue value)
            => isEnabled && text != null && text.CheckDebugValue(value);
    }

    public class DebugTextSettings
    {
        public bool renderChuncks;
        public bool disableDistanceCutoff;
        public bool disableViewportCutoff;

        public bool CheckDebugValue(EnumDebugValue value)
        {
            switch (value)
            {
                case EnumDebugValue.TEXT_RENDER_CHUNKS: return renderChuncks;
                case EnumDebugValue.TEXT_DISABLE_DISTANCE_CUTOFF: return disableDistanceCutoff;
                case EnumDebugValue.TEXT_DISABLE_VIEWPORT_CUTOFF: return disableViewportCutoff;
            }
            return false;
        }
    }
}
