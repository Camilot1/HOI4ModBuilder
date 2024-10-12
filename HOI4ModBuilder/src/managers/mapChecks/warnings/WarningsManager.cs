using static HOI4ModBuilder.utils.Structs;
using System.Collections.Generic;
using System;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.managers.mapChecks;
using HOI4ModBuilder.src.managers.mapChecks.warnings.checkers;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.managers
{
    public class WarningsManager : MapCheckerManager
    {
        public static WarningsManager Instance { get; private set; }

        public WarningsManager()
            : base(
                  EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_WARNINGS_SEARCHING,
                  256,
                  new Color3F(1f, 0.65f, 0f),
                  new Color3F(0f, 0f, 1f),
                  Enum.GetValues(typeof(EnumMapWarningCode)).Length,
                  new MapChecker[]
                  {
                      new MapCheckerProvinceWrongColors(),
                      new MapCheckerProvincesXCrosses(),
                      new MapCheckerDividedProvinces(),
                      new MapCheckerProvincesBordersLimit(),
                      new MapCheckerProvincesTerrains(),
                      new MapCheckerProvincesContinents(),
                      new MapCheckerProvincesBordersMismatches(),
                      new MapCheckerProvincesWithMultiVictoryPoints(),
                      new MapCheckerStatesVictoryPointsForForeignProvinces(),
                      new MapCheckerRailways(),
                      new MapCheckerSupplyHubHasNoConnections(),
                      new MapCheckerFrontlinePossibleErrors(),
                      new MapCheckerHeightMapMismatches(),
                  }
              )
        {
            Instance = this;
        }

        public static void Init()
        {
            if (Instance == null)
                Instance = new WarningsManager();
            Instance.Execute();
        }

        protected override void InitFilters()
        {
            for (int i = 0; i < _enabledCodes.Length; i++)
                _enabledCodes[i] = false;

            var filter = SettingsManager.Settings.GetWarningsFilter();
            if (filter == null) return;

            foreach (var enumObj in Enum.GetValues(typeof(EnumMapWarningCode)))
            {
                if (filter.Contains(enumObj.ToString()))
                    _enabledCodes[(int)enumObj] = true;
            }
        }

        public List<EnumMapWarningCode> GetWarningCodes(Point2D pos, double distance)
        {
            var codes = new List<EnumMapWarningCode>();

            foreach (var p in _poses.Keys)
            {
                if (p.GetDistanceTo(pos) <= distance)
                {
                    ulong value = _poses[p];

                    foreach (EnumMapWarningCode code in Enum.GetValues(typeof(EnumMapWarningCode)))
                    {
                        if ((value & (1uL << (int)code)) != 0) codes.Add(code);
                    }
                }
            }

            return codes;
        }

    }
}
