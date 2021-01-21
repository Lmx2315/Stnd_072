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

        DDS_code dds_code   = null;
        public List<DDS_code> list = null;

        public UDP_server udp0 ;
        public UDP_sender udp0_sender ;
        int FLAG_SINT_INIT = 0;

        Config cfg = new Config();//тут храним конфигурацию , будем брать её из файла

        public STATUS_b072 b072 = new STATUS_b072();//структура хранит состояния составляющих кассеты 072
        /*
        DAC_status DAC0  = new DAC_status();
        DAC_status DAC1  = new DAC_status();
        ADC_status ADC0  = new ADC_status();
        ADC_status ADC1  = new ADC_status();
        LMK_status LMK   = new LMK_status();
        BOARD_status BRD = new BOARD_status();*/

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        private void Timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("Инициализация.init:"+Инициализация.init);
            if (Инициализация.init == false) Панель_инициализации.IsChecked = false;
            if (Синтезатор.init    == false) Панель_синтезатора.IsChecked   = false;
            if (Приёмник.init      == false) Панель_приёмника.IsChecked     = false;
            if (Калибровка.init    == false) Панель_калибровки.IsChecked    = false;
            if (Consol.init        == false) Панель_консоли.IsChecked       = false;
            
            if (FLAG_SINT_INIT==0)
            {
                FLAG_SINT_INIT = 1;
                Панель_синтезатора.IsChecked = true;                
                mnuSint_Click(Панель_синтезатора, null);
            }
            CMD_REAL_TIME_SEND();
            SYS_CONTROL();
        }
        public MainWindow()
        {
            InitializeComponent();
            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 250);
            Timer1.Start();//запускаю таймер 
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
            if (list!=null)
            {
                Console.WriteLine("список не пустой!");
                list = null;
            }
        }


        private void SYS_CONTROL ()
        {
            int error = 0;
            string msg="";
 
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
                    (b072.DAC1.memin_pll_lfvolt   > 4)){ error++; msg += "memin_pll_lfvolt   :" + b072.DAC1.memin_pll_lfvolt.ToString() + "\r"; }
                if  (b072.DAC1.alarms_from_lanes  > 0) { error++; msg += "alarms_from_lanes  :" + b072.DAC1.alarms_from_lanes.ToString() + "\r"; }
                if  (b072.DAC1.alarm_fifo_flags_0 > 0) { error++; msg += "alarm_fifo_flags_0 :" + b072.DAC1.alarm_fifo_flags_0.ToString() + "\r"; }
                if  (b072.DAC1.alarm_fifo_flags_1 > 0) { error++; msg += "alarm_fifo_flags_1 :" + b072.DAC1.alarm_fifo_flags_1.ToString() + "\r"; }
                if  (b072.DAC1.ALARM_ERROR        > 0) { error++; msg += "ALARM_ERROR        :" + b072.DAC1.ALARM_ERROR.ToString() + "\r"; }
                if  (b072.DAC1.SYNC_N_ERROR       > 0) { error++; msg += "SYNC_N_ERROR       :" + b072.DAC1.SYNC_N_ERROR.ToString() + "\r"; }
                if  (b072.DAC1.alarm_l_error_0    > 0) { error++; msg += "alarm_l_error_0    :" + b072.DAC1.alarm_l_error_0.ToString() + "\r"; }
                if  (b072.DAC1.alarm_l_error_1    > 0) { error++; msg += "alarm_l_error_1    :" + b072.DAC1.alarm_l_error_1.ToString() + "\r"; }
                if  (b072.DAC1.error_count_link0  > 0) { error++; msg += "error_count_link0  :" + b072.DAC1.error_count_link0.ToString() + "\r"; }
                if  (b072.DAC1.error_count_link1  > 0) { error++; msg += "error_count_link1  :" + b072.DAC1.error_count_link1.ToString() + "\r"; }

                b072.DAC1.ERROR = error;
                b072.DAC1.MSG = msg;

                if (error == 0) button_DAC1.Background = new LinearGradientBrush(Colors.White, Colors.LightGreen, 90);
                else            button_DAC1.Background = new LinearGradientBrush(Colors.White, Colors.Red, 90);

                error = 0;
                msg = "";
            }
            else button_DAC1.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);


        }

    }
}
