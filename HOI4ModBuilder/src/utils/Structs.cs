using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HOI4ModBuilder.utils
{
    public class Structs
    {
        public class HotKey
        {
            public Keys key;
            public bool control, shift, alt;

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
        }

    }
}
