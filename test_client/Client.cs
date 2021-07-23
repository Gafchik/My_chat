using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test_client
{
    public class Client
    {
        private NetworkStream _stream;
        private string _username;
        private string _ip = "192.168.1.105";
        private int _port = 9999;
        private TcpClient client = null;
        private void listen(object stream) // метод для слушания входящих сообщений в новом потоке
        {
            try
            {
                NetworkStream _stream = (stream as NetworkStream); // принятый поток от клиента
                while (true)
                {
                    // получаем ответ
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                       
                        bytes = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (_stream.DataAvailable);


                    Console.WriteLine("chat: " + builder.ToString());
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private bool Set_Settings()
        {
            try
            {
                client = new TcpClient(_ip, _port);
                _stream = client.GetStream();
                Thread clientThread = new Thread(new ParameterizedThreadStart(listen));
                clientThread.Start(_stream);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public void Get_started()
        {
            if (!Set_Settings())
                return;
            Console.Write("Введите свое имя:");
            _username = Console.ReadLine();
            try
            {
                while (true)
                {
                    // ввод сообщения
                    string _message = Console.ReadLine();
                    string _msg_send = _username + ": " + _message;
                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.Unicode.GetBytes(_msg_send);
                    // отправка сообщения
                    _stream.Write(data, 0, data.Length);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
