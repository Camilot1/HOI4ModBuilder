using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class PairCommentedObject : PairObject, ICommentedObject
    {
        public GameComments comments = new GameComments();

        public PairCommentedObject() : base() { }

        public PairCommentedObject(IScriptObject key, IScriptObject value) : base(key, value) { }

        public override string ToString()
            => $"{GetKeyword()}<{KeyType?.GetKeyword()}, {ValueType?.GetKeyword()}>(key={GetKey()}, value={GetValue()}" +
            $", prevComments=[\"{string.Join("\", \"", comments?.Previous)}\"], inlineComments=\"{comments?.Inline}\"";

        public new bool IsSameType(IScriptObject scriptObject) => scriptObject is PairCommentedObject;

        public new IScriptObject GetEmptyCopy() => new PairCommentedObject(KeyType.GetEmptyCopy(), ValueType.GetEmptyCopy());

        public new IScriptObject GetCopy()
        {
            var pair = new PairCommentedObject(KeyType.GetEmptyCopy(), ValueType.GetEmptyCopy());

            if (KeyType is IPrimitiveObject)
                pair.SetKey(((IScriptObject)GetKey())?.GetCopy());
            else
                pair.SetKey((IScriptObject)GetKey());

            if (ValueType is IPrimitiveObject)
                pair.SetValue(((IScriptObject)GetValue())?.GetCopy());
            else
                pair.SetValue((IScriptObject)GetValue());

            comments.CopyTo(pair.comments);

            return pair;
        }

        public new string GetKeyword() => PairCommentedDeclarator.GetKeyword();

        public new void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (IsSameType(value) || CanBeConvertedFrom(value))
            {
                var copy = (IPairObject)value.GetCopy();
                SetKey((IScriptObject)copy.GetKey());
                SetValue((IScriptObject)copy.GetValue());

                if (copy is PairCommentedObject copyPair)
                {
                    comments = copyPair.comments;
                }
            }
            else if (value == null)
                throw new VariableIsNotDeclaredScriptException(lineIndex, args, lineIndex);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }


        public void GetPrevComments(int lineIndex, string[] args, IListObject value)
        {
            if (value is null)
                throw new NullPointerScriptException(lineIndex, args);

            if (!(value.GetValueType() is AnyObject) && !(value.GetValueType() is IStringObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, value.GetValueType());

            value.Clear(lineIndex, args);

            foreach (var s in comments.Previous)
                value.Add(lineIndex, args, new StringObject(s));
        }

        public void GetInlineComment(int lineIndex, string[] args, IStringObject value)
        {
            if (value is null)
                throw new NullPointerScriptException(lineIndex, args);

            value.Set(lineIndex, args, new StringObject(comments.Inline));
        }

        public void SetPrevComments(int lineIndex, string[] args, IListObject value)
        {
            if (value is null)
                throw new NullPointerScriptException(lineIndex, args);

            if (!(value.GetValueType() is IStringObject))
                throw new InvalidValueTypeScriptException(lineIndex, args, value.GetValueType());

            IntObject size = new IntObject();
            value.GetSize(lineIndex, args, size);
            comments.Previous = new string[size.Value];

            int index = 0;
            value.ForEach((obj) =>
            {
                comments.Previous[index] = ((IStringObject)obj).GetString();
                index++;
            });
        }

        public void SetInlineComment(int lineIndex, string[] args, IStringObject value)
        {
            if (value is null)
                throw new NullPointerScriptException(lineIndex, args);

            comments.Inline = value.GetString();
        }

    }
}
