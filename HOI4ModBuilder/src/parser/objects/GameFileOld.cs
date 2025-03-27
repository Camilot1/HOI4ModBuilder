using HOI4ModBuilder.src.parser.parameter;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.parser.objects
{
    public class GameFileOld : AbstractParseObjectOld
    {
        private readonly FileInfo _fileInfo;
        public string FilePath => _fileInfo.filePath;
        public FileInfo FileInfo => _fileInfo;

        private readonly bool _allowsConstants;
        public new bool IsAllowsConstants() => _allowsConstants;

        private Dictionary<string, Func<object, object>> _adapter;
        //private Dictionary<string, IParseObject> _objects = new Dictionary<string, IParseObject>();

        public GameFileOld(
            FileInfo fileInfo, bool allowsConstants,
            Dictionary<string, Func<object, object>> adapter
        //Dictionary<string, IParseObject> objects
        )
        {
            _fileInfo = fileInfo;
            _allowsConstants = allowsConstants;
            _adapter = adapter;
            //_objects = objects;
        }

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => _adapter;
        public override Dictionary<string, DynamicGameParameterOld> GetDynamicAdapter() => null;

        public override IParseObjectOld GetEmptyCopy()
        {
            var newObjects = new Dictionary<string, IParseObjectOld>();

            //foreach (var entry in _objects)
            //    newObjects[entry.Key] = entry.Value.GetEmptyCopy();

            return new GameFileOld(_fileInfo, _allowsConstants, _adapter/*, newObjects*/);
        }

        public new bool IsNeedToSave()
        {
            if (base.IsNeedToSave())
                return true;

            var constants = GetConstants();
            if (constants != null)
            {
                foreach (var entry in constants)
                    if (entry.Value.NeedToSave)
                        return true;
            }


            return false;
        }
    }
}
