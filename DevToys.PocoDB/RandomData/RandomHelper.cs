using System;
using System.Text;

namespace DevToys.PocoDB.RandomData
{ 
    internal static class RandomHelper
    {
        private static char[] _SpecialChars = new char[] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '+', '=', '{', '}', '[', ']', '?', '!' };

        private static int _Seed = DateTime.Now.Millisecond;
        private static Random Rnd = new Random(_Seed);

        public static string RandomWord() => Words.Items[Rnd.Next(0, Words.Items.Length -1)];

        public static string RandomCountry() => Countries.Items[Rnd.Next(0, Countries.Items.Length - 1)];

        public static string RandomStreet() => Streets.Items[Rnd.Next(0, Streets.Items.Length - 1)];

        public static string RandomCity() => Cities.Items[Rnd.Next(0, Cities.Items.Length - 1)];

        public static string RandomCompany() => Companies.Items[Rnd.Next(0, Companies.Items.Length - 1)];

        public static string RandomFirstName() => FirstNames.Items[Rnd.Next(0, FirstNames.Items.Length - 1)];

        public static string RandomLastName() => LastNames.Items[Rnd.Next(0, LastNames.Items.Length - 1)];

        public static string RandomColorName() => Colors.Items[Rnd.Next(0, Colors.Items.Length - 1)];

        public static string RandomGuid() => Guid.NewGuid().ToString();

        public static int RandomNumber(int min, int max) => Rnd.Next(min, max);

        /// <param name="format">@U = Uppercase @L = Lowercase @N = Number</param>
        public static string RandomFormatString(string format)
        {
            StringBuilder _result = new StringBuilder();

            char _prev = ' ';
            foreach (char c in format.ToCharArray())
            {
                char _chr = c;

                if (_prev == '@')
                {
                    if (_chr == 'U')
                        _chr = (char)Rnd.Next('A', 'Z');

                    if (_chr == 'L')
                        _chr = (char)Rnd.Next('a', 'z');

                    if (_chr == 'N')
                        _chr = (char)Rnd.Next('0', '9');
                }
                if (_chr != '@')
                    _result.Append(_chr);

                _prev = _chr;
            }
            return _result.ToString();
        }


        public static string RandomBSNNumber()
        {
            // (9 * A) +(8 * B) +(7 * C) +(6 * D) +(5 * E) +(4 * F) +(3 * G) +(2 * H) +(-1 * I)
            int result = 1;
            int[] vector = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, -1 };
            int[] digits = new int[9];

            while (result != 0)
            {
                for (int ii = 0; ii < digits.Length; ii++)
                    digits[ii] = Rnd.Next(0, 9);

                int value = 0;
                for (int ii = 0; ii < digits.Length; ii++)
                    value = value + (vector[ii] * digits[ii]);
                result = value % 11;
            }

            StringBuilder sb = new StringBuilder();
            for (int ii = 0; ii < digits.Length; ii++)
                sb.Append(digits[ii].ToString());

            return sb.ToString();
        }

        public static Enum RandomEnum(Type enumtype)
        {
            Array _items = Enum.GetValues(enumtype);
            int _max = _items.Length;            
            return (Enum)_items.GetValue(Rnd.Next(0, _max));
        }

        public static string RandomAdress() => $"{RandomZipCode()}, {RandomStreet()} {RandomNumber(1, 200)}, {RandomCity()} ";

        public static string RandomAdressLines() => $"{RandomZipCode()}\r\n {RandomStreet()} {RandomNumber(1, 200)}\r\n {RandomCity()} ";

        public static string RandomText(int wordcount)
        {
            StringBuilder _sb = new StringBuilder();
            for (int ii =0; ii <wordcount; ii++)
            {
                _sb.Append(RandomWord());
            }
            return _sb.ToString();
        }

        public static string RandomUrl() => $"https://www.{RandomCompany()}.com";

        public static DateTime RandomDateTime(DateTime min, DateTime max) => min.AddDays(Rnd.Next((min - max).Days));

        public static string RandomDateTimeFormatted(DateTime min, DateTime max, string format) => min.AddDays(Rnd.Next((min - max).Days)).ToString(format);

        public static double RandomDouble(double start, double end) => (Rnd.NextDouble() * Math.Abs(end - start)) + start;

        public static string RandomFullName() => $"{RandomFirstName()} {RandomLastName()}";

        public static string RandomZipCode()
        {
            int x = RandomNumber(1, 10);
            if (x < 7)
                return RandomFormatString("@N@N@N@N @U@U");
            else
                return RandomNumber(10000, 99999).ToString();
        }

        public static object RandomArrayItem(object[] items) => items[Rnd.Next(0, items.Length - 1)];

        public static bool RandomBool(int percentagetrue) => RandomNumber(0, 100) > percentagetrue ? false : true;

        public static string RandomPassword(int length)
        {
            StringBuilder _sb = new StringBuilder();
            char _chr = ' ';

            for (int ii = 0; ii < length; ii++)
            {
                if (RandomBool(90) == false)
                    _chr = _SpecialChars[Rnd.Next(0, _SpecialChars.Length - 1)];
                else if ((RandomBool(30) == false))
                    _chr = (char)Rnd.Next('0', '9');
                else if ((RandomBool(50) == false))
                    _chr = (char)Rnd.Next('A', 'Z');
                else
                    _chr = (char)Rnd.Next('a', 'z');

                _sb.Append(_chr);
            }
            return _sb.ToString();
        }
    }
}
