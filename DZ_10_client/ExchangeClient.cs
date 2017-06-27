using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DZ_10_client
{
	public enum ExDirection
	{
		EURbuy, EURsale, RUBbuy, RUBsale, USDbuy, USDsale
	}
    class ExchangeClient
    {
   		private string[] DirStr = new string[]
   			{"EURbuy", "EURsale", "RUBbuy", "RUBsale", "USDbuy", "USDsale"};
        private Socket clientSocket;
        private IPEndPoint ep;
        private StreamReader sr;
        private StreamWriter sw;
        /// <summary>
        /// Конструктор задаёт параметры подключения
        /// </summary>
        public ExchangeClient()
        {
            clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream, //TCP
                ProtocolType.IP);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            ep = new IPEndPoint(ip, 13853);
        }
        /// <summary>
        /// Выполняет подключение к серверу
        /// </summary>
        public void Connect()
        {
            clientSocket.Connect(ep);
            sr = new StreamReader(new NetworkStream(clientSocket));
            sw = new StreamWriter(new NetworkStream(clientSocket));
        }
        /// <summary>
        /// Конвертирует валюты
        /// </summary>
        /// <param name="direct"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public double Convert(ExDirection direct, double money)
        {
            double res = 0;
            sw.WriteLine(DirStr[(int)direct] + ' ' + money.ToString());
            sw.Flush();
            string tmp = sr.ReadLine();
            if (!tmp.Equals("error"))
            {
                res = System.Convert.ToDouble(tmp);
            }
            else
                throw new ApplicationException("Ошибка конвертации. Возможно не верный формат числа.");
            return res;
        }
        /// <summary>
        /// Закрывает подключение к серверу
        /// </summary>
        public void Close()
        {
            sw.WriteLine("/disconnect");
            sw.Flush();
            sr.ReadLine();
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}
