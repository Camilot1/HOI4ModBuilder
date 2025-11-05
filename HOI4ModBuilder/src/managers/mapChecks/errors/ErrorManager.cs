using HOI4ModBuilder.src.managers.errors;
using HOI4ModBuilder.src.managers.mapChecks;
using HOI4ModBuilder.src.managers.mapChecks.errors.checkers;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers
{
    public class ErrorManager : MapCheckerManager
    {
        public static ErrorManager Instance { get; private set; }

        public ErrorManager()
            : base(
                  EnumLocKey.MAP_TAB_PROGRESSBAR_MAP_ERRORS_SEARCHING,
                  256,
                  new Color3F(1f, 0, 0),
                  new Color3F(0, 0, 1f),
                  Enum.GetValues(typeof(EnumMapErrorCode)).Length,
                  new MapChecker[]
                  {
                      new MapCheckerProvincesCoastalMismatches(),
                      new MapCheckerLandProvincesWithNoState(),
                      new MapCheckerSeaProvincesWithState(),
                      new MapCheckerProvincesWithNoRegion(),
                      new MapCheckerProvincesWithMultiStates(),
                      new MapCheckerProvincesWithMultiRegions(),
                      new MapCheckerStateWithMultiRegions(),
                      new CheckRegionWithNotNavalTerrain(),
                      new MapCheckerAdjacencies(),
                      new MapCheckerRiversMismatches(),
                      new MapCheckerStateWithNoOwner()
                  }
              )
        {
            Instance = this;
        }

        public static IEnumerable<(string, Action)> Init()
        {
            if (Instance == null)
                Instance = new ErrorManager();
            return Instance.PrepareExecuteActions();
        }

        protected override void InitFilters()
        {
            for (int i = 0; i < _enabledCodes.Length; i++)
                _enabledCodes[i] = false;

            var filter = SettingsManager.Settings.searchErrorsSettings?.enabled;
            if (filter == null)
                return;

            foreach (var enumObj in Enum.GetValues(typeof(EnumMapErrorCode)))
            {
                if (filter.Contains(enumObj.ToString()))
                    _enabledCodes[(int)enumObj] = true;
            }
        }

        public List<EnumMapErrorCode> GetErrorCodes(Point2D pos, double distance)
        {
            var codes = new List<EnumMapErrorCode>();

            foreach (var p in _poses.Keys)
            {
                if (p.GetDistanceTo(pos) > distance)
                    continue;

                ulong value = _poses[p];

                foreach (EnumMapErrorCode code in Enum.GetValues(typeof(EnumMapErrorCode)))
                {
                    if ((value & (1uL << (int)code)) != 0)
                        codes.Add(code);
                }
            }

            return codes;
        }
    }
}
