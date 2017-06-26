using System;
using System.Xml;

namespace DZ_10_srv
{
    class Program
    {

        public static void Main(string[] args)
        {
            ExchangeSrv srv = new ExchangeSrv();
            srv.Start();
            Console.WriteLine("EURbuy = " + srv.currentRates.EURbuy + "; EURsale = " + srv.currentRates.EURsale);
            Console.WriteLine("RUBbuy = " + srv.currentRates.RUBbuy + "; RUBsale = " + srv.currentRates.RUBsale);
            Console.WriteLine("RUBbuy = " + srv.currentRates.USDbuy + "; RUBsale = " + srv.currentRates.USDsale);



            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}
