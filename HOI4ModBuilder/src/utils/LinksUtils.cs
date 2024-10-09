using HOI4ModBuilder.src.forms.messageForms;
using System.Diagnostics;

namespace HOI4ModBuilder.src.utils
{
    public class LinksUtils
    {
        public static void OpenLink(string link)
        {
            Logger.TryOrLog(() =>
            {
                using (Process.Start(link)) { }
            });
        }
    }
}
