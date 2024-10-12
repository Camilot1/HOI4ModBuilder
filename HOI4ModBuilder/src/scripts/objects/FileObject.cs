using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.utils;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.scripts.objects.interfaces
{
    public class FileObject : IFileObject
    {
        public string FilePath { get; set; }

        public FileObject(string filePath) { FilePath = filePath; }
        public FileObject() : this("") { }

        public IScriptObject GetEmptyCopy() => new FileObject();
        public IScriptObject GetCopy() => new FileObject(FilePath);
        public string GetKeyword() => FileDeclarator.GetKeyword();
        public object GetValue() => FilePath;
        public bool IsSameType(IScriptObject scriptObject) => scriptObject is FileObject;
        public override string ToString() => GetKeyword() + "(" + FilePath + ")";
        public override bool Equals(object obj)
            => obj is FileObject @object &&
                   FilePath == @object.FilePath;
        public override int GetHashCode() => FilePath.GetHashCode();

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is StringObject stringObject)
                FilePath = stringObject.Value;
            else if (value is FileObject fileObject)
                FilePath = fileObject.FilePath;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Append(int lineIndex, string[] args, IScriptObject value)
        {
            Logger.TryOrCatch(
                () =>
                {
                    string text = value.GetValue().ToString();
                    text = ScriptParser.ReplaceSpecialChars(text);
                    File.AppendAllText(FilePath, text);
                },
                (ex) => throw new InternalScriptException(lineIndex, args, ex)
            );
        }

        public void AppendRange(int lineIndex, string[] args, IScriptObject value)
        {
            Logger.TryOrCatch(
                () =>
                {
                    string text = FormatCollectionToText(lineIndex, args, value);
                    text = ScriptParser.ReplaceSpecialChars(text);
                    File.AppendAllText(FilePath, text);
                },
                (ex) => throw new InternalScriptException(lineIndex, args, ex)
            );
        }

        public void Write(int lineIndex, string[] args, IScriptObject value)
        {
            Logger.TryOrCatch(
                () =>
                {
                    string text = value.GetValue().ToString();
                    text = ScriptParser.ReplaceSpecialChars(text);
                    File.WriteAllText(FilePath, text);
                },
                (ex) => throw new InternalScriptException(lineIndex, args, ex)
            );
        }

        public void WriteRange(int lineIndex, string[] args, IScriptObject value)
        {
            Logger.TryOrCatch(
                () =>
                {
                    string text = FormatCollectionToText(lineIndex, args, value);
                    text = ScriptParser.ReplaceSpecialChars(text);
                    File.WriteAllText(FilePath, text);
                },
                (ex) => throw new InternalScriptException(lineIndex, args, ex)
            );
        }


        private string FormatCollectionToText(int lineIndex, string[] args, IScriptObject value)
        {
            string text;

            if (!(value is ICollectionObject))
                throw new InvalidOperationScriptException(lineIndex, args, value);

            var sb = new StringBuilder();
            if (value is IListObject listObject)
            {
                listObject.ForEach((obj) =>
                {
                    sb.Append(obj.GetValue().ToString()).Append('\n');
                });
                text = sb.ToString();
            }
            else if (value is IMapObject mapObject)
            {
                mapObject.ForEach((keyObj, valueObj) =>
                {
                    sb.Append(keyObj.GetValue().ToString()).Append('=').Append(valueObj.GetValue().ToString()).Append('\n');
                });
                text = sb.ToString();
            }
            else throw new InvalidValueTypeScriptException(lineIndex, args, value);

            return text;
        }
    }
}
