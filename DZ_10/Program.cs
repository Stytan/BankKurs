using System;

namespace DZ_10_srv
{
    class Program
    {

        public static void Main(string[] args)
        {
            ExchangeSrv srv = new ExchangeSrv();
            srv.Start();
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}
