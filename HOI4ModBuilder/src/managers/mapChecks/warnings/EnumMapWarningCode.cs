namespace HOI4ModBuilder.src.managers.warnings
{
    public enum EnumMapWarningCode
    {
        PROVINCE_WRONG_COLOR, //Слишком тёмный цвет наземной или слишком яркий цвет водной провинции
        PROVINCE_X_CROSS, //4 граничащих пикселя имеют разные цвета
        PROVINCE_DIVIDED, //Провинция разделена на несколько отдельных областей
        PROVINCE_HAS_MORE_THAN_8_BORDERS, //У провинции слишком много границ с другими провинциями
        PROVINCE_CONTINENT_ID_NOT_EXISTS, //
        PROVINCE_WITH_NO_TERRAIN, //
        PROVINCE_BORDERS_MISMATCH, //Некорректный тип соседних провинций
        PROVINCE_MULTI_VICTORY_POINTS, //Провинция имеет несколько разных викторипоинтов

        STATE_VICTORY_POINT_FOR_FOREIGN_PROVINCE, //Область определяет викторипоинты для провинции, находящей в другой области

        HEIGHTMAP_MISMATCH, //Некорректная высота точки провинции
        RAILWAY_PROVINCES_CONNECTION, //Железная дорога соединяет несоседствующие провинции
        RAILWAY_OVERLAP_CONNECTION, //Железная дорога проходит по пути, который уже прошла другая дорога 
        SUPPLY_HUB_NO_CONNECTION, //Узел снабжения не соединён с Ж/Д

        FRONTLINE_POSSIBLE_ERROR //Вероятная ошибка с линией фронта
    }
}
