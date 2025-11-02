using BreakInfinity;

namespace PunchKing
{
    public static class NumberFormatter
    {
        private static readonly string[] suffixes = new string[]
        {
            "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc",
            "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd", "Nod", "Vg",
            "Uvg", "Dvg", "Tvg", "Qavg", "Qivg", "Sxvg", "Spvg", "Ocvg", "Novg", "Tg"
        };

        public static string FormatNumber(BigDouble number)
        {
            if (number < 1000)
                return number.ToString("F0");

            if (number < 0)
                return "0";

            int magnitude = (int)(number.Exponent / 3);
            double displayValue = number.Mantissa * System.Math.Pow(10, number.Exponent % 3);

            if (magnitude < suffixes.Length)
            {
                return $"{displayValue:F2}{suffixes[magnitude]}";
            }

            // 과학적 표기법
            return $"{number.Mantissa:F2}e{number.Exponent}";
        }

        public static string FormatTime(float seconds)
        {
            if (seconds < 60)
                return $"{seconds:F0}s";
            else if (seconds < 3600)
                return $"{(seconds / 60):F0}m {(seconds % 60):F0}s";
            else
                return $"{(seconds / 3600):F0}h {((seconds % 3600) / 60):F0}m";
        }
    }
}
