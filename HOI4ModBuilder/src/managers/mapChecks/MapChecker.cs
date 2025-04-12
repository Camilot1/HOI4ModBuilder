using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.managers.mapChecks
{
    public abstract class MapChecker
    {
        public int Flag { get; private set; }
        public List<MapCheckData> Values { get; private set; }
        private Action<List<MapCheckData>> _action;

        private readonly bool[] _isRunning = new bool[1];
        public bool IsRunning() => _isRunning[0];

        public MapChecker(int flag, Action<List<MapCheckData>> action)
        {
            Flag = flag;
            _action = action;
        }

        public void Clear()
        {
            Values = new List<MapCheckData>(64);
        }

        public void Execute()
        {
            _isRunning[0] = true;
            Clear();
            _action?.Invoke(Values);
            _isRunning[0] = false;
        }
    }

    public struct MapCheckData
    {
        public Point2F key;
        public int value;

        public MapCheckData(Point2F key, int value)
        {
            this.key = key;
            this.value = value;
        }

        public MapCheckData(float x, float y, int value)
        {
            key = new Point2F(x, y);
            this.value = value;
        }
    }
}
