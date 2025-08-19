using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HOI4ModBuilder.utils
{
    public class Structs
    {
        public class HotKey
        {
            public Keys key;
            public bool control, shift, alt;
            public Action<KeyEventArgs> hotKeyEvent;

            public bool CheckKeys(KeyEventArgs e) => !(e.Control ^ control | e.Shift ^ shift | e.Alt ^ alt);
            public override string ToString()
            {
                if (key == Keys.None) return "";
                string value = "[";
                if (control) value += "CTRL+";
                if (shift) value += "SHIFT+";
                if (alt) value += "ALT+";
                value += key.ToString().ToUpper();
                value += "]";
                return value;
            }

            public void SubscribeTabKeyEvent(EnumTabPage tab)
            {
                if (key != Keys.None && hotKeyEvent != null)
                {
                    MainForm.SubscribeTabKeyEvent(
                        tab, key, (sender, e) =>
                        {
                            if (CheckKeys(e))
                                hotKeyEvent(e);
                        }
                    );
                }
            }

            public void SubscribeGlobalKeyEvent()
            {
                if (key != Keys.None && hotKeyEvent != null)
                {
                    MainForm.SubscribeGlobalKeyEvent(
                        key, (sender, e) =>
                        {
                            if (CheckKeys(e))
                                hotKeyEvent(e);
                        }
                    );
                }
            }
        }

    }
}
