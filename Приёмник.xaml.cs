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
using System.Windows.Shapes;
using System.ComponentModel; // CancelEventArgs
using PlotWrapper;
using System.Diagnostics;

namespace Stnd_072
{
    /// <summary>
    /// Логика взаимодействия для Приёмник.xaml
    /// </summary>
    public partial class Приёмник : Window
    {
        public static bool init;
        MainWindow main = null;
        public int FLAG_SMOOTH_FILTR=2;//состояние сглаживающего фильтра для спектра
        static int BUF_SIZE = 131072;          //это размер максимального БПФ*4 (т.е. в байтах)
        static int FFT_SIZE_MAX = BUF_SIZE / 4;//максимальный размер БПФ

        Plot3 fig_TIME0 = new Plot3("time", "x", "y");
        Plot  fig_FFT0  = new Plot(90, "FFT (dBV)", "кГц", "Mag (dBV)", "", "", "", "", "");

        Plot3 fig_TIME1 = new Plot3("time", "x", "y");
        Plot fig_FFT1 = new Plot(90, "FFT (dBV)", "кГц", "Mag (dBV)", "", "", "", "", "");

        Plot3 fig_TIME2 = new Plot3("time", "x", "y");
        Plot fig_FFT2 = new Plot(90, "FFT (dBV)", "кГц", "Mag (dBV)", "", "", "", "", "");

        Plot3 fig_TIME3 = new Plot3("time", "x", "y");
        Plot fig_FFT3 = new Plot(90, "FFT (dBV)", "кГц", "Mag (dBV)", "", "", "", "", "");

        public uint FFT_SIZE;
        public string selectedWindowName="HFT248D";//название сглаживающего окна для БПФ

        int FLAG_DISP_FFT0 = 0;
        int FLAG_DISP_FFT1 = 0;
        int FLAG_DISP_FFT2 = 0;
        int FLAG_DISP_FFT3 = 0;

        int FLAG_DISP_TIME0 = 0;
        int FLAG_DISP_TIME1 = 0;
        int FLAG_DISP_TIME2 = 0;
        int FLAG_DISP_TIME3 = 0;

        public STRUCT_FFT_DATA data0;
        public STRUCT_FFT_DATA data1;
        public STRUCT_FFT_DATA data2;
        public STRUCT_FFT_DATA data3;

        BRD_state BRD_st = new BRD_state();

        private void checkBox_SPECTR0_Checked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_FFT0 = 1;
            fig_FFT0.Visible=true;
        }

