using System;
using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.src.managers
{
    class FileManager
    {
        public static readonly char PATH_SEPARATOR = Path.DirectorySeparatorChar;
        public static readonly string[] ANY_FORMAT = { "" };
        public static readonly string[] TXT_FORMAT = { ".txt" };
        public static readonly string[] YML_FORMAT = { ".yml" };

        public static Dictionary<string, FileInfo> ReadFileInfos(Settings settings, string subPath, string[] formats)
        {
            var fileInfos = new Dictionary<string, FileInfo>(64);

            if (ReadFileInfos(settings, settings.modDirectory, subPath, fileInfos, formats, true))
                return fileInfos;

            foreach (string path in settings.changableModDirectories)
                if (ReadFileInfos(settings, path, subPath, fileInfos, formats, true)) return fileInfos;

            foreach (string path in settings.unchangableModDirectories)
                if (ReadFileInfos(settings, path, subPath, fileInfos, formats, false)) return fileInfos;

            ReadFileInfos(settings, settings.gameDirectory, subPath, fileInfos, formats, false);

            return fileInfos;
        }

        public static RecursiveFileData ReadRecursiveFileInfos(Settings settings, string subPath, string[] formats)
        {
            var data = new RecursiveFileData();

            if (ReadFileInfos(settings, settings.modDirectory, subPath, data, formats, true))
                return data;

            foreach (string path in settings.changableModDirectories)
                if (ReadFileInfos(settings, path, subPath, data, formats, true)) return data;

            foreach (string path in settings.unchangableModDirectories)
                if (ReadFileInfos(settings, path, subPath, data, formats, false)) return data;

            ReadFileInfos(settings, settings.gameDirectory, subPath, data, formats, false);

            return data;
        }

        public static Dictionary<string, FileInfo> ReadFileInfos(string path, string[] formats, bool needToSave)
        {
            var fileInfos = new Dictionary<string, FileInfo>();
            ReadFileInfos(fileInfos, path, formats, needToSave);
            return fileInfos;
        }

        private static bool ReadFileInfos(Dictionary<string, FileInfo> fileInfos, string path, string[] formats, bool needToSave)
        {
            if (!Directory.Exists(path)) return false;

            foreach (string filePath in Directory.GetFiles(path))
            {
                foreach (string format in formats)
                {
                    if (filePath.Length < format.Length || !filePath.EndsWith(format)) continue;

                    string[] filePathPart = filePath.Split(PATH_SEPARATOR);
                    string fileName = filePathPart[filePathPart.Length - 1];

                    if (!fileInfos.ContainsKey(fileName))
                    {
                        fileInfos[fileName] = new FileInfo(fileName, filePath, needToSave);
                    }
                    break;
                }
            }

            return true;
        }

        private static bool ReadFileInfos(RecursiveFileData data, string path, string[] formats, bool needToSave)
        {
            if (!Directory.Exists(path)) return false;

            data.fileInfos = new Dictionary<string, FileInfo>();
            data.innerDirectories = new Dictionary<string, RecursiveFileData>();

            foreach (string filePath in Directory.GetFiles(path))
            {
                foreach (string format in formats)
                {
                    if (filePath.Length < format.Length || !filePath.EndsWith(format)) continue;

                    string[] filePathParts = filePath.Split(PATH_SEPARATOR);
                    string fileName = filePathParts[filePathParts.Length - 1];

                    if (!data.fileInfos.ContainsKey(fileName))
                    {
                        data.fileInfos[fileName] = new FileInfo(fileName, filePath, needToSave);
                    }
                    break;
                }
            }

            foreach (string dirPath in Directory.GetDirectories(path))
            {
                string[] dirPathParts = dirPath.Split(PATH_SEPARATOR);
                string dirName = dirPathParts[dirPathParts.Length - 1];


                if (!data.innerDirectories.TryGetValue(dirName, out RecursiveFileData innerData))
                {
                    innerData = new RecursiveFileData();
                    data.innerDirectories[dirName] = innerData;
                }
                //TODO Implements replace path check for inner directories
                ReadFileInfos(innerData, dirPath, formats, needToSave);
            }

            return true;
        }

        private static bool ReadFileInfos(Settings settings, string path, string subPath, Dictionary<string, FileInfo> fileInfos, string[] formats, bool needToSave)
        {
            var dirPath = path + subPath;
            ReadFileInfos(fileInfos, dirPath, formats, needToSave);
            return settings.modDescriptors.TryGetValue(path, out ModDescriptor descriptor) && descriptor.replacePathers.Contains(subPath);
        }
        private static bool ReadFileInfos(Settings settings, string path, string subPath, RecursiveFileData data, string[] formats, bool needToSave)
        {
            var dirPath = path + subPath;
            ReadFileInfos(data, dirPath, formats, needToSave);
            return settings.modDescriptors.TryGetValue(path, out ModDescriptor descriptor) && descriptor.replacePathers.Contains(subPath);
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

    public struct RecursiveFileData
    {
        public Dictionary<string, FileInfo> fileInfos;
        public Dictionary<string, RecursiveFileData> innerDirectories;
    }
}
