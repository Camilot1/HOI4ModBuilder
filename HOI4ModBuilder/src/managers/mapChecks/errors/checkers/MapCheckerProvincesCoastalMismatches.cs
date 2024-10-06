﻿using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerProvincesCoastalMismatches : MapChecker
    {
        public MapCheckerProvincesCoastalMismatches()
            : base((int)EnumMapErrorCode.PROVINCE_COASTAL_MISMATCH, (list) =>
            {
                foreach (var p in ProvinceManager.GetProvinces())
                {
                    if (p.IsCoastal != p.CheckCoastalType())
                        list.Add(new MapCheckData(p.center, (int)EnumMapErrorCode.PROVINCE_COASTAL_MISMATCH));
                }
            })
        { }
    }
}
