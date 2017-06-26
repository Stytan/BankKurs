using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_10_client
{
    class Program
    {
        static void Main(string[] args)
        {
            ExchangeClient client = new ExchangeClient();
            client.Connect();
            string str = "RUBbuy";
            Console.WriteLine(client.Convert("RUBbuy", 100.5));
            client.Close();
        }
    }
}
