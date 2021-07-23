using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test_client
{
    class Program
    {
      
        static void Main(string[] args)
        {
            Client _client = new Client();
            _client.Get_started();
            
            Console.ReadLine();
        }
    }
}
