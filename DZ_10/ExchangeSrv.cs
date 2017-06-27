using System;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DZ_10_srv
{
	class ExchangeSrv
	{
		public struct ExchangeRates
		{
			public double EURbuy; //Курс покупки банком
			public double EURsale; //Курс продажи банком
			public double RUBbuy;
			public double RUBsale;
			public double USDbuy;
			public double USDsale;
		}
		//Структура для хранения актуальных курсов валют
		private ExchangeRates currentRates;
		/// <summary>
		/// Метод загружает актальные курсы валют
		/// или выставляет курсы на 26.06.2017 если нет подключения
		/// </summary>
		/// <returns>Возвращает true если актуальные курсы загружены с сайта</returns>
		private bool GetRates()
		{
			try {
				//Ресурс для загрузки курсов
				String url = @"http://resources.finance.ua/ru/public/currency-cash.xml";
				//Создаём веб-клиент
				WebClient client = new WebClient();
				//Устанавливаем параметры подключения по умолчанию
				client.Proxy.Credentials = CredentialCache.DefaultCredentials;
				//Загружаем данные
				MemoryStream ms = new MemoryStream(client.DownloadData(url));
				//Читаем XML из потока
				XmlReader reader = XmlReader.Create(ms);
				while (reader.ReadToFollowing("organization")) {
					//Ощадбанк 7oiylpmiow8iy1sma9a
					//Приватбанк 7oiylpmiow8iy1sma7w
					if (reader.GetAttribute("id").Equals("7oiylpmiow8iy1sma9a")) {
						//Устанавливаем точку разделителем разрядов
						System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
						provider.NumberDecimalSeparator = ".";
						reader.ReadToFollowing("c");
						if (reader.GetAttribute("id").Equals("EUR")) {
							currentRates.EURbuy = Convert.ToDouble(reader.GetAttribute("br"), provider);
							currentRates.EURsale = Convert.ToDouble(reader.GetAttribute("ar"), provider);
						}
						reader.ReadToFollowing("c");
						if (reader.GetAttribute("id").Equals("RUB")) {
							currentRates.RUBbuy = Convert.ToDouble(reader.GetAttribute("br"), provider);
							currentRates.RUBsale = Convert.ToDouble(reader.GetAttribute("ar"), provider);
						}
						reader.ReadToFollowing("c");
						if (reader.GetAttribute("id").Equals("USD")) {
							currentRates.USDbuy = Convert.ToDouble(reader.GetAttribute("br"), provider);
							currentRates.USDsale = Convert.ToDouble(reader.GetAttribute("ar"), provider);
						}
						ms.Dispose();
						return true;
					}
				}
			} catch (Exception e) {
				currentRates.EURbuy = 28.8;
				currentRates.EURsale = 29.3;
				currentRates.RUBbuy = 0.423;
				currentRates.RUBsale = 0.46;
				currentRates.USDbuy = 25.85;
				currentRates.USDsale = 26.2;
				Console.WriteLine(e.Message);
			}
			return false;
		}
		/// <summary>
		/// Стартует сервер ожидающий подключения клиента
		/// </summary>
		public void Start()
		{
			if (GetRates())
				Console.WriteLine("Актуальный курс валют загружен с ресурса http://resources.finance.ua");
			else
				Console.WriteLine("Не удалось получить актуальные курсы валют. Установлены курсы актуальные на 26.06.2017г.");
            Console.WriteLine("EURbuy = " + currentRates.EURbuy + "; EURsale = " + currentRates.EURsale);
            Console.WriteLine("RUBbuy = " + currentRates.RUBbuy + "; RUBsale = " + currentRates.RUBsale);
            Console.WriteLine("RUBbuy = " + currentRates.USDbuy + "; RUBsale = " + currentRates.USDsale);
			Socket serverSocket = new Socket(
				                               AddressFamily.InterNetwork,
				                               SocketType.Stream, //TCP
				                               ProtocolType.IP);
			IPAddress ip = IPAddress.Parse("127.0.0.1");
			IPEndPoint ep = new IPEndPoint(ip, 13853);
			Socket clientSocket = null;
			try {
				serverSocket.Bind(ep); //Занимает порт
				serverSocket.Listen(10); //Разрешаем сокету слушать входящие подключения
				while (true) {
					clientSocket = serverSocket.Accept(); //Ожидаем подключения клиента
					Console.WriteLine("Клиент подключен");
					StreamReader sr = null;
					StreamWriter sw = null;
					sr = new StreamReader(new NetworkStream(clientSocket));
					sw = new StreamWriter(new NetworkStream(clientSocket));
					string str = "";
					while (str != "/disconnect") {
						double rate = 0;
						str = sr.ReadLine();
						if (str.Equals("/disconnect"))
							break;
						string[] request = str.Split(new char[] { ' ' });
						if (request.Length == 2) {
							switch (request[0]) {
								case "EURsale":
									{
										rate = currentRates.EURsale;
										break; }
								case "EURbuy":
									{
										rate = currentRates.EURbuy;
										break; }
								case "RUBsale":
									{
										rate = currentRates.RUBsale;
										break; }
								case "RUBbuy":
									{
										rate = currentRates.RUBbuy;
										break; }
								case "USDsale":
									{
										rate = currentRates.USDsale;
										break; }
								case "USDbuy":
									{
										rate = currentRates.USDbuy;
										break; }
							}
							try {
								double money = Convert.ToDouble(request[1]);
								str = (money * rate).ToString();
							} catch (FormatException) {
								str = "error";
							}
							sw.WriteLine(str);
							sw.Flush();
						}
					}
					sw.WriteLine("/disconnect");
					sw.Flush();
					Console.WriteLine("Клиент отключен");
				}
			} catch (Exception e) {
				Console.WriteLine(e.StackTrace);
			} finally {
				if (clientSocket != null) {
					clientSocket.Shutdown(SocketShutdown.Both);
					clientSocket.Close();
				}
				serverSocket.Close();
			}
		}
	}
}
