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
   public class Server : IDisposable
    {
        private bool _is_listen = true;
       // private string _ip = "192.168.1.105";
        private string _ip = "127.0.0.1";
        private int _port = 9999;
        private TcpListener _listener; // слушатель для входящих сообщений
        public Server()
        {
            _listener = new TcpListener(IPAddress.Parse(_ip), _port); // забиваем данные в слушатель
            _listener.Start(); // запускаем прослушивание            
        }
        public void End_Listen_Async() => _is_listen = false;
        public async void Begin_Listen_Async()
        {
            Console.WriteLine("Bagin await new Connection");
            await Task.Run(() => 
            {
                while (_is_listen) // цикл подключения клиентов
                {                  
                    Client _client = new Client(_listener.AcceptTcpClient()); // новый клиент подключение                  
                    Program._clients.Add(_client); // добавляем клиента в колекцию клиентов
                    Console.WriteLine($"count client : {Client._id} new conection fo server");
                    _client.Start_Listen_Async(); // запускаем асинхроною прослушку сообщений от этого клиента
                   
                }
            });
        }

        public void Dispose() => _is_listen = false;


    }
}
