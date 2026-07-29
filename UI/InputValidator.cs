using System.Globalization;

namespace CraftCalc.UI
{
    public static class InputValidator
    {

        public static string ReadValidString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                Console.WriteLine("Помилка: Поле не може бути порожнім. Спробуйте ще раз.");
            }
        }


        public static decimal ReadValidDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    input = input.Replace(",", ".");
                    if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) && result >= 0)
                    {
                        return result;
                    }
                }
                Console.WriteLine("Помилка: Будь ласка, введіть коректне додатне число (наприклад, 150 або 2.5).");
            }
        }
        public static int ReadValidInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int result) && result >= 0)
                {
                    return result;
                }
                Console.WriteLine("Помилка: Будь ласка, введіть ціле число.");
            }
        }
    }
}
