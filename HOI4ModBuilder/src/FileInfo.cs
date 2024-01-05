using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src
{
    class FileInfo
    {
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

        public override int GetHashCode()
        {
            int hashCode = 1864127530;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fileName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(filePath);
            return hashCode;
        }
    }
}
