namespace HOI4ModBuilder.src
{
    public class FileInfo
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public string fileName;
        public string filePath;
        public bool needToSave;
        public bool needToDelete;

        public FileInfo(string fileName, string filePath, bool isChangable)
        {
            this.fileName = fileName;
            this.filePath = filePath;
            needToSave = isChangable;
        }

        public override bool Equals(object obj)
        {
            return obj is FileInfo info &&
                   fileName == info.fileName &&
                   filePath == info.filePath;
        }

        public override string ToString()
            => $"FileName: {fileName}; FilePath: {filePath}; NeedToSave: {needToSave}; NeedToDelete: {needToDelete}";
    }
}
