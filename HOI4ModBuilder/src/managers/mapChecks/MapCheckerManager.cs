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
using System.Linq;

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

            var tasks = RunCheckers();
            int startCount = tasks.Count;

            while (tasks.Any(t => !t.IsCompleted))
            {
                int finished = tasks.Count(t => t.IsCompleted);
                MainForm.DisplayProgress(
                    _locKey,
                    null,
                    $"({finished}/{startCount})",
                    finished / (float)startCount
                );
                Thread.Sleep(100);
            }

            CollectCheckersData();
        }

        private List<Task> RunCheckers()
        {
            List<Task> tasks = new List<Task>(_mapCheckers.Length);
            foreach (var checker in _mapCheckers)
            {
                if (checker.Flag >= 0 && !CheckFilter(checker.Flag))
                    continue;

                tasks.Add(Task.Run(() => checker.Execute()));
            }
            return tasks;
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

        public ulong GetErrorInfo(Point2F pos)
        {
            if (_poses.TryGetValue(pos, out var code))
                return code;
            return 0;
        }

        public void SetErrorInfo(Point2F pos, ulong code)
        {
            if (code == 0)
                _poses.Remove(pos);
            else
                _poses[pos] = code;
        }

    }
}
