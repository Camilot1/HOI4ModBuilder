using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerFrontlinePossibleErrors : MapChecker
    {
        public MapCheckerFrontlinePossibleErrors()
            : base((int)EnumMapWarningCode.FRONTLINE_POSSIBLE_ERROR, (list) =>
            {
                var borderProvincesIds = new HashSet<ushort>();
                ushort borderProvinceId;

                foreach (var p in ProvinceManager.GetProvinces())
                {
                    //Пропускаем не наземные провинции
                    if (p.Type != EnumProvinceType.LAND) continue;

                    foreach (var b in p.borders)
                    {
                        //Если текущая провинция является провинцией A в границе
                        if (b.provinceA.Id == p.Id)
                        {
                            //Если соседняя провинция не land, то переходим к следующей границе
                            if (b.provinceB.Type != EnumProvinceType.LAND) continue;

                            //Получаем id соседней провинции
                            borderProvinceId = b.provinceB.Id;
                            //Если id соседней провинции уже есть в списке id соседних провинций
                            if (borderProvincesIds.Contains(borderProvinceId))
                            { //То добавляем ошибку и переходим к следующей провинции в списке
                                list.Add(new MapCheckData(p.center.x, p.center.y, (int)EnumMapWarningCode.FRONTLINE_POSSIBLE_ERROR));
                                break;
                            } //Иначе соседней провинции нет в списке id соседних провинций, а значит её надо добавить
                            else borderProvincesIds.Add(borderProvinceId);
                        }
                        else //Иначе текущая провинция является провинцией B в границе
                        {
                            //Если соседняя провинция не land, то переходим к следующей границе
                            if (b.provinceA.Type != EnumProvinceType.LAND) continue;

                            //Получаем id соседней провинции
                            borderProvinceId = b.provinceA.Id;
                            //Если id соседней провинции уже есть в списке id соседних провинций
                            if (borderProvincesIds.Contains(borderProvinceId))
                            { //То добавляем ошибку и переходим к следующей провинции в списке
                                list.Add(new MapCheckData(p.center.x, p.center.y, (int)EnumMapWarningCode.FRONTLINE_POSSIBLE_ERROR));
                                break;
                            } //Иначе соседней провинции нет в списке id соседних провинций, а значит её надо добавить
                            else borderProvincesIds.Add(borderProvinceId);
                        }
                    }

                    //При переходе к след. провинции очищаем список соседних провинций
                    borderProvincesIds.Clear();
                }
            })
        { }

    }
}
