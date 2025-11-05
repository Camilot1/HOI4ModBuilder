namespace HOI4ModBuilder.src.managers.errors
{
    public enum EnumMapErrorCode
    {
        PROVINCE_COASTAL_MISMATCH, //Некорректная прибрежность провинции
        PROVINCE_LAND_WITH_NO_STATE, //Наземная провинция без области
        PROVINCE_SEA_WITH_STATE, //Морская провинция в области
        PROVINCE_WITH_NO_REGION, //Провинция без страт. региона
        PROVINCE_MULTI_STATES, //Провинция находится в нескольких областях
        PROVINCE_MULTI_REGIONS, //Провинция находится в нескольких страт. регионах

        STATE_MULTI_REGIONS, //Область находится в нескольких страт. регионах

        REGION_USES_NOT_NAVAL_TERRAIN, //У региона установлена неморская местность

        RIVER_NO_START_POINT, //Отсутствие точки начала реки
        RIVER_START_POINT_ERROR, //Точка начала реки имеет некорректных пикселей-соседей
        RIVER_FLOW_IN_OR_OUT_ERROR, //
        RIVER_MULTI_START_POINTS, //Несколько стартовых точек у реки

        ADJACENCY_SEA_CROSS_HAS_NO_RULE_NOR_SEA_PROVINCE, //Морской переход не имеет ни правила, ни морской провинции
        ADJACENCY_CONNECTS_DIFFERENT_TYPES_PROVINCES,
        ADJACENCY_ONE_OF_THE_PROVINCES_IS_A_LAKE,
        ADJACENCY_THAT_CONNECTS_SEA_PROVINCES_MUST_HAVE_TYPE_NONE,
        ADJACENCY_START_OR_END_PROVINCE_IS_NULL,
        STATE_WITH_NO_OWNER,
        COASTAL_BUILDING_IN_NOT_COASTAL_LAND_PROVINCE,
    }
}
