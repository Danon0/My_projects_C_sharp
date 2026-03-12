using System;

namespace SimpleCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== КАЛЬКУЛЯТОР ===");
            Console.WriteLine("----------------------------------------");

            while (true)
            {
                try
                {
                    Console.Write("Введите первое число: ");
                    string input1 = Console.ReadLine();

                    if (input1.ToLower() == "q")
                        break;

                    if (!double.TryParse(input1, out double num1))
                    {
                        Console.WriteLine("Ошибка! Введите корректное число.\n");
                        continue;
                    }

                    Console.Write("Введите второе число: ");
                    string input2 = Console.ReadLine();

                    if (input2.ToLower() == "q")
                        break;

                    if (!double.TryParse(input2, out double num2))
                    {
                        Console.WriteLine("Ошибка! Введите корректное число.\n");
                        continue;
                    }

                    Console.Write("Выберите операцию (+, -, *, /) или 'q' для выхода: ");
                    string operation = Console.ReadLine();

                    if (operation.ToLower() == "q" || string.IsNullOrEmpty(operation))
                        break;

                    double result = 0;
                    bool validOperation = true;

                    switch (operation)
                    {
                        case "+":
                            result = num1 + num2;
                            break;
                        case "-":
                            result = num1 - num2;
                            break;
                        case "*":
                            result = num1 * num2;
                            break;
                        case "/":
                            if (num2 == 0)
                            {
                                Console.WriteLine("Ошибка! Деление на ноль невозможно.\n");
                                validOperation = false;
                            }
                            else
                            {
                                result = num1 / num2;
                            }
                            break;
                        default:
                            Console.WriteLine("Ошибка! Неизвестная операция.\n");
                            validOperation = false;
                            break;
                    }

                    if (validOperation)
                    {
                        Console.WriteLine($"Результат: {num1} {operation} {num2} = {result}\n");
                    }

                    Console.WriteLine("----------------------------------------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}\n");
                }
            }

            Console.WriteLine("Программа завершена. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}