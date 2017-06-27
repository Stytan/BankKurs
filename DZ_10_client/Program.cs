using System;

namespace DZ_10_client
{
	class Program
	{
		private static int Select(int N)
        {
            int res = -1;
            do
            {
                Console.Write("Сделайте ваш выбор: ");
                try
                {
                    res = Convert.ToInt32(Console.ReadKey(false).KeyChar.ToString());
                    if (res < 0 || res > N) throw new OverflowException();
                    Console.WriteLine();
                }
                catch (Exception)
                {
                    Console.WriteLine("\nНе верный выбор. Попробуйте снова.");
                }
            } while (res < 0 || res > N);
            return res;
        }
		static void Main(string[] args)
		{
			ExchangeClient client = new ExchangeClient();
			try {
				client.Connect();
				//Устанавливаем точку и запятую разделителем разрядов
				System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
				provider.NumberDecimalSeparator = ".";
				string[] currencies = {"EUR", "RUS", "USD"};
				Console.WriteLine("Программа конвертирует гривни в заданную валюту по актуальному на сегодня курсу");
				do
				{
					Console.WriteLine("1. Вы хотите продать валюту.");
					Console.WriteLine("2. Вы хотите купить валюту.");
					Console.WriteLine("0. Выход.");
					int selDir = Select(2);
					if (selDir == 0)
						break;
					string DirStr = selDir==1 ? "продать" : "купить";
					Console.WriteLine("Какую валюту вы хотите " + DirStr + "?");
					Console.WriteLine("1. EUR\n2. RUS\n3. USD\n0. Вернуться назад");
					int selCurrency = Select(3);
					if (selCurrency == 0)
						continue;
					double sum = 0;
					while (Math.Abs(sum) < 0.01) {
						try{
							Console.WriteLine("Какую сумму валюты вы хотите " + DirStr +
							                  "? (разделитель дробной части - точка)");
							sum = Convert.ToDouble(Console.ReadLine(),provider);
						} catch (FormatException) {
							Console.WriteLine("Не верный формат числа. Число должно быть больше чем 0.01.");
						}
					}
					ExDirection Dir = (ExDirection) ((selCurrency-1)*2+(selDir-1));
					double res = client.Convert(Dir, sum);
					if(selDir==1)
						Console.WriteLine("При продаже "+sum+currencies[selCurrency-1]+" вы получите "+res+"UAH");
					else
						Console.WriteLine("Для покупки "+sum+currencies[selCurrency-1]+" вам нужно "+res+"UAH");
				} while(true);
				client.Close();
			} catch (System.Net.Sockets.SocketException e) {
				Console.WriteLine("Ошибка: Нет подключения к серверу конвертации. (" + e.Message + ")");
			}
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
		}
	}
}
