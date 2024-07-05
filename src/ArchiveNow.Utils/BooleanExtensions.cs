namespace ArchiveNow.Utils
{
    public static class BooleanExtensions
    {
        public static bool Not(this bool value)
        {
            return !value;
        }

        public static bool IsNotTrue(this bool value)
        {
            return value.Not();
        }
    }
}
