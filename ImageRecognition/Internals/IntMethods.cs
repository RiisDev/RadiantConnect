namespace RadiantConnect.ImageRecognition.Internals
{
    internal static class IntMethods
    {
        internal static bool IsClose(this int caller, int toCheck, int range)
        {
            long minimum = (long)caller - range;
            long maximum = (long)caller + range;

            return toCheck >= minimum && toCheck <= maximum;
        }

        public static bool IsWithinFourSeconds(this TimeOnly time1, TimeOnly time2)
        {
            if (time1.Hour != time2.Hour) return false;
            if (time1.Minute != time2.Minute) return false;
            if (time1.Second.IsClose(time2.Second, 3)) return true;
            if (time1.Millisecond.IsClose(time2.Millisecond, 300)) return true;
            return false;
        }
    }
}