        private void checkBox_SPECTR0_Unchecked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_FFT0 = 0;
            fig_FFT0.Visible = false;
        }

        private void checkBox_TIME0_Checked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_TIME0 = 1;
        }

        private void checkBox_TIME0_Unchecked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_TIME0 = 0;
        }

        private void checkBox_SPECTR1_Checked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_FFT1 = 1;
            fig_FFT1.Visible = true;
        }

        private void checkBox_SPECTR1_Unchecked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_FFT1 = 0;
            fig_FFT1.Visible = false;
        }

        private void checkBox_TIME1_Checked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_TIME1 = 1;
        }

        private void checkBox_TIME1_Unchecked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_TIME1 = 0;
        }

        private void checkBox_SPECTR2_Checked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_FFT2 = 1;
            fig_FFT2.Visible = true;
        }

        private void checkBox_SPECTR2_Unchecked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_FFT2 = 0;
            fig_FFT2.Visible = false;
        }

        private void checkBox_TIME2_Checked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_TIME2 = 1;
        }

        private void checkBox_TIME2_Unchecked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_TIME2 = 0;
        }

        private void checkBox_SPECTR3_Checked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_FFT3 = 1;
            fig_FFT3.Visible = true;
        }

        private void checkBox_SPECTR3_Unchecked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_FFT3 = 0;
            fig_FFT3.Visible = false;
        }

        private void checkBox_TIME3_Checked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_TIME3 = 1;
        }

        private void checkBox_TIME3_Unchecked(object sender, RoutedEventArgs e)
        {
            FLAG_DISP_TIME3 = 0;
        }

        public Приёмник(MainWindow a)
        {
            InitializeComponent();
            init = true;
            main = a;
            if (a != null)
            {
                
            }
            FFT_SIZE = main.FFT_SIZE;

            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 50);
            Timer1.Start();//запускаю таймер 

            comboBox_winFFT.ItemsSource = Enum.GetNames(typeof(DSPLib.DSP.Window.Type));
            comboBox_winFFT.SelectedItem=1;
            //selectedWindowName = comboBox_winFFT.SelectedValue.ToString();
            //
        }

        ~Приёмник()
        {
            init = false;

            if (fig_FFT0 != null) fig_FFT0.Dispose();
            if (fig_FFT1 != null) fig_FFT1.Dispose();
            if (fig_FFT2 != null) fig_FFT2.Dispose();
            if (fig_FFT3 != null) fig_FFT3.Dispose();
        }

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if ((main.FLAG_DISPAY0==1)&&(FLAG_DISP_FFT0==1))
           {
                DISPLAY_FFT_0();
                main.FLAG_DISPAY0 = 0;
                textBox_SCH_UDP0.Text = data0.SCH_FFT_PKG0.ToString();
                data0.SCH_FFT_PKG0 = 0;
            }

            if ((main.FLAG_DISPAY1 == 1) && (FLAG_DISP_FFT1 == 1))
           {
                DISPLAY_FFT_1();
                main.FLAG_DISPAY1 = 0;
                textBox_SCH_UDP1.Text = data1.SCH_FFT_PKG0.ToString();
                data1.SCH_FFT_PKG0 = 0;
            }

            if ((main.FLAG_DISPAY2 == 1) && (FLAG_DISP_FFT2 == 1))
            {
                DISPLAY_FFT_2();
                main.FLAG_DISPAY2 = 0;
                textBox_SCH_UDP2.Text = data2.SCH_FFT_PKG0.ToString();
                data2.SCH_FFT_PKG0 = 0;
            }

            if ((main.FLAG_DISPAY3 == 1) && (FLAG_DISP_FFT3 == 1))
            {
                DISPLAY_FFT_3();
                main.FLAG_DISPAY3 = 0;
                textBox_SCH_UDP3.Text = data3.SCH_FFT_PKG0.ToString();
                data3.SCH_FFT_PKG0 = 0;
            }
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            init = false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string log_txt;
            BRD_st.Att0 = Convert.ToByte(textBox_Att0.Text);
            BRD_st.Att1 = Convert.ToByte(textBox_Att1.Text);
            BRD_st.Att2 = Convert.ToByte(textBox_Att2.Text);
            BRD_st.Att3 = Convert.ToByte(textBox_Att3.Text);

            log_txt = "Отправляем значения аттенюатора 0=" + BRD_st.Att0.ToString() + ",1=" + BRD_st.Att1.ToString() + ",2=" + BRD_st.Att2.ToString() + ",3=" + BRD_st.Att3.ToString();

            if (main != null)
            {
                Console.WriteLine(log_txt);
                    Log.Write    (log_txt);
                main.udp0_sender.UDP_SEND(UDP_sender.CMD.CMD_ATT, BRD_st.ATT_TDATA(),(byte) BRD_st.ATT_TDATA().Length, 0);
            }
        }

        void DISPLAY_FFT_0()
        {
            // размер БПФ
            double[] TSAMPL_tmp      = new double[FFT_SIZE];
            double[] MAG_LOG_tmp     = new double[FFT_SIZE];
            double[] time_series_tmp = new double[FFT_SIZE];

            if (data0!=null)
            {
                Array.Copy(data0.TSAMPL, TSAMPL_tmp, FFT_SIZE);
                Array.Copy(data0.MAG_LOG, MAG_LOG_tmp, FFT_SIZE);

                //Console.WriteLine("AMAX0:"+ data0.AMAX);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                fig_FFT0.PlotData(TSAMPL_tmp, MAG_LOG_tmp, data0.AMAX, data0.BMAX, data0.CMAX, data0.M1X, data0.M1Y, data0.M2X, data0.M2Y, data0.M3X, data0.M3Y);
                fig_FFT0.Show();
            }

        }

        void DISPLAY_FFT_1()
        {
            // размер БПФ
            double[] TSAMPL_tmp = new double[FFT_SIZE];
            double[] MAG_LOG_tmp = new double[FFT_SIZE];
            double[] time_series_tmp = new double[FFT_SIZE];

            if (data1 != null)
            {
                Array.Copy(data1.TSAMPL, TSAMPL_tmp, FFT_SIZE);
                Array.Copy(data1.MAG_LOG, MAG_LOG_tmp, FFT_SIZE);

                //Console.WriteLine("AMAX0:"+ data0.AMAX);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                fig_FFT1.PlotData(TSAMPL_tmp, MAG_LOG_tmp, data1.AMAX, data1.BMAX, data1.CMAX, data1.M1X, data1.M1Y, data1.M2X, data1.M2Y, data1.M3X, data1.M3Y);
                fig_FFT1.Show();
            }
        }

        void DISPLAY_FFT_2()
        {
            // размер БПФ
            double[] TSAMPL_tmp = new double[FFT_SIZE];
            double[] MAG_LOG_tmp = new double[FFT_SIZE];
            double[] time_series_tmp = new double[FFT_SIZE];

            if (data2 != null)
            {
                Array.Copy(data2.TSAMPL, TSAMPL_tmp, FFT_SIZE);
                Array.Copy(data2.MAG_LOG, MAG_LOG_tmp, FFT_SIZE);

                //Console.WriteLine("AMAX0:"+ data0.AMAX);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                fig_FFT2.PlotData(TSAMPL_tmp, MAG_LOG_tmp, data2.AMAX, data2.BMAX, data2.CMAX, data2.M1X, data2.M1Y, data2.M2X, data2.M2Y, data2.M3X, data2.M3Y);
                fig_FFT2.Show();
            }
        }

        void DISPLAY_FFT_3()
        {
            // размер БПФ
            double[] TSAMPL_tmp = new double[FFT_SIZE];
            double[] MAG_LOG_tmp = new double[FFT_SIZE];
            double[] time_series_tmp = new double[FFT_SIZE];

            if (data3 != null)
            {
                Array.Copy(data3.TSAMPL, TSAMPL_tmp, FFT_SIZE);
                Array.Copy(data3.MAG_LOG, MAG_LOG_tmp, FFT_SIZE);

                //Console.WriteLine("AMAX0:"+ data0.AMAX);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                fig_FFT3.PlotData(TSAMPL_tmp, MAG_LOG_tmp, data3.AMAX, data3.BMAX, data3.CMAX, data3.M1X, data3.M1Y, data3.M2X, data3.M2Y, data3.M3X, data3.M3Y);
                fig_FFT3.Show();
            }
        }

        void DISPLAY_TIME_0()
        {/*
            // размер БПФ
            double[] TSAMPL_tmp = new double[FFT_SIZE];
            double[] MAG_LOG_tmp = new double[FFT_SIZE];
            double[] time_series_tmp = new double[FFT_SIZE];

            Array.Copy(TSAMPL, TSAMPL_tmp, FFT_SIZE);
            Array.Copy(MAG_LOG, MAG_LOG_tmp, FFT_SIZE);
            Array.Copy(time_series, time_series_tmp, FFT_SIZE);
            // Debug.WriteLine("*");
            // Start a Stopwatch
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            fig3.PlotData(TSAMPL_tmp, MAG_LOG_tmp, AMAX, BMAX, CMAX, M1X, M1Y, M2X, M2Y, M3X, M3Y);
            fig3.Show();

            fig2.PlotData(time_series_tmp);
            fig2.Show();
            */
        }

        private void mnu_winFFT_Click(object sender, RoutedEventArgs e)
        {
            selectedWindowName=comboBox_winFFT.SelectedValue.ToString();
        }

        private void mnu_smooth1_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.IsChecked)
            {
                FLAG_SMOOTH_FILTR = 1;
                smooth_2.IsChecked = false;
            }
            else
            {
                if (smooth_2.IsChecked==false) FLAG_SMOOTH_FILTR = 0;
            }
        }

        private void mnu_smooth2_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.IsChecked)
            {
                FLAG_SMOOTH_FILTR = 2;
                smooth_1.IsChecked = false;
            }
            else
            {
                if (smooth_1.IsChecked == false) FLAG_SMOOTH_FILTR = 0;
            }
        }

        private void comboBox_winFFT_Initialized(object sender, EventArgs e)
        {
           // comboBox_winFFT.ItemsSource = Enum.GetNames(typeof(DSPLib.DSP.Window.Type));
        }

        private void textBox_sizeFFT_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n = 0;
            string txt = textBox_sizeFFT.Text;
            if (txt == "") txt = "0";
            n = Convert.ToInt16(txt);
            if ((n == 64) || (n == 128) || (n == 256) || (n == 512) || (n == 1024) || (n == 2048) || (n == 4096) || (n == 8192) || (n == 16384)) FFT_SIZE = Convert.ToUInt32(textBox_sizeFFT.Text);
        }

        private void comboBox_winFFT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedWindowName = comboBox_winFFT.SelectedValue.ToString();
        }
    }
}
