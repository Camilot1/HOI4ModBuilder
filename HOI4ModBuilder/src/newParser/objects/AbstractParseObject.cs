using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.parser.parameter;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.newParser.objects
{
    public abstract class AbstractParseObject : IParseObject
    {
        public abstract Dictionary<string, Func<object, object>> GetStaticAdapter();
        public abstract Dictionary<string, DynamicGameParameter> GetDynamicAdapter();
        public abstract IParseObject GetEmptyCopy();

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
                    var param = entry.Value.parameterProvider(this);
                    if (param != null && param is INeedToSave iNeedSave && iNeedSave.IsNeedToSave())
                        return true;
                }
            }

            return false;
        }

        public void ParseCallback(GameParser parser)
        {
            parser.ParseUnquoted();
            var key = parser.PullParsedDataString();

            if (key.Length == 0)
                throw new Exception("Invalid key structure: " + parser.GetCursorInfo());

            if (ParseStaticAdapter(parser, key))
                return;

            if (ParseDynamicAdapter(key))
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
        private bool ParseDynamicAdapter(string key)
        {
            //TODO

            return true;
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

            if (GetParent() is IConstantable iConstable)
                return iConstable.TryGetConstantParentable(name, out constant);

            constant = null;
            return false;
        }

        public string AssemblePath() => ParserUtils.AsseblePath(this);
    }
}
