﻿using System;
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

        public UDP_server udp0 ;
        public UDP_sender udp0_sender ;
        int FLAG_SINT_INIT = 0;

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        private void Timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("Инициализация.init:"+Инициализация.init);
            if (Инициализация.init == false) Панель_инициализации.IsChecked = false;
            if (Синтезатор.init    == false) Панель_синтезатора.IsChecked   = false;
            if (Приёмник.init      == false) Панель_приёмника.IsChecked     = false;
            if (Калибровка.init    == false) Панель_калибровки.IsChecked    = false;
            if (Consol.init        == false) Панель_консоли.IsChecked       = false;

            /*
            if (FLAG_SINT_INIT==0)
            {
                FLAG_SINT_INIT = 1;
                Панель_синтезатора.IsChecked = true;
                mnuSint_Click(Панель_синтезатора, null);
            }
            */
        }
        public MainWindow()
        {
            InitializeComponent();
            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 250);
            Timer1.Start();//запускаю таймер 

            UDP_server z = new UDP_server(textBox_IP_072.Text,textBox_Port_072.Text);
            udp0 = z;
            UDP_sender x = new UDP_sender(textBox_IP_dest.Text,textBox_Port_dest.Text);
            udp0_sender=x;

        }

        private void button_SYS_START_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Инициализируем сервер");
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
                    Синтезатор z = new Синтезатор(this);
                    panel_Sint = z;
                    panel_Sint.Show();
                    panel_Sint.Owner = this;
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
                    Инициализация z = new Инициализация(this);
                    panel_Init = z;
                    panel_Init.Show();
                    panel_Init.Owner = this;
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
                    Приёмник z = new Приёмник(this);
                    panel_Recv = z;
                    panel_Recv.Show();
                    panel_Recv.Owner = this; //это надо чтобы модальное окно могло обращаться к методам родительсокго окна!!!
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
                    Калибровка z = new Калибровка(this);
                    panel_Cal = z;
                    panel_Cal.Show();
                    panel_Cal.Owner = this;
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
                    Consol z = new Consol(this);
                    panel_Cons = z;
                    panel_Cons.Show();
                    panel_Cons.Owner = this;
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
            //команда запрос статуса
            byte[] a = new byte[4];
            Log.Write("Запрашиваем статус!");
            try
            {
                udp0_sender.UDP_SEND
                       (
                       UDP_sender.CMD.CMD_STATUS, //команда 
                       a,   //данные
                       4,   //число данных в байтах
                       0    //время исполнения , 0 - значит немедленно как сможешь.
                       );                
            }
            catch
            {
                Console.WriteLine("Проблема с UDP!");
                        Log.Write("Проблема с UDP!");
            }
            

        }

        private void button_SYSTIME_SETUP_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Посылаем текущее время");
        }
    }
}
