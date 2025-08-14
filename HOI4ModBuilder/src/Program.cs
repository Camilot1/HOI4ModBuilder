using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.parser;
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
            TransferDataToOutputDirectory();

            TestMain.Execute();
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
        private static void TransferDataToOutputDirectory()
        {
            string debugPath = Application.StartupPath;

            string[] debugPathParts = debugPath.Split(FileManager.PATH_SEPARATOR);
            string basePath = "";
            string releasePath = "";

            if (debugPathParts.Length < 2 || debugPathParts[debugPathParts.Length - 2] != "x64" || debugPathParts[debugPathParts.Length - 1] != "Debug")
                throw new AccessViolationException(debugPath);

            for (int i = 0; i < debugPathParts.Length - 3; i++)
                basePath += debugPathParts[i] + FileManager.PATH_SEPARATOR_STRING;

            for (int i = 0; i < debugPathParts.Length - 1; i++)
                releasePath += debugPathParts[i] + FileManager.PATH_SEPARATOR_STRING;


            debugPath += FileManager.PATH_SEPARATOR_STRING;
            releasePath += "Release" + FileManager.PATH_SEPARATOR_STRING;

            CopyFiles(basePath, debugPath, releasePath, new[] { "data" });
            CopyFiles(basePath, debugPath, releasePath, new[] { "data", "fonts" });
            CopyFiles(basePath, debugPath, releasePath, new[] { "data", "shaders" });
            CopyFiles(basePath, debugPath, releasePath, new[] { "localization" });
            CopyDirectoryRecursive(basePath, debugPath, releasePath, new[] { "data", "savePatterns" });
        }

        private static void CopyFiles(string basePath, string debugPath, string releasePath, string[] directoryToCopy)
        {
            var directoryToCopyPath = FileManager.AssembleFolderPath(directoryToCopy);
            FileManager.CopyFilesFromBetweenDirectories(basePath + directoryToCopyPath, debugPath + directoryToCopyPath);
            FileManager.CopyFilesFromBetweenDirectories(basePath + directoryToCopyPath, releasePath + directoryToCopyPath);
        }

        private static void CopyDirectoryRecursive(string basePath, string debugPath, string releasePath, string[] directoryToCopy)
        {
            var directoryToCopyPath = FileManager.AssembleFolderPath(directoryToCopy);
            FileManager.CopyDirectoryRecursive(basePath + directoryToCopyPath, debugPath + directoryToCopyPath);
            FileManager.CopyDirectoryRecursive(basePath + directoryToCopyPath, releasePath + directoryToCopyPath);
        }
    }
}
