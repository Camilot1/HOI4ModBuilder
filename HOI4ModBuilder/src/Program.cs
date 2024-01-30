using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TransferDataToDebugDirectory();

            Logger.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

        }

        [Conditional("DEBUG")]
        public static void IsDebugCheck(ref bool isDebug)
        {
            isDebug = true;
        }

        [Conditional("DEBUG")]
        private static void TransferDataToDebugDirectory()
        {
            string debugPath = Application.StartupPath;

            string[] debugPathParts = debugPath.Split('\\');
            string baseDirectoryPath = "";

            if (debugPathParts.Length < 2 || debugPathParts[debugPathParts.Length - 2] != "bin")
                throw new AccessViolationException(debugPath);

            for (int i = 0; i < debugPathParts.Length - 2; i++)
                baseDirectoryPath += debugPathParts[i] + "\\";

            FileManager.CopyFilesFromBetweenDirectories(baseDirectoryPath + "data\\", debugPath + "\\data\\");
            FileManager.CopyFilesFromBetweenDirectories(baseDirectoryPath + "localization\\", debugPath + "\\localization\\");
        }
    }
}
