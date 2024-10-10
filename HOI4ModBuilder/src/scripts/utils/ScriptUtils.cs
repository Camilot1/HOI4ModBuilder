using HOI4ModBuilder.src.scripts.exceptions;

namespace HOI4ModBuilder.src.scripts.utils
{
    public class ScriptUtils
    {
        private static int[] daysInMonths = new int[]
        {
            31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
        };

        public static int GetDaysSinceYearStart(int lineIndex, string[] args, int dayIndex, int monthIndex)
        {
            if (monthIndex < 0 || monthIndex >= daysInMonths.Length)
                throw new IndexOutOfRangeScriptException(lineIndex, args, monthIndex);

            int daysInMonth = daysInMonths[monthIndex];

            if (dayIndex < 0 || dayIndex >= daysInMonth)
                throw new IndexOutOfRangeScriptException(lineIndex, args, dayIndex);

            int daysSinceYearStart = 0;
            for (int i = 0; i < monthIndex; i++)
                daysSinceYearStart += daysInMonths[i];

            daysSinceYearStart += dayIndex;
            return daysSinceYearStart;
        }

        public static int GetWeeksSinceYearStart(int lineIndex, string[] args, int dayIndex, int monthIndex)
        {
            int daysSinceYearStart = GetDaysSinceYearStart(lineIndex, args, dayIndex, monthIndex);
            return GetWeeksSinceYearStart(daysSinceYearStart);
        }
        public static int GetWeeksSinceYearStart(int daysSinceYearStart)
        {
            return daysSinceYearStart / 7;
        }
    }
}
