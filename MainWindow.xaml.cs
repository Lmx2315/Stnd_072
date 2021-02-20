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
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Stnd_072
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int POROG_TIMER=6;    //верхняя планка безответных интервалов

        Тестирование  panel_Test = null;
        Синтезатор    panel_Sint = null;
        Приёмник      panel_Recv = null;
        Калибровка    panel_Cal  = null;
        Инициализация panel_Init = null;
        Consol        panel_Cons = null;
        DAC_panel     panel_DAC0 = null;
        DAC_panel     panel_DAC1 = null;
        ADC_panel     panel_ADC0 = null;
        ADC_panel     panel_ADC1 = null;
        FPGA_panel    panel_FPGA = null;
        BOARD_panel   panel_BOARD = null;

        BRD_state BRD_st = new BRD_state();

        DDS_code dds_code   = null;
        public List<DDS_code> list = null;

        public UDP_server udp0 ;
        public UDP_sender udp0_sender ;
        public UDP_DATA_SERVER sDATA0;
        public UDP_DATA_SERVER sDATA1;

        static int BUF_SIZE = 131072;          //это размер максимального БПФ*4 (т.е. в байтах)
        static int FFT_SIZE_MAX = BUF_SIZE / 4;//максимальный размер БПФ
                
        int FLAG_SINT_INIT = 0;
        public uint FFT_SIZE    = 8192;//размер БПФ
        public int FLAG_DISPAY0 = 0;//флаг показывает возможность вывода на экран - CH0
        public int FLAG_DISPAY1 = 0;//- CH1
        public int FLAG_DISPAY2 = 0;//- CH2
        public int FLAG_DISPAY3 = 0;//- CH3

        public string selectedWindowName="Hann";//название сглаживающего окна для БПФ
        public int FILTR_SMOOTH = 0; //тип фильтра сглаживания выходных данных отображения

        Config cfg = new Config();//тут храним конфигурацию , будем брать её из файла

        public STATUS_b072 b072 = new STATUS_b072();//структура хранит состояния составляющих кассеты 072
        /*
        DAC_status DAC0  = new DAC_status();
        DAC_status DAC1  = new DAC_status();
        ADC_status ADC0  = new ADC_status();
        ADC_status ADC1  = new ADC_status();
        LMK_status LMK   = new LMK_status();
        BOARD_status BRD = new BOARD_status();*/

        STRUCT_FFT_DATA data0;
        STRUCT_FFT_DATA data1;
        STRUCT_FFT_DATA data2;
        STRUCT_FFT_DATA data3;

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer Timer2 = new System.Windows.Threading.DispatcherTimer();
        private void Timer1_Tick(object sender, EventArgs e)//быстрое перывание - отвечает за обновление данных для вывода на экран
        {

            if (panel_Recv != null)
            {
                FFT_SIZE = panel_Recv.FFT_SIZE;                    //тут мы получаем размер БПФ
                selectedWindowName = panel_Recv.selectedWindowName;//тут мы получаем тип окна БПФ
                FILTR_SMOOTH = panel_Recv.FLAG_SMOOTH_FILTR;       //тут мы получаем тип сглаживающего фильтра перед выводом на экран
                panel_Recv.data0 = data0;
                panel_Recv.data1 = data1;
                panel_Recv.data2 = data2;
                panel_Recv.data3 = data3;
            }

            if (sDATA0 != null)
            {
                sDATA0.FFT_SIZE   = FFT_SIZE;//передаём размер БПФ для обработки
                sDATA0.FLAG_filtr = FILTR_SMOOTH;
                sDATA0.selectedWindowName = selectedWindowName;
                FLAG_DISPAY0 = sDATA0.FLAG_DISPAY0;
                FLAG_DISPAY1 = sDATA0.FLAG_DISPAY1;
                data0 = sDATA0.data0;
                data1 = sDATA0.data1;
            }

            if (sDATA1 != null)
            {
                sDATA1.FFT_SIZE = FFT_SIZE;//передаём размер БПФ для обработки
                sDATA1.FLAG_filtr = FILTR_SMOOTH;
                sDATA1.selectedWindowName = selectedWindowName;
                FLAG_DISPAY2 = sDATA1.FLAG_DISPAY0;
                FLAG_DISPAY3 = sDATA1.FLAG_DISPAY1;
                data2 = sDATA1.data0;
                data3 = sDATA1.data1;
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)//медленное прерывание - отвечает за обмен с поделкой
        {
            //Console.WriteLine("Инициализация.init:"+Инициализация.init);
            if (Инициализация.init == false) Панель_инициализации.IsChecked = false;
            if (Синтезатор.init == false) Панель_синтезатора.IsChecked = false;
            if (Приёмник.init == false) Панель_приёмника.IsChecked = false;
            if (Калибровка.init == false) Панель_калибровки.IsChecked = false;
            if (Consol.init == false) Панель_консоли.IsChecked = false;

            if (FLAG_SINT_INIT == 0)
            {
                FLAG_SINT_INIT = 1;
                Панель_синтезатора.IsChecked = true;
                mnuSint_Click(Панель_синтезатора, null);
            }
            CMD_REAL_TIME_SEND();
            SYS_CONTROL();

            if (panel_Recv != null)
            {
                FFT_SIZE = panel_Recv.FFT_SIZE;                    //тут мы получаем размер БПФ
                selectedWindowName = panel_Recv.selectedWindowName;//тут мы получаем тип окна БПФ
                FILTR_SMOOTH = panel_Recv.FLAG_SMOOTH_FILTR;       //тут мы получаем тип сглаживающего фильтра перед выводом на экран
                panel_Recv.data0 = data0;
                panel_Recv.data1 = data1;
                panel_Recv.data2 = data2;
                panel_Recv.data3 = data3;
            }

            if (sDATA0 != null)
            {
                sDATA0.FFT_SIZE = FFT_SIZE;//передаём размер БПФ для обработки
                sDATA0.FLAG_filtr = FILTR_SMOOTH;
                sDATA0.selectedWindowName = selectedWindowName;
                FLAG_DISPAY0 = sDATA0.FLAG_DISPAY0;
                FLAG_DISPAY1 = sDATA0.FLAG_DISPAY1;
                data0 = sDATA0.data0;
                data1 = sDATA0.data1;
            }

            if (sDATA1 != null)
            {
                sDATA1.FFT_SIZE = FFT_SIZE;//передаём размер БПФ для обработки
                sDATA1.FLAG_filtr = FILTR_SMOOTH;
                sDATA1.selectedWindowName = selectedWindowName;
                FLAG_DISPAY2 = sDATA1.FLAG_DISPAY0;
                FLAG_DISPAY3 = sDATA1.FLAG_DISPAY1;
                data2 = sDATA1.data0;
                data3 = sDATA1.data1;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0,10);
            Timer1.Start();//запускаю таймер 

            Timer2.Tick += new EventHandler(Timer2_Tick);
            Timer2.Interval = new TimeSpan(0, 0, 0, 0,500);
            Timer2.Start();//запускаю таймер 
            
            CFG_load();
            b072.DAC0 = new DAC_status();
            b072.DAC1 = new DAC_status();
            b072.ADC0 = new ADC_status();
            b072.ADC1 = new ADC_status();
            b072.LMK  = new LMK_status();
            b072.BRD  = new BOARD_status();
            b072.FPGA = new FPGA_status();
        }

        private void button_SYS_START_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Инициализируем сервер");
            UDP_server z = new UDP_server(cfg.my_IP, cfg.my_PORT,cfg,b072);
            udp0 = z;
            UDP_sender x = new UDP_sender(cfg.dst_IP, cfg.dst_PORT);
            udp0_sender = x;

            UDP_DATA_SERVER y = new UDP_DATA_SERVER(this,cfg.DATA0_IP, cfg.DATA0_PORT);// 
            sDATA0 = y;            
            UDP_DATA_SERVER q = new UDP_DATA_SERVER(this,cfg.DATA1_IP, cfg.DATA1_PORT);
            sDATA1 = q;
            
        }
        //--------------------обработка создания панелей--------------------------
        private void button_DAC0_Click(object sender, RoutedEventArgs e)
        {
            if (DAC_panel.DAC0 == false)
            {
                Console.WriteLine("Создаём панель");
                DAC_panel z = new DAC_panel(this,"DAC0");
                panel_DAC0 = z;
                panel_DAC0.Left = 600;//положение левого края панели относительно рабочего стола
                panel_DAC0.Top = 100; //положение верхнего края панели относительно рабочего стола
                panel_DAC0.Show();
                panel_DAC0.Owner = this;
            }
        }
        private void button_DAC1_Click(object sender, RoutedEventArgs e)
        {
            if (DAC_panel.DAC1 == false)
            {
                Console.WriteLine("Создаём панель");
                DAC_panel z = new DAC_panel(this, "DAC1");
                panel_DAC1 = z;
                panel_DAC1.Left = 600;//положение левого края панели относительно рабочего стола
                panel_DAC1.Top = 100; //положение верхнего края панели относительно рабочего стола
                panel_DAC1.Show();
                panel_DAC1.Owner = this;
            }
        }

        private void button_ADC0_Click(object sender, RoutedEventArgs e)
        {
            if (ADC_panel.ADC0 == false)
            {
                Console.WriteLine("Создаём панель");
                ADC_panel z = new ADC_panel(this, "ADC0");
                panel_ADC0 = z;
                panel_ADC0.Left = 600;//положение левого края панели относительно рабочего стола
                panel_ADC0.Top = 100; //положение верхнего края панели относительно рабочего стола
                panel_ADC0.Show();
                panel_ADC0.Owner = this;
            }
        }

        private void button_ADC1_Click(object sender, RoutedEventArgs e)
        {
            if (ADC_panel.ADC1 == false)
            {
                Console.WriteLine("Создаём панель");
                ADC_panel z = new ADC_panel(this, "ADC1");
                panel_ADC1 = z;
                panel_ADC1.Left = 600;//положение левого края панели относительно рабочего стола
                panel_ADC1.Top = 100; //положение верхнего края панели относительно рабочего стола
                panel_ADC1.Show();
                panel_ADC1.Owner = this;
            }
        }

        private void button_FPGA_Click(object sender, RoutedEventArgs e)
        {
            if (FPGA_panel.FPGA == false)
            {
                Console.WriteLine("Создаём панель");
                FPGA_panel z = new FPGA_panel(this, "FPGA");
                panel_FPGA = z;
                panel_FPGA.Left = 600;//положение левого края панели относительно рабочего стола
                panel_FPGA.Top = 100; //положение верхнего края панели относительно рабочего стола
                panel_FPGA.Show();
                panel_FPGA.Owner = this;
            }
        }

        private void button_BOARD_Click(object sender, RoutedEventArgs e)
        {
            if (BOARD_panel.BOARD == false)
            {
                Console.WriteLine("Создаём панель");
                BOARD_panel z = new BOARD_panel(this, "BOARD");
                panel_BOARD = z;
                panel_BOARD.Left = 600;//положение левого края панели относительно рабочего стола
                panel_BOARD.Top = 100; //положение верхнего края панели относительно рабочего стола
                panel_BOARD.Show();
                panel_BOARD.Owner = this;
            }
        }

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
                    panel_Sint.Left = 500;//положение левого края панели относительно рабочего стола
                    panel_Sint.Top  = 0; //положение верхнего края панели относительно рабочего стола
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
            //команда старт!
            byte[] a = new byte[4];
            Log.Write("Запрашиваем статус!");
            try
            {
                udp0_sender.UDP_SEND
                       (
                       UDP_sender.CMD.CMD_START, //команда 
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

        private void button_calibr_ch_OK_Click(object sender, RoutedEventArgs e)
        {
            //команда старт!
            byte[] a = new byte[4];
            byte ch1, ch2, ch3, ch4, ch1_off, ch2_off, ch3_off, ch4_off;

            ch1 = Convert.ToByte(checkBox_ch1.IsChecked);
            ch2 = Convert.ToByte(checkBox_ch2.IsChecked);
            ch3 = Convert.ToByte(checkBox_ch3.IsChecked);
            ch4 = Convert.ToByte(checkBox_ch4.IsChecked);

            ch1_off = (byte)((ch1) & 1);
            ch2_off = (byte)((ch2) & 1);
            ch3_off = (byte)((ch3) & 1);
            ch4_off = (byte)((ch4) & 1);

            a[3] = (byte)((ch1 << 7) + (ch2 << 6) + (ch3 << 5) + (ch4 << 4) + (ch1_off << 3) + (ch2_off << 2) + (ch3_off << 1) + (ch4_off << 0));

            Log.Write("Переключаем каналы с прямого выхода на калибровку и обратно!");
            try
            {
                Console.WriteLine("Отсылаем команду переключения каналов!");
                udp0_sender.UDP_SEND
                       (
                       UDP_sender.CMD.CMD_CHANNEL, //команда 
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

        private void button_ATT_Click(object sender, RoutedEventArgs e)
        {

            BRD_st.Att0 = Convert.ToInt16(textBox_ATT0.Text);
            BRD_st.Att1 = Convert.ToInt16(textBox_ATT1.Text);
            BRD_st.Att2 = Convert.ToInt16(textBox_ATT2.Text);
            BRD_st.Att3 = Convert.ToInt16(textBox_ATT3.Text);

            Log.Write("управляем аттенюаторами!");
            try
            {
                Console.WriteLine("Отсылаем команду управления аттенюаторами!");
                udp0_sender.UDP_SEND
                       (
                       UDP_sender.CMD.CMD_ATT,      //команда 
                       BRD_st.ATT_TDATA(),          //данные
                (uint) BRD_st.ATT_TDATA().Length,   //число данных в байтах
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

            //
            byte[] a = new byte[8];
            UInt64 time =Convert.ToUInt64(textBox_SYSTIME.Text);

            a[0] = Convert.ToByte(time >> 56);
            a[1] = Convert.ToByte(time >> 48);
            a[2] = Convert.ToByte(time >> 40);
            a[3] = Convert.ToByte(time >> 32);
            a[4] = Convert.ToByte(time >> 24);
            a[5] = Convert.ToByte(time >> 16);
            a[6] = Convert.ToByte(time >>  8);
            a[7] = Convert.ToByte(time >>  0);

            try
            {
                udp0_sender.UDP_SEND
                       (
                       UDP_sender.CMD.CMD_TIME_SETUP, //команда 
                       a,   //данные
                       8,   //число данных в байтах
                       0    //время исполнения , 0 - значит немедленно как сможешь.
                       );
            }
            catch
            {
                Console.WriteLine("Проблема с UDP!");
                Log.Write("Проблема с UDP!");
            }
        }

        private void mnuConfig_Click(object sender, RoutedEventArgs e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            path = System.IO.Path.GetDirectoryName(path);
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "dat|*.dat";
            of.Title = "Load config.dat";
            of.InitialDirectory = path;
            of.ShowDialog();

            if (of.CheckFileExists == true)
            {
                // получаем выбранный файл
                string filename = of.FileName;
         //       Console.WriteLine("of.CheckFileExists == true");
                try
                {
                    XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(Config));
                    FileStream fr = new FileStream(filename, FileMode.Open);
                    cfg = (Config)xmlSerialaizer.Deserialize(fr);
                    Log.Write($"Загрузили конфигурационный файл \n server IP:{cfg.my_IP} server PORT:{cfg.my_PORT} \n dst IP:{cfg.dst_IP} dst PORT:{cfg.dst_PORT}");
                    fr.Close();
                }
                catch
                {

                }
            }
        }

        private bool CFG_load ()
        {
            bool error = false;
            string path = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            path = System.IO.Path.GetDirectoryName(path);

                // получаем выбранный файл
                string filename = "cfg.dat";
                try
                {
                    XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(Config));
                    FileStream fr = new FileStream(filename, FileMode.Open);
                    cfg = (Config)xmlSerialaizer.Deserialize(fr);
                    Log.Write($"Загрузили конфигурационный файл \n server IP:{cfg.my_IP} server PORT:{cfg.my_PORT} \n dst IP:{cfg.dst_IP} dst PORT:{cfg.dst_PORT}");
                    fr.Close();
                }
                catch
                {
                    Debug.WriteLine("нет файла cfg!");
                 }

            return error;
        }

        void CMD_REAL_TIME_SEND()
        {
            byte[] a;
            int i = 0;
            uint n = 0;

            if (list!=null)
            {
                Console.WriteLine("список не пустой!");
                
                i=list.Count;
                
                while (i>0)
                {
                    i--;
                    a=ARRAY_FILL(list[i]);
                    n = (uint)a.Length;
                    Debug.WriteLine("n:"+n);
                    Log.Write("Отсылаем команду реального времени!");

                    try 
                    {
                        udp0_sender.UDP_SEND
                       (
                        UDP_sender.CMD.CMD_REALTIME, //команда реального времени, задаёт интервалы излучения и приёма
                        a,                           //данные
                        n,                           //число данных в байтах
                        list[i].zTIME                //время исполнения , 0 - значит немедленно как сможешь.
                        );
                    }
                    catch
                    {
                        MessageBox.Show("Нет абонента!");
                    }
                    
                           
                }

                list.Clear();
                list = null;
            }

        }

        byte [] ARRAY_FILL (DDS_code l) //заполняет траспортный массив данными из структуры
        {
            byte[] m = new byte[51];
            int i = 0;

            Console.WriteLine("zTIME        :" + l.zTIME);
            Console.WriteLine("zFREQ        :" + l.zFREQ);
            Console.WriteLine("zFREQ_STEP   :" + l.zFREQ_STEP);
            Console.WriteLine("zFREQ_RATE   :" + l.zFREQ_RATE);
            Console.WriteLine("zN_impulse   :" + l.zN_impulse);
            Console.WriteLine("zTYPE_impulse:" + l.zTYPE_impulse);
            Console.WriteLine("zInterval_Ti :" + l.zInterval_Ti);
            Console.WriteLine("zInterval_Tp :" + l.zInterval_Tp);
            Console.WriteLine("zTblank1     :" + l.zTblank1);
            Console.WriteLine("zTblank2     :" + l.zTblank2);

            m[i++] = (byte)(l.zFREQ >> 40);
            m[i++] = (byte)(l.zFREQ >> 32);
            m[i++] = (byte)(l.zFREQ >> 24);
            m[i++] = (byte)(l.zFREQ >> 16);
            m[i++] = (byte)(l.zFREQ >> 8);
            m[i++] = (byte)(l.zFREQ >> 0);//5

            m[i++] = (byte)(l.zFREQ_STEP >> 40);
            m[i++] = (byte)(l.zFREQ_STEP >> 32);
            m[i++] = (byte)(l.zFREQ_STEP >> 24);
            m[i++] = (byte)(l.zFREQ_STEP >> 16);
            m[i++] = (byte)(l.zFREQ_STEP >> 8);
            m[i++] = (byte)(l.zFREQ_STEP >> 0);//11

            m[i++] = (byte)(l.zFREQ_RATE >> 24);
            m[i++] = (byte)(l.zFREQ_RATE >> 16);
            m[i++] = (byte)(l.zFREQ_RATE >> 8);
            m[i++] = (byte)(l.zFREQ_RATE >> 0);//15

            m[i++] = (byte)(l.zN_impulse >> 8);
            m[i++] = (byte)(l.zN_impulse >> 0);

            m[i++] = (byte)(l.zTYPE_impulse);//18 

            m[i++] = (byte)(l.zInterval_Ti >> 24);
            m[i++] = (byte)(l.zInterval_Ti >> 16);
            m[i++] = (byte)(l.zInterval_Ti >> 8);
            m[i++] = (byte)(l.zInterval_Ti >> 0);//22

            m[i++] = (byte)(l.zInterval_Tp >> 24);
            m[i++] = (byte)(l.zInterval_Tp >> 16);
            m[i++] = (byte)(l.zInterval_Tp >> 8);
            m[i++] = (byte)(l.zInterval_Tp >> 0);//26

            m[i++] = (byte)(l.zTblank1 >> 24);
            m[i++] = (byte)(l.zTblank1 >> 16);
            m[i++] = (byte)(l.zTblank1 >> 8);
            m[i++] = (byte)(l.zTblank1 >> 0);//30

            m[i++] = (byte)(l.zTblank2 >> 24);
            m[i++] = (byte)(l.zTblank2 >> 16);
            m[i++] = (byte)(l.zTblank2 >> 8);
            m[i++] = (byte)(l.zTblank2 >> 0);//34 

            m[i++] = (byte)(l.zAmp0 >> 8);//35
            m[i++] = (byte)(l.zAmp0 >> 0);
            m[i++] = (byte)(l.zAmp1 >> 8);
            m[i++] = (byte)(l.zAmp1 >> 0);
            m[i++] = (byte)(l.zAmp2 >> 8);
            m[i++] = (byte)(l.zAmp2 >> 0);
            m[i++] = (byte)(l.zAmp3 >> 8);
            m[i++] = (byte)(l.zAmp3 >> 0);//42

            m[i++] = (byte)(l.zPhase0 >> 8);
            m[i++] = (byte)(l.zPhase0 >> 0);
            m[i++] = (byte)(l.zPhase1 >> 8);
            m[i++] = (byte)(l.zPhase1 >> 0);
            m[i++] = (byte)(l.zPhase2 >> 8);
            m[i++] = (byte)(l.zPhase2 >> 0);
            m[i++] = (byte)(l.zPhase3 >> 8);
            m[i++] = (byte)(l.zPhase3 >> 0);//50 (количество 50 + 1) Обязательно менять размер массива вверху!!

            return m;
        }

        private void SYS_CONTROL ()
        {
            int error = 0;
            string msg="";
            STATUS_b072.timer_obmen++;

            if (STATUS_b072.timer_obmen > POROG_TIMER)
            {
                b072.ADC0.INIT = 0;
                b072.ADC1.INIT = 0;
                b072.DAC0.INIT = 0;
                b072.DAC1.INIT = 0;
                b072.FPGA.INIT = 0;
                b072.BRD.INIT  = 0;
                b072.LMK.INIT  = 0;
            }       
          
            if (b072.ADC0.INIT == 1)
            {
                if((b072.ADC0.align_ok_adc != 0x0A)&&
                   (b072.ADC0.align_ok_adc != 0x05))     { error++; msg += "align_ok_adc     :" + b072.ADC0.align_ok_adc.ToString("X") + "\r"; }
                if (b072.ADC0.rx_ready_adc != 1)         { error++; msg += "rx_ready_adc     :" + b072.ADC0.rx_ready_adc.ToString("X") + "\r"; }
                if (b072.ADC0.sync_n_adc   != 1)         { error++; msg += "sync_n_adc       :" + b072.ADC0.sync_n_adc.ToString("X") + "\r"; }
                if (b072.ADC0.rx_syncstatus_adc != 0xFF) { error++; msg += "rx_syncstatus_adc:" + b072.ADC0.rx_syncstatus_adc.ToString("X") + "\r"; }
        //      if (b072.ADC0.align_adc != 0x55) { /*error++;*/ msg += "align_adc        :" + b072.ADC0.align_adc.ToString("X") + "\r"; }

                b072.ADC0.ERROR = error;
                b072.ADC0.MSG = msg;

                if  (error==0) button_ADC0.Background = new LinearGradientBrush(Colors.White, Colors.LightGreen, 90);
                else           button_ADC0.Background = new LinearGradientBrush(Colors.White, Colors.Red, 90);

                error = 0;
                msg = "";
            }
            else button_ADC0.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);

            if (b072.ADC1.INIT == 1)
            {
                if((b072.ADC1.align_ok_adc      != 0x0A)&&
                   (b072.ADC1.align_ok_adc      != 0x05)){ error++; msg += "align_ok_adc     :" + b072.ADC1.align_ok_adc.ToString("X") + "\r"; }
                if (b072.ADC1.rx_ready_adc      !=    1) { error++; msg += "rx_ready_adc     :" + b072.ADC1.rx_ready_adc.ToString("X") + "\r"; }
                if (b072.ADC1.sync_n_adc        !=    1) { error++; msg += "sync_n_adc       :" + b072.ADC1.sync_n_adc.ToString("X") + "\r"; }
                if (b072.ADC1.rx_syncstatus_adc != 0xFF) { error++; msg += "rx_syncstatus_adc:" + b072.ADC1.rx_syncstatus_adc.ToString("X") + "\r"; }
      //        if (b072.ADC1.align_adc         != 0x55) {/* error++; */msg += "align_adc        :" + b072.ADC1.align_adc.ToString("X") + "\r"; }
                
                b072.ADC1.ERROR = error;
                b072.ADC1.MSG = msg;

                if (error == 0) button_ADC1.Background = new LinearGradientBrush(Colors.White, Colors.LightGreen, 90);
                else            button_ADC1.Background = new LinearGradientBrush(Colors.White, Colors.Red, 90);

                error = 0;
                msg = "";
            }
            else button_ADC1.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);

            if (b072.DAC0.INIT==1)
            {
                if  (b072.DAC0.dac_pll_locked    != 1)   { error++; msg += "dac_pll_locked    :" + b072.DAC0.dac_pll_locked.ToString() + "\r"; }
                if ((b072.DAC0.memin_pll_lfvolt   < 3)||
                    (b072.DAC0.memin_pll_lfvolt   > 4))  { error++; msg += "memin_pll_lfvolt   :" + b072.DAC0.memin_pll_lfvolt.ToString() + "\r"; }
                if  (b072.DAC0.alarms_from_lanes  > 0)   { error++; msg += "alarms_from_lanes  :" + b072.DAC0.alarms_from_lanes.ToString() + "\r"; }
                if  (b072.DAC0.alarm_fifo_flags_0 > 0)   { error++; msg += "alarm_fifo_flags_0 :" + b072.DAC0.alarm_fifo_flags_0.ToString() + "\r"; }
                if  (b072.DAC0.alarm_fifo_flags_1 > 0)   { error++; msg += "alarm_fifo_flags_1 :" + b072.DAC0.alarm_fifo_flags_1.ToString() + "\r"; }
                if  (b072.DAC0.ALARM_ERROR        > 0)   { error++; msg += "ALARM_ERROR        :" + b072.DAC0.ALARM_ERROR.ToString() + "\r"; }
                if  (b072.DAC0.SYNC_N_ERROR       > 0)   { error++; msg += "SYNC_N_ERROR       :" + b072.DAC0.SYNC_N_ERROR.ToString() + "\r"; }
                if  (b072.DAC0.alarm_l_error_0    > 0)   { error++; msg += "alarm_l_error_0    :" + b072.DAC0.alarm_l_error_0.ToString() + "\r"; }
                if  (b072.DAC0.alarm_l_error_1    > 0)   { error++; msg += "alarm_l_error_1    :" + b072.DAC0.alarm_l_error_1.ToString() + "\r"; }
                if  (b072.DAC0.error_count_link0  > 0)   { error++; msg += "error_count_link0  :" + b072.DAC0.error_count_link0.ToString() + "\r"; }
                if  (b072.DAC0.error_count_link1  > 0)   { error++; msg += "error_count_link1  :" + b072.DAC0.error_count_link1.ToString() + "\r"; }
                if  (b072.DAC0.TEMP > 60) { error++; msg += "TEMP  :" + b072.DAC0.TEMP.ToString() + "\r"; }

                b072.DAC0.ERROR = error;
                b072.DAC0.MSG = msg;

                if (error == 0) button_DAC0.Background = new LinearGradientBrush(Colors.White, Colors.LightGreen, 90);
                else            button_DAC0.Background = new LinearGradientBrush(Colors.White, Colors.Red, 90);

                error = 0;
                msg = "";
            }
            else button_DAC0.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);

            if (b072.DAC1.INIT == 1)
            {
                if  (b072.DAC1.dac_pll_locked    != 1) { error++; msg += "dac_pll_locked     :" + b072.DAC1.dac_pll_locked.ToString() + "\r"; }
                if ((b072.DAC1.memin_pll_lfvolt   < 3) ||
                    (b072.DAC1.memin_pll_lfvolt   > 4)) { error++; msg += "memin_pll_lfvolt   :" + b072.DAC1.memin_pll_lfvolt.ToString() + "\r"; }
                if  (b072.DAC1.alarms_from_lanes  > 0)  { error++; msg += "alarms_from_lanes  :" + b072.DAC1.alarms_from_lanes.ToString() + "\r"; }
                if  (b072.DAC1.alarm_fifo_flags_0 > 0)  { error++; msg += "alarm_fifo_flags_0 :" + b072.DAC1.alarm_fifo_flags_0.ToString() + "\r"; }
                if  (b072.DAC1.alarm_fifo_flags_1 > 0)  { error++; msg += "alarm_fifo_flags_1 :" + b072.DAC1.alarm_fifo_flags_1.ToString() + "\r"; }
                if  (b072.DAC1.ALARM_ERROR        > 0)  { error++; msg += "ALARM_ERROR        :" + b072.DAC1.ALARM_ERROR.ToString() + "\r"; }
                if  (b072.DAC1.SYNC_N_ERROR       > 0)  { error++; msg += "SYNC_N_ERROR       :" + b072.DAC1.SYNC_N_ERROR.ToString() + "\r"; }
                if  (b072.DAC1.alarm_l_error_0    > 0)  { error++; msg += "alarm_l_error_0    :" + b072.DAC1.alarm_l_error_0.ToString() + "\r"; }
                if  (b072.DAC1.alarm_l_error_1    > 0)  { error++; msg += "alarm_l_error_1    :" + b072.DAC1.alarm_l_error_1.ToString() + "\r"; }
                if  (b072.DAC1.error_count_link0  > 0)  { error++; msg += "error_count_link0  :" + b072.DAC1.error_count_link0.ToString() + "\r"; }
                if  (b072.DAC1.error_count_link1  > 0)  { error++; msg += "error_count_link1  :" + b072.DAC1.error_count_link1.ToString() + "\r"; }
                if  (b072.DAC1.TEMP               > 60) { error++; msg += "TEMP  :" + b072.DAC1.TEMP.ToString() + "\r"; }

                b072.DAC1.ERROR = error;
                b072.DAC1.MSG = msg;

                if (error == 0) button_DAC1.Background = new LinearGradientBrush(Colors.White, Colors.LightGreen, 90);
                else            button_DAC1.Background = new LinearGradientBrush(Colors.White, Colors.Red, 90);

                error = 0;
                msg = "";
            }
            else button_DAC1.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);

            if (b072.FPGA.INIT == 1)
            {
                if (b072.FPGA.STATUS_REF != 1) { error++; msg += "STATUS_REF     :" + b072.FPGA.STATUS_REF.ToString() + "\r"; }
                if (b072.FPGA.TEMP        >60) { error++; msg += "TEMP           :" + b072.FPGA.TEMP.ToString() + "\r"; }

                b072.FPGA.ERROR = error;
                b072.FPGA.MSG = msg;

                if (error == 0) button_FPGA.Background = new LinearGradientBrush(Colors.White, Colors.LightGreen, 90);
                else button_FPGA.Background = new LinearGradientBrush(Colors.White, Colors.Red, 90);

                error = 0;
                msg = "";
            }
            else button_FPGA.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);

            if (b072.BRD.INIT == 1)
            {
                if (true)                {        ; msg += "BRD_NUMBER     :" + b072.BRD.BRD_NUMBER.ToString() + "\r"; }
                if (b072.BRD.TEMP0 > 60) { error++; msg += "TEMP0          :" + b072.BRD.TEMP0.ToString() + "\r"; }
                if (b072.BRD.TEMP1 > 60) { error++; msg += "TEMP1          :" + b072.BRD.TEMP1.ToString() + "\r"; }

                b072.BRD.ERROR = error;
                b072.BRD.MSG = msg;

                if (error == 0) button_BOARD.Background = new LinearGradientBrush(Colors.White, Colors.LightGreen, 90);
                else button_BOARD.Background = new LinearGradientBrush(Colors.White, Colors.Red, 90);

                error = 0;
                msg = "";
            }
            else button_BOARD.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);



            //шлём команду для поддержания линии связи в зелёном состоянии
            if (udp0!=null)
            {
                if (udp0.FLAG_MSG_RCV)
                {
                    byte[] a = new byte[4];

                    udp0_sender.UDP_SEND
                           (
                           UDP_sender.CMD.CMD_STATUS, //команда 
                           a,   //данные
                           4,   //число данных в байтах
                           0    //время исполнения , 0 - значит немедленно как сможешь.
                           );
                }
            }
            
        }

        private void checkBox_ch1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void mnuTest_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.IsChecked)
            {
                if (Тестирование.init == false)
                {
                    Console.WriteLine("Создаём панель тестирования");
                    Тестирование z = new Тестирование(this);
                    panel_Test = z;
                    panel_Test.Show();
                    panel_Test.Owner = this;
                }
            }
            else
            {
                if (Инициализация.init == true)
                {
                    Console.WriteLine("удаляем панель тестирования");
                    Тестирование.init = false;
                    panel_Test.Close();
                }
            }
        }
    }
}
