using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {    
        public MainWindow()
        {
            InitializeComponent();
            try { DataContext = new ModelView(_tb_nick); }
            catch (Exception) { App.Current.Shutdown(); }         
            send.Click += Send_Click;
            KeyDown += MainWindow_KeyDown;


        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                (DataContext as ModelView).Send(textbox);
        }

        private void _list_msg_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _list_msg.SelectedIndex = _list_msg.Items.Count - 1;
        }
    
        private void Send_Click(object sender, RoutedEventArgs e) => (DataContext as ModelView).Send(textbox);


    }
}
