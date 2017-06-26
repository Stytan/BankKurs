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
            public double EURbuy;
            public double EURsale;
            public double RUBbuy;
            public double RUBsale;
            public double USDbuy;
            public double USDsale;
        }

        public ExchangeRates currentRates;

        public bool GetRates()
        {
            try
            {
                XmlReader reader = XmlReader.Create(@"http://resources.finance.ua/ru/public/currency-cash.xml");
                while (reader.ReadToFollowing("organization"))
                {
                    if (reader.GetAttribute("id").Equals("7oiylpmiow8iy1sma7w"))
                    {
                        System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
                        provider.NumberDecimalSeparator = ".";
                        reader.ReadToFollowing("c");
                        if (reader.GetAttribute("id").Equals("EUR"))
                        {
                            currentRates.EURbuy = Convert.ToDouble(reader.GetAttribute("br"), provider);
                            currentRates.EURsale = Convert.ToDouble(reader.GetAttribute("ar"), provider);
                        }
                        reader.ReadToFollowing("c");
                        if (reader.GetAttribute("id").Equals("RUB"))
                        {
                            currentRates.RUBbuy = Convert.ToDouble(reader.GetAttribute("br"), provider);
                            currentRates.RUBsale = Convert.ToDouble(reader.GetAttribute("ar"), provider);
                        }
                        reader.ReadToFollowing("c");
                        if (reader.GetAttribute("id").Equals("USD"))
                        {
                            currentRates.USDbuy = Convert.ToDouble(reader.GetAttribute("br"), provider);
                            currentRates.USDsale = Convert.ToDouble(reader.GetAttribute("ar"), provider);
                        }
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                currentRates.EURbuy = 28.8;
                currentRates.EURsale = 29.3;
                currentRates.RUBbuy = 0.423;
                currentRates.RUBsale = 0.46;
                currentRates.USDbuy = 25.85;
                currentRates.USDsale = 26.2;
            }
            return false;
        }
        public void Start()
        {
            if (GetRates())
                Console.WriteLine("Актуальный курс валют загружен с ресурса http://resources.finance.ua");
            else
                Console.WriteLine("Не удалось получить актуальные курсы валют. Установлены курсы актуальные на 26.06.2017г.");
            Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream, //TCP
                ProtocolType.IP);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(ip, 13853);
            Socket clientSocket = null;
            try
            {
                serverSocket.Bind(ep); //Занимает порт: Может выдать исключение если этот порт занят
                serverSocket.Listen(10); //Разрешает сокету слушать входящие подключения
                clientSocket = serverSocket.Accept(); //Блокирующий метод, переводит сервер в состояние
                //ожидания подключения, его всегда нужно выносить в отдельный тред
                StreamReader sr = null;
                StreamWriter sw = null;
                sr = new StreamReader(new NetworkStream(clientSocket));
                sw = new StreamWriter(new NetworkStream(clientSocket));
                string str = "";
                while (str != "/disconnect")
                {
                    double rate = 0;
                    str = sr.ReadLine();
                    if (str.Equals("/disconnect")) break;
                    string[] request = str.Split(new char[] { ' ' });
                    if (request.Length == 2)
                    {
                        switch (request[0])
                        {
                            case "EURsale":
                                { rate = currentRates.EURsale; break; }
                            case "EURbuy":
                                { rate = currentRates.EURbuy; break; }
                            case "RUBsale":
                                { rate = currentRates.RUBsale; break; }
                            case "RUBbuy":
                                { rate = currentRates.RUBbuy; break; }
                            case "USDsale":
                                { rate = currentRates.USDsale; break; }
                            case "USDbuy":
                                { rate = currentRates.USDbuy; break; }
                        }
                        try
                        {
                            double money = Convert.ToDouble(request[1]);
                            str = (money * rate).ToString();
                        }
                        catch (FormatException)
                        {
                            str = "error";
                        }
                        sw.WriteLine(str);
                        sw.Flush();
                    }
                }
                sw.WriteLine("/disconnect");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (clientSocket != null)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                serverSocket.Close();
            }
        }
    }
}
