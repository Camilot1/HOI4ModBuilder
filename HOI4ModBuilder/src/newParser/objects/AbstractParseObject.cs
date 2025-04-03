using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.newParser.objects
{
    public abstract class AbstractParseObject : IParseObject
    {
        public AbstractParseObject()
        {
            var staticAdapter = GetStaticAdapter();
            if (staticAdapter != null)
                foreach (var entry in staticAdapter)
                    ((IParentable)entry.Value.Invoke(this)).SetParent(this);

            var dynamicAdapter = GetDynamicAdapter();
            if (dynamicAdapter != null)
                foreach (var entry in dynamicAdapter)
                    ((IParentable)entry.Value.provider.Invoke(this)).SetParent(this);
        }

        public virtual Dictionary<string, Func<object, object>> GetStaticAdapter() => null;
        public virtual Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;
        public abstract SaveAdapter GetSaveAdapter();
        public abstract IParseObject GetEmptyCopy();
        public AbstractParseObject GetThis() => this;

        private IParentable _parent;
        public IParentable GetParent() => _parent;
        public void SetParent(IParentable parent) => _parent = parent;

        private GameComments _comments;
        public GameComments GetComments() => _comments;
        public void SetComments(GameComments comments) => Utils.Setter(ref _comments, ref comments, ref _needToSave);

        protected bool _needToSave;

        public void SetNeedToSave(bool needToSave) => _needToSave = needToSave;
        public bool IsNeedToSave()
        {
            if (_needToSave)
                return true;

            var staticAdapter = GetStaticAdapter();
            if (staticAdapter != null)
            {
                foreach (var entry in staticAdapter)
                {
                    var param = entry.Value(this);
                    if (param != null && param is INeedToSave iNeedSave && iNeedSave.IsNeedToSave())
                        return true;
                }
            }

            var dynamicAdapter = GetDynamicAdapter();
            if (dynamicAdapter != null)
            {
                foreach (var entry in dynamicAdapter)
                {
                    var param = entry.Value.provider(this);
                    if (param != null && param is INeedToSave iNeedSave && iNeedSave.IsNeedToSave())
                        return true;
                }
            }

            return false;
        }

        public virtual void ParseCallback(GameParser parser)
        {
            parser.ParseUnquoted();
            var key = parser.PullParsedDataString();

            if (key.Length == 0)
                throw new Exception("Invalid key structure: " + parser.GetCursorInfo());

            if (ParseStaticAdapter(parser, key))
                return;

            if (ParseDynamicAdapter(parser, key))
                return;

            throw new Exception("Unknown token: " + key + ": " + parser.GetCursorInfo());
        }

        private bool ParseStaticAdapter(GameParser parser, string key)
        {
            var staticAdapter = GetStaticAdapter();
            if (staticAdapter == null)
                return false;

            if (!staticAdapter.TryGetValue(key, out var provider))
                return false;

            var handler = provider.Invoke(this);
            if (handler is IParseCallbackable parseCallbackable)
                parseCallbackable.ParseCallback(parser);
            else
                throw new Exception("Invalid ParseStaticAdapter handler type: " + parser.GetCursorInfo());

            return true;
        }
        private bool ParseDynamicAdapter(GameParser parser, string key)
        {
            var dynamicAdapter = GetDynamicAdapter();
            if (dynamicAdapter == null)
                return false;

            object newObj = null;
            object providerHandler = null;
            foreach (var entry in dynamicAdapter)
            {
                newObj = entry.Value.factory.Invoke(this, key);
                if (newObj != null)
                {
                    providerHandler = entry.Value.provider.Invoke(this);
                    break;
                }
            }

            if (newObj == null)
                return false;

            if (newObj is IParseCallbackable parseCallbackable)
                parseCallbackable.ParseCallback(parser);
            else
                throw new Exception("Invalid ParseStaticAdapter handler type: " + parser.GetCursorInfo());

            if (providerHandler == null)
                throw new Exception("providerHandler in ParseStaticAdapter is null: " + parser.GetCursorInfo());

            if (providerHandler is IValuePushable valuePushable)
                valuePushable.Push(newObj);
            else
                throw new Exception("providerHandler in ParseStaticAdapter is not IValuePushable: " + parser.GetCursorInfo());

            return true;
        }

        private static readonly Dictionary<string, Func<object, object>> _defaultStaticAdapter = new Dictionary<string, Func<object, object>>();
        private static readonly Dictionary<string, DynamicGameParameter> _defaultDynamicAdapter = new Dictionary<string, DynamicGameParameter>();

        public virtual void Save(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter)
        {
            var saveAdapter = GetSaveAdapter();

            var staticAdapter = GetStaticAdapter();
            if (staticAdapter == null)
                staticAdapter = _defaultStaticAdapter;

            var dynamicAdapter = GetDynamicAdapter();
            if (dynamicAdapter == null)
                dynamicAdapter = _defaultDynamicAdapter;

            if (saveParameter.AddEmptyLineBefore)
                sb.Append(outIndent).Append(Constants.NEW_LINE);

            var comments = GetComments();
            if (comments == null)
            {
                var parent = GetParent();
                if (parent is ICommentable commentable)
                    comments = commentable.GetComments();
            }
            if (comments == null)
                comments = GameComments.DEFAULT;

            if (comments.Previous.Length > 0)
                sb.Append(outIndent).Append(comments.Previous).Append(Constants.NEW_LINE);

            bool isInline = false;
            string innerIndent = outIndent;

            if (key != null)
            {
                sb.Append(outIndent).Append(key).Append(" = {");

                if (comments.Inline.Length > 0)
                    sb.Append(' ').Append(comments.Inline);

                isInline = saveParameter.IsForceInline ||
                    comments.Inline.Length == 0 &&
                    (staticAdapter.Count + dynamicAdapter.Count <= 1);
                innerIndent = isInline ? "" : outIndent + Constants.INDENT;

                if (!isInline)
                    sb.Append(Constants.NEW_LINE);
            }

            foreach (var parameter in saveAdapter.Parameters)
            {
                if (staticAdapter.TryGetValue(parameter.Name, out var staticProvider))
                    ((ISaveable)staticProvider.Invoke(this))
                        .Save(parser, sb, innerIndent, parameter.Name, parameter);
                else if (dynamicAdapter.TryGetValue(parameter.Name, out var dynamicProvider))
                    ((ISaveable)dynamicProvider.provider.Invoke(this))
                        .Save(parser, sb, innerIndent, null, parameter);
            }

            if (key != null)
            {
                //if (!isInline)
                //    sb.Append(Constants.NEW_LINE);

                sb.Append(outIndent).Append('}').Append(Constants.NEW_LINE);
            }
        }


        private Dictionary<string, GameConstant> _constants;
        public Dictionary<string, GameConstant> GetConstants() => _constants;

        public void InitConstantsIfNull()
        {
            if (_constants == null) _constants = new Dictionary<string, GameConstant>();
        }

        public bool TryGetConstant(string name, out GameConstant constant)
        {
            if (_constants != null)
                return _constants.TryGetValue(name, out constant);

            constant = null;
            return false;
        }

        public bool TryGetConstantParentable(string name, out GameConstant constant)
        {
            if (TryGetConstant(name, out constant))
                return true;

            var tempParent = GetParent();

            while (tempParent != null)
            {
                if (tempParent is IConstantable constantable)
                {
                    if (constantable.TryGetConstant(name, out constant))
                        return true;
                }

                tempParent = tempParent.GetParent();
            }

            constant = null;
            return false;
        }

        public string AssemblePath() => ParserUtils.AsseblePath(this);

        public virtual void Validate(LinkedLayer layer)
        {
            var staticAdapter = GetStaticAdapter();
            if (staticAdapter == null)
                staticAdapter = _defaultStaticAdapter;

            var dynamicAdapter = GetDynamicAdapter();
            if (dynamicAdapter == null)
                dynamicAdapter = _defaultDynamicAdapter;

            foreach (var entry in staticAdapter)
            {
                var handler = entry.Value.Invoke(this);
                if (handler is IValidatable validatable)
                    validatable.Validate(new LinkedLayer(layer, entry.Key));
            }

            foreach (var entry in dynamicAdapter)
            {
                var handler = entry.Value.provider.Invoke(this);
                if (handler is IValidatable validatable)
                    validatable.Validate(new LinkedLayer(layer, entry.Key));
            }
        }
    }
}
