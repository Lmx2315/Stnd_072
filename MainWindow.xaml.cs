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

namespace Stnd_072
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Синтезатор    panel_Sint = null;
        Приёмник      panel_Recv = null;
        Калибровка    panel_Cal  = null;
        Инициализация panel_Init = null;
        Consol        panel_Cons = null;

        UDP_server udp0        = null;
        UDP_sender udp0_sender = null;

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        private void Timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("Инициализация.init:"+Инициализация.init);
            if (Инициализация.init == false) Панель_инициализации.IsChecked = false;
            if (Синтезатор.init == false) Панель_синтезатора.IsChecked = false;
        }
        public MainWindow()
        {
            InitializeComponent();
            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 250);
            Timer1.Start();//запускаю таймер 
        }

        private void button_SYS_START_Click(object sender, RoutedEventArgs e)
        {
            UDP_server z = new UDP_server(textBox_IP_072.Text,textBox_Port_072.Text);
            udp0 = z;
            UDP_sender x = new UDP_sender(textBox_IP_dest.Text,textBox_Port_dest.Text);
            udp0_sender=x;
        }
//--------------------обработка создания панелей--------------------------
        private void mnuSint_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.IsChecked)
            {
                if (Синтезатор.init == false)
                {
                    Console.WriteLine("Создаём панель");
                    Синтезатор z = new Синтезатор();
                    panel_Sint = z;
                    panel_Sint.Show();
                }
            } else
            {
                if (Синтезатор.init == true)
                {
                    Console.WriteLine("удаляем панель");
                    Синтезатор.init = false;
                    panel_Sint.Close();                    
                }
            }
            
        }

        private void mnuInit_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.IsChecked)
            {
                if (Инициализация.init == false)
                {
                    Console.WriteLine("Создаём панель");
                    Инициализация z = new Инициализация();
                    panel_Init = z;
                    panel_Init.Show();
                }
            }
            else
            {
                if (Инициализация.init == true)
                {
                    Console.WriteLine("удаляем панель");
                    Инициализация.init = false;
                    panel_Init.Close();
                }
            }
        }

        private void mnuRecv_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.IsChecked)
            {
                if (Приёмник.init == false)
                {
                    Console.WriteLine("Создаём панель");
                    Приёмник z = new Приёмник();
                    panel_Recv = z;
                    panel_Recv.Show();
                }
            }
            else
            {
                if (Приёмник.init == true)
                {
                    Console.WriteLine("удаляем панель");
                    Приёмник.init = false;
                    panel_Recv.Close();
                }
            }
        }

        private void mnuCal_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.IsChecked)
            {
                if (Калибровка.init == false)
                {
                    Console.WriteLine("Создаём панель");
                    Калибровка z = new Калибровка();
                    panel_Cal = z;
                    panel_Cal.Show();
                }
            }
            else
            {
                if (Калибровка.init == true)
                {
                    Console.WriteLine("удаляем панель");
                    Калибровка.init = false;
                    panel_Cal.Close();
                }
            }
        }

        private void mnuCons_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.IsChecked)
            {
                if (Consol.init == false)
                {
                    Console.WriteLine("Создаём панель");
                    Consol z = new Consol();
                    panel_Cons = z;
                    panel_Cons.Show();
                }
            }
            else
            {
                if (Consol.init == true)
                {
                    Console.WriteLine("удаляем панель");
                    Consol.init = false;
                    panel_Cons.Close();
                }
            }
        }

         private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            byte cmd = 203;
            byte[] a = new byte[4];

            udp0_sender.UDP_SEND
                       (
                       cmd, //команда 
                       a,   //данные
                       4,   //число данных в байтах
                       0    //время исполнения , 0 - значит немедленно как сможешь.
                       );

        }
    }
}
