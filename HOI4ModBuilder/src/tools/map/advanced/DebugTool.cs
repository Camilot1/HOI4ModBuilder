using HOI4ModBuilder.managers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.tools.map.advanced
{
    public class DebugTool
    {
        public DebugTool()
        {
            MainForm.SubscribeGlobalKeyEvent(Keys.D, (sender, e) =>
            {
                if (e.Control && !(e.Shift || e.Alt))
                {
                    MapManager.FontRenderController?.DebugLog();

                    Task.Run(() => MessageBox.Show("FontRenderController DebugLog saved in logs/latest.log"));
                }
            });
        }
    }
}
