using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public class Client : IDisposable
    {
        public static int _id = 0;
        private TcpClient _tcp_client;
        private bool _is_listen = true;
        public Client(TcpClient tcpClient)
        {
            _tcp_client = tcpClient;
            _id++;
        }
        private void Send_All(string msg)
        {
            try
            {
                byte[] data = new byte[64]; // буфер для получаемых данных
                data = Encoding.Unicode.GetBytes(msg);
                Program._clients.ForEach(i => i._tcp_client.GetStream().Write(data, 0, data.Length));
            }
            catch (Exception ex) { throw ex; }
           
        }
        public async void Start_Listen_Async()
        {

            await Task.Run(() =>
            {
                NetworkStream stream = null;// должен быть создан только в это методе
                                            // потому что не будет к нему доступа в другом потоке
                byte[] data = new byte[64]; // буфер для получаемых данных
                try
                {
                    stream = _tcp_client.GetStream();// получаем поток данных с клиента
                    while (_is_listen)
                    {
                        // получаем сообщение
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        string message = builder.ToString();
                        if (message == "")
                            throw new Exception();
                        if ("@#$" == message)
                            stream.Write(data, 0, data.Length);
                        else
                        {
                            Send_All(message); // отправляем  сообщение всем
                            Console.WriteLine($"{_id} : " + message);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("отключился :" + _id.ToString());
                    _is_listen = false;
                    Program._clients.Remove(this);
                    _id--;
                }
            });


        }

        public void Dispose() => _tcp_client.Close();
       
    }
}

