using System;

namespace It4Medicine
{
    static class Program
    {
        private const string ErrorStr = "please enter X and Y\nwhere X=[1..64], Y=[1..2,147,483,647]";
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                int x, y;
                int.TryParse(args[0], out x);
                int.TryParse(args[1], out y);
                if (x <= 64 && x > 1 && y > 0)
                {
                    try
                    {
                        using (var dispatcher = new Dispatcher())
                        {
                            dispatcher.Start(x, y);
                            while (true)
                            {
                                ConsoleKey key = Console.ReadKey(true).Key;
                                if (key == ConsoleKey.Enter)
                                {
                                    dispatcher.Stop();
                                    break;
                                }
                            }
                            dispatcher.ShowStatistic();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine(ErrorStr);
                }
            }
            else
            {
                Console.WriteLine(ErrorStr);
                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}

