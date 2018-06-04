namespace NETMP.Module9.Nuget
{
    public static class CoolUtils
    {
        public static bool IsEven(int value)
        {
            return !IsOdd(value);
        }

        public static bool IsOdd(int value)
        {
            return !IsEven(value);
        }
    }
}
