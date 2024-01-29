using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.src.managers
{
    class FileManager
    {
        public static Dictionary<string, FileInfo> ReadMultiFileInfos(Settings settings, string subDirectoryPath)
        {
            var fileInfos = new Dictionary<string, FileInfo>();
            ReadSingleFileInfos(settings, fileInfos, settings.gameDirectory, subDirectoryPath, false);
            //ReadSingleFileInfos(infos, settings.gameTempDirectory, subDirectoryPath, false);
            foreach (string path in settings.unchangableModDirectories) ReadSingleFileInfos(settings, fileInfos, path, subDirectoryPath, false);
            foreach (string path in settings.changableModDirectories) ReadSingleFileInfos(settings, fileInfos, path, subDirectoryPath, true);
            ReadSingleFileInfos(settings, fileInfos, settings.modDirectory, subDirectoryPath, true);
            return fileInfos;
        }

        private static readonly string[] TXT_FORMAT = { ".txt" };

        public static Dictionary<string, FileInfo> ReadMultiTXTFileInfos(string directoryPath) => ReadMultiFileInfos(directoryPath, TXT_FORMAT);

        public static Dictionary<string, FileInfo> ReadMultiFileInfos(string directoryPath, string[] fileFormats)
        {
            var fileInfos = ReadSingleFileInfos(directoryPath);
            if (fileFormats == null || fileFormats.Length == 0) return fileInfos;

            var newFileInfos = new Dictionary<string, FileInfo>();

            //Идём по именам найденных файлов
            foreach (var fileName in fileInfos.Keys)
            {
                //Идём по разрешённым форматам
                foreach (var format in fileFormats)
                {
                    //Проверяем формат файла
                    if (fileName.Length > format.Length && fileName.EndsWith(format))
                    {
                        newFileInfos[fileName] = fileInfos[fileName];
                        break;
                    }
                }
            }

            return newFileInfos;
        }

        public static Dictionary<string, FileInfo> ReadMultiTXTFileInfos(Settings settings, string subDirectoryPath) => ReadMultiFileInfos(settings, subDirectoryPath, TXT_FORMAT);

        public static Dictionary<string, FileInfo> ReadMultiFileInfos(Settings settings, string subDirectory, string[] fileFormats)
        {
            var fileInfos = ReadMultiFileInfos(settings, subDirectory);
            if (fileFormats == null || fileFormats.Length == 0) return fileInfos;

            var newFileInfos = new Dictionary<string, FileInfo>();

            //Идём по именам найденных файлов
            foreach (var fileName in fileInfos.Keys)
            {
                //Идём по разрешённым форматам
                foreach (var format in fileFormats)
                {
                    //Проверяем формат файла
                    if (fileName.Length > format.Length && fileName.EndsWith(format))
                    {
                        newFileInfos[fileName] = fileInfos[fileName];
                        break;
                    }
                }
            }

            return newFileInfos;
        }

        private static Dictionary<string, FileInfo> ReadSingleFileInfos(string directoryPath)
        {
            string fileName;
            var fileInfos = new Dictionary<string, FileInfo>();

            if (!directoryPath.EndsWith("\\")) directoryPath = directoryPath + "\\";

            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                string[] filePathPart = filePath.Split('\\');
                fileName = filePathPart[filePathPart.Length - 1];

                if (fileInfos.ContainsKey(fileName)) fileInfos.Remove(fileName);

                fileInfos.Add(fileName, new FileInfo(fileName, filePath, false));
            }

            return fileInfos;
        }

        private static void ReadSingleFileInfos(Settings settings, Dictionary<string, FileInfo> fileInfos, string directoryPath, string subDirectoryPath, bool needToSave)
        {
            //Если существует дескриптор мода
            if (settings.modDescriptors.ContainsKey(directoryPath))
            {
                var modDescriptor = settings.modDescriptors[directoryPath];
                //И он заменяет путь к текущей дериктории
                if (modDescriptor.replacePathers.Contains(subDirectoryPath))
                {
                    //То очищаем словарь все файлов из предыдущих дерикторий
                    fileInfos.Clear();
                }
            }

            var dirPath = directoryPath + subDirectoryPath;
            if (!Directory.Exists(dirPath)) return;

            string fileName;

            foreach (string filePath in Directory.GetFiles(dirPath))
            {
                string[] filePathPart = filePath.Split('\\');
                fileName = filePathPart[filePathPart.Length - 1];

                if (fileInfos.ContainsKey(fileName)) fileInfos.Remove(fileName);

                fileInfos.Add(fileName, new FileInfo(fileName, filePath, needToSave));
            }
        }

        public static void CopyFilesFromBetweenDirectories(string srcPath, string destPath)
        {
            if (!Directory.Exists(srcPath))
                throw new DirectoryNotFoundException(srcPath);

            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);

            foreach (var file in Directory.GetFiles(srcPath))
            {
                string destFilePath = destPath + Path.GetFileName(file);
                if (File.Exists(destFilePath)) File.Delete(destFilePath);
                File.Copy(file, destFilePath);
            }
        }
    }
}
