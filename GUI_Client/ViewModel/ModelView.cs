using GUI_Client.View.ViewModel;
using GUI_Client.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GUI_Client
{
    internal class ModelView : INotifyPropertyChanged
    {
        TextBox _tb_nick;
        private bool _get_connection = true;// переменная  для постоянной проверки наличия соединения
        Client_Listener _listener; // слушатель новых сообщений
     
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged; // ивент обновления
        public void OnPropertyChanged([CallerMemberName] string prop = "")
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        #endregion
        public ObservableCollection<string> _massages { get; set; } // колекция сообщений
        // конструктор 
        public ModelView(TextBox value)
        {
            _tb_nick = value;
            try
            {              
                _listener = new Client_Listener(); // слушатель новых сообщений
                Is_Connection = _listener.IsConnectd().ToString();              
            }
            catch (Exception e) { throw e; }       
            _listener.Notyfy_new_msg += _listener_Notyfy_new_msg; // обработчик события нового сообщения          
            _massages = new ObservableCollection<string>();

        }

        // возвращение булевой переменной соединения
        public bool Get_connection() => _get_connection;

        // обработчик событий нового сообщения
        private void _listener_Notyfy_new_msg(string msg)
        {
            if (App.Current == null)
                return;
            if (!_listener.IsConnectd())
                return;
            App.Current.Dispatcher.Invoke(() =>
          {
              if (_massages != null)
                  _massages.Add(msg);
          });
        }
        // метод для проверки соединения  с сервеом всегда
        public async void Get_Connection_Async()
        {
            await Task.Run(()=> 
            {
                while (_get_connection)
                {
                    try 
                    {
                        _listener.Send_test_command();
                        Thread.Sleep(1000);
                    }
                    catch (Exception) {  _get_connection = false; Disconnect.Execute(_get_connection);  }
                }
            });
        }
        // фул проп никнейма
        private string nickname;

        public string NicKName
        {
            get { return nickname; }
            set { nickname = value; OnPropertyChanged("NicKName"); }
        }


        // команда подключения соединения
        private RelayCommand connet;
        public RelayCommand Connect
        {
            get
            {
                return connet ?? (connet = new RelayCommand(act =>
                {
                    if(nickname == "" || nickname == null)
                    {
                        MessageBox.Show("Вы не указали никнейм", "ошибка подключения", MessageBoxButton.OK);
                        return;
                    }
                    try
                    {
                        _listener.Connect();
                        if (_listener.IsConnectd())
                        {
                            _listener.Start_Listen();
                            Is_Connection = _listener.IsConnectd().ToString();
                            Get_Connection_Async();
                        }
                        _tb_nick.IsEnabled = false;
                    }
                    catch (Exception) { MessageBox.Show("Сервер не отвечает", "ошибка подключения", MessageBoxButton.OK); }                  
                }));
            }
        }

        // команда отключения соединения
        private RelayCommand disconnet;
        public RelayCommand Disconnect
        {
            get
            {
                return disconnet ?? (disconnet = new RelayCommand(act =>
                {
                    try
                    {
                        _listener.Disconnect();
                        Is_Connection = _listener.IsConnectd().ToString();
                        _get_connection = false;
                        Application.Current.Dispatcher.Invoke(() => { _tb_nick.IsEnabled = true; });
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "ошибка отключения", MessageBoxButton.OK); }
                }));
            }
        }
     
        // фул проп соединения
        private string is_connection;
        public string Is_Connection
        {
            get { return is_connection; }
            set { is_connection = value; OnPropertyChanged("Is_Connection"); }
        }

        // отправка сообщения
        internal void Send(TextBox textbox)
        {
            if (textbox.Text != "")
                _listener.Send_MSG(NicKName, textbox.Text);
            textbox.Text = "";
        }

        
    }
}
