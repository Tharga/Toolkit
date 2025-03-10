namespace Tharga.Toolkit
{
    public static class StringExtension
    {
        public static string NullIfEmpty(this string item)
        {
            if (item == string.Empty)
            {
                return null;
            }

            return item;
        }

        public static bool IsNullOrEmpty(this string item)
        {
            return string.IsNullOrEmpty(item);
        }

        public static string IfEmpty(this string item, string value)
        {
            if (string.IsNullOrEmpty(item)) return value;
            return item;
        }
    }
}