using static HOI4ModBuilder.utils.Structs;
using System.Collections.Generic;
using HOI4ModBuilder.src.managers.errors;
using System;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using HOI4ModBuilder.src.utils;
using System.Threading;
using System.Net.Sockets;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.managers.mapChecks
{
    public abstract class MapCheckerManager
    {
        private readonly EnumLocKey _locKey;
        private readonly int _initialCapacity;
        protected Dictionary<Point2F, ulong> _poses;

        protected readonly Color3F _mainColor, _secondColor;
        protected readonly bool[] _enabledCodes;
        private readonly MapChecker[] _mapCheckers;

        public MapCheckerManager(EnumLocKey locKey, int initialCapacity, Color3F mainColor, Color3F secondColor, int enabledCodesSize, MapChecker[] mapCheckers)
        {
            _locKey = locKey;
            _initialCapacity = initialCapacity;
            _mainColor = mainColor;
            _secondColor = secondColor;
            _enabledCodes = new bool[enabledCodesSize];
            _mapCheckers = mapCheckers;
        }

        public void Execute()
        {
            _poses = new Dictionary<Point2F, ulong>(_initialCapacity);
            InitFilters();

            int startedCounter = RunCheckers();

            int runningCounter;
            do
            {
                runningCounter = GetRunningCheckersCount();
                int finishedCounter = startedCounter - runningCounter;

                MainForm.DisplayProgress(
                    _locKey,
                    null,
                    $"({finishedCounter}/{startedCounter})",
                    finishedCounter / (float)startedCounter
                );
                Thread.Sleep(100);
            } while (runningCounter > 0);

            CollectCheckersData();
        }

        private int RunCheckers()
        {
            int startedCounter = 0;
            foreach (var checker in _mapCheckers)
            {
                if (checker.Flag >= 0 && !CheckFilter(checker.Flag))
                    continue;

                startedCounter++;
                Task.Run(() => checker.Execute());
            }
            return startedCounter;
        }

        private int GetRunningCheckersCount()
        {
            int runningCounter = 0;
            foreach (var checker in _mapCheckers)
            {
                if (checker.IsRunning())
                    runningCounter++;
            }
            return runningCounter;
        }

        private void CollectCheckersData()
        {
            foreach (var checker in _mapCheckers)
            {
                if (checker.Values == null)
                    continue;

                foreach (var data in checker.Values)
                {
                    AddErrorInfo(data.key, data.value);
                }
            }
        }

        public void Draw()
        {
            //TODO Оптимизировать: отправлять на видеокарту все точки за один вызов, а не каждую по отдельности
            GL.Color3(_secondColor.r, _secondColor.g, _secondColor.b);
            GL.PointSize(14f);
            GL.Begin(PrimitiveType.Points);
            foreach (Point2F p in _poses.Keys)
                GL.Vertex2(p.x, p.y);
            GL.End();

            GL.Color3(_mainColor.r, _mainColor.g, _mainColor.b);
            GL.PointSize(8f);
            GL.Begin(PrimitiveType.Points);
            foreach (Point2F p in _poses.Keys)
                GL.Vertex2(p.x, p.y);
            GL.End();
        }

        protected abstract void InitFilters();

        public bool CheckFilter(int code) => _enabledCodes[code];

        private void AddErrorInfo(float x, float y, int code) => AddErrorInfo(new Point2F(x, y), code);

        private void AddErrorInfo(Point2F pos, int code)
        {
            _poses.TryGetValue(pos, out ulong errorCode);
            errorCode |= (1uL << code);
            _poses[pos] = errorCode;
        }

    }
}
