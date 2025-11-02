using BreakInfinity;

namespace PunchKing
{
    public static class NumberFormatter
    {
        // 한국 수의 단위 (만 = 10^4 단위)
        private static readonly string[] koreanSuffixes = new string[]
        {
            "",        // 1 (10^0)
            "만",      // 10^4
            "억",      // 10^8
            "조",      // 10^12
            "경",      // 10^16
            "해",      // 10^20
            "자",      // 10^24
            "양",      // 10^28
            "구",      // 10^32
            "간",      // 10^36
            "정",      // 10^40
            "재",      // 10^44
            "극",      // 10^48
            "항하사",  // 10^52
            "아승기",  // 10^56
            "나유타",  // 10^60
            "불가사의",// 10^64
            "무량대수" // 10^68
        };

        public static string FormatNumber(BigDouble number)
        {
            if (number < 0)
                return "0";

            // 10,000 미만은 그냥 숫자로 표시
            if (number < 10000)
                return number.ToString("F0");

            // 만 단위 (10^4) 계산
            int magnitude = (int)(number.Exponent / 4);
            double displayValue = number.Mantissa * System.Math.Pow(10, number.Exponent % 4);

            if (magnitude < koreanSuffixes.Length)
            {
                // 1000 이상이면 다음 단위로
                if (displayValue >= 1000)
                {
                    displayValue /= 10000;
                    magnitude++;
                }

                // 단위가 범위를 벗어나면 과학적 표기법
                if (magnitude >= koreanSuffixes.Length)
                {
                    return string.Format("{0:F2}e{1}", number.Mantissa, number.Exponent);
                }

                // 한국 단위로 표시
                return string.Format("{0:F2}{1}", displayValue, koreanSuffixes[magnitude]);
            }

            // 과학적 표기법
            return string.Format("{0:F2}e{1}", number.Mantissa, number.Exponent);
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
