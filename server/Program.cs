using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    class Program
    {
        public static List<Client> _clients = new List<Client>(); // коллекция клиентов
        
        static void Main(string[] args)
        {          
            Server s = new Server();
           s.Begin_Listen_Async();
            Console.WriteLine("press Enter for stop Listen");
            Console.ReadLine();
            s.End_Listen_Async();
            
        }
    }
}

   