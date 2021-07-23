using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GUI_Client.ViewModel
{
    public class Client_Listener : IDisposable
    {
        public delegate void Publish_msg(string msg); // делегат для ивента  нового сообщения
        public event Publish_msg Notyfy_new_msg; // событие нового сообщения
       // private string _ip = "192.168.1.105"; // ip соединения
        private string _ip = "127.0.0.1"; // ip соединения
        private int _port = 9999; // порт соединения
        private TcpClient _client = new TcpClient(); // тсп клиент для передачи данных по тсп протоколу
        private bool _is_listen = false;
        public bool IsConnectd()
        {
            try { return _client.Connected; }
            catch (NullReferenceException) { return false; }     
        }
        public void Connect()
        {
            _client = new TcpClient(_ip, _port);
            _is_listen = true;
        }
        public void Disconnect()
        {          
            _client?.Dispose();
            _is_listen = false;
        }

        public async void Start_Listen() // запуcк прослушки
        {
            await Task.Run(() =>
           {
               while (_is_listen)
               {
                   try
                   {
                       NetworkStream _stream = _client.GetStream();
                         // получаем ответ
                         byte[] data = new byte[64]; // буфер для получаемых данных
                         StringBuilder builder = new StringBuilder();
                       int bytes = 0; // количество принятых байт

                         while (_stream.DataAvailable)  // проверка  на доступновть потока
                         {
                           bytes = _stream.Read(data, 0, data.Length); // попытка читать поток
                                                                       // если нет байтов будет ждать пока они не появятся
                             builder.Append(Encoding.Unicode.GetString(data, 0, bytes)); // парс с байтов в строку
                           string _received_msg = builder.ToString();
                           if (!_received_msg.Contains("@#$"))
                               Notyfy_new_msg?.Invoke(builder.ToString()); // вызов события нового сообщения
                         }
                   }
                   catch (Exception)
                   {
                       MessageBox.Show("Вы отключены от сервера", "ошибка прослушивания", MessageBoxButton.OK);
                       Disconnect();
                   }
               }
           });
        }
        public void Send_MSG(string username, string msg)
        {
            try
            {
                // ввод сообщения         
                string _msg_send = username + " : " + msg;
                // преобразуем сообщение в массив байтов
                byte[] data = Encoding.Unicode.GetBytes(_msg_send);
                // отправка сообщения
                _client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception) { MessageBox.Show("Вы не подключены", "ошибка отправки", MessageBoxButton.OK);}
        }
        public void Dispose() => _client.Dispose();
        public void Send_test_command()
        {
            try
            {
                // ввод сообщения         
                string _msg_send = "@#$";
                // преобразуем сообщение в массив байтов
                byte[] data = Encoding.Unicode.GetBytes(_msg_send);
                // отправка сообщения
                _client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "ошибка отправки", MessageBoxButton.OK); throw ex; }
        }

    }
}
