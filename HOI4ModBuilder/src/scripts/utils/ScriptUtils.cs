using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.utils;
using System.Text;

namespace HOI4ModBuilder.src.scripts.utils
{
    public class ScriptUtils
    {
        private static int[] _daysInMonths = new int[]
        {
            31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
        };

        public static int GetDaysSinceYearStart(int lineIndex, string[] args, int dayIndex, int monthIndex)
        {
            if (monthIndex < 0 || monthIndex >= _daysInMonths.Length)
                throw new IndexOutOfRangeScriptException(lineIndex, args, monthIndex);

            int daysInMonth = _daysInMonths[monthIndex];

            if (dayIndex < 0 || dayIndex >= daysInMonth)
                throw new IndexOutOfRangeScriptException(lineIndex, args, dayIndex);

            int daysSinceYearStart = 0;
            for (int i = 0; i < monthIndex; i++)
                daysSinceYearStart += _daysInMonths[i];

            daysSinceYearStart += dayIndex;
            return daysSinceYearStart;
        }

        public static int GetWeeksSinceYearStart(int lineIndex, string[] args, int dayIndex, int monthIndex)
        {
            int daysSinceYearStart = GetDaysSinceYearStart(lineIndex, args, dayIndex, monthIndex);
            return GetWeeksSinceYearStart(daysSinceYearStart);
        }
        public static int GetWeeksSinceYearStart(int daysSinceYearStart)
            => daysSinceYearStart / 7;

        public static void GetImplements(StringBuilder sb, object obj)
        {
            if (obj is IScriptObject) sb.Append("ISCRIPTOBJECT ");

            if (obj is ICollectionObject) sb.Append("ICOLLECTION ");
            if (obj is IFileObject) sb.Append("IFILE ");
            if (obj is IListObject) sb.Append("ILIST ");
            if (obj is ILogicalObject) sb.Append("ILOGICAL ");
            if (obj is IMapObject) sb.Append("IMAP ");
            if (obj is INumberObject) sb.Append("INUMBER ");
            if (obj is IRelativeObject) sb.Append("IRELATIVE ");
            if (obj is IStringObject) sb.Append("ISTRING ");

            if (obj is IAddObject) sb.Append("IADD ");
            if (obj is IAddRangeObject) sb.Append("IADDRANGE ");
            if (obj is IAndObject) sb.Append("IAND ");
            if (obj is IAppendObject) sb.Append("IAPPEND ");
            if (obj is IClearObject) sb.Append("ICLEAR ");
            if (obj is IDivideObject) sb.Append("IDIVIDE ");
            if (obj is IGetObject) sb.Append("IGET ");
            if (obj is IGetSizeObject) sb.Append("IGETSIZE ");
            if (obj is IInsertObject) sb.Append("IINSERT ");
            if (obj is IModuloObject) sb.Append("IMODULO ");
            if (obj is IMultiplyObject) sb.Append("IMULTIPLY ");
            if (obj is INotObject) sb.Append("INOT ");
            if (obj is IOrObject) sb.Append("IOR ");
            if (obj is IPrimitiveObject) sb.Append("IPRIMITIVE ");
            if (obj is IPutObject) sb.Append("IPUT ");
            if (obj is IRemoveAtObject) sb.Append("IREMOVEAT ");
            if (obj is IRemoveObject) sb.Append("IREMOVE ");
            if (obj is ISetObject) sb.Append("ISET ");
            if (obj is ISetSizeObject) sb.Append("ISETSIZE ");
            if (obj is ISubtractObject) sb.Append("ISUBTRACT ");
            if (obj is ITrimObject) sb.Append("ITRIM ");
            if (obj is IWriteObject) sb.Append("IWRITE ");
            if (obj is IXorObject) sb.Append("IXOR ");
        }
    }
}
