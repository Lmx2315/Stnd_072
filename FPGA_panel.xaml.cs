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
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;

namespace Stnd_072
{
    /// <summary>
    /// Логика взаимодействия для DAC.xaml
    /// </summary>
    public partial class FPGA_panel : Window
    {
        public static bool FPGA;

        string name;

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        public FPGA_panel(MainWindow main, string name)
        {
            InitializeComponent();
            if (name == "FPGA") FPGA = true;
            this.name = name;
            this.Title = name.ToUpper();

            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 250);
            Timer1.Start();//запускаю таймер 
        }

        FPGA_panel()
        {
            Console.WriteLine("Удаляем панель FPGA");
            if (name == "FPGA") FPGA = false;            
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            if (name == "FPGA") FPGA = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;

            if (main != null)
            {

            }

            if (name == "FPGA")
            {
                TEMP_disp(main.b072.FPGA.TEMP);
                if (main.b072.FPGA.MSG != "") this.Show_data(main.b072.FPGA.MSG);
                main.b072.FPGA.MSG = "";
            }
        }
        public void Show_data(string a)
        {
            richTextBox.AppendText(a);
            richTextBox.ScrollToEnd();
        }

        void TEMP_disp(int temp)
        {
            label_Temp.Content = temp.ToString();
        }

        private void button_clr_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Document.Blocks.Clear();
        }
    }
}




