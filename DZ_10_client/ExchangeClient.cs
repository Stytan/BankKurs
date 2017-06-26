using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DZ_10_client
{
    class ExchangeClient
    {
        private Socket clientSocket;
        private IPEndPoint ep;
        private StreamReader sr;
        private StreamWriter sw;
        public ExchangeClient()
        {
            clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream, //TCP
                ProtocolType.IP);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            ep = new IPEndPoint(ip, 13853);
        }
        public void Connect()
        {
            clientSocket.Connect(ep);
            sr = new StreamReader(new NetworkStream(clientSocket));
            sw = new StreamWriter(new NetworkStream(clientSocket));
        }
        public double Convert(string direct, double money)
        {
            double res = 0;
            sw.WriteLine(direct + ' ' + money.ToString());
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
