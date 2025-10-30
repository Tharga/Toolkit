namespace Tharga.Toolkit
{
    public static class IntegerExtensions
    {
        public static string GetNameForNumber(int number)
        {
            switch (number)
            {
                case 1:
                    return "Primary";
                case 2:
                    return "Secondary";
                case 3:
                    return "Tertiary";
                case 4:
                    return "Quaternary";
                case 5:
                    return "Quinary";
                case 6:
                    return "Senary";
                case 7:
                    return "Septenary";
                case 8:
                    return "Octonary";
                case 9:
                    return "Nonary";
                case 10:
                    return "Denary";
                default:
                    return $"Number {number}"; // fallback
            }
        }
    }
}