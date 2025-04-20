using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.newParser.objects
{
    public abstract class GameFile : AbstractParseObject
    {
        private FileInfo _fileInfo;
        public string FilePath => _fileInfo.filePath;
        public FileInfo FileInfo => _fileInfo;

        private bool _allowsConstants;
        public bool IsAllowsConstants() => _allowsConstants;

        public GameFile() : base() { }

        public GameFile(FileInfo fileInfo) : this()
        {
            _fileInfo = fileInfo;
        }

        public GameFile(FileInfo fileInfo, bool allowsConstants) : this(fileInfo)
        {
            _allowsConstants = allowsConstants;
            if (_allowsConstants) InitConstantsIfNull();
        }

        public override bool IsNeedToSave()
            => FileInfo.needToSave || base.IsNeedToSave();

        public void SaveToFile()
        {
            StringBuilder sb = new StringBuilder();
            Save(sb, "", null, default);
        }

        public void SaveToFile(FileInfo fileInfo)
        {
            StringBuilder sb = new StringBuilder();
            Save(sb, "", null, default);

            File.WriteAllText(fileInfo.filePath, sb.ToString());
        }
    }
}
