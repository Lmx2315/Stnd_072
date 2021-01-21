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
    public partial class ADC_panel : Window
    {
        public static bool ADC0;
        public static bool ADC1;

        string name;

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        public ADC_panel(MainWindow main, string name)
        {
            InitializeComponent();
            if (name == "ADC0") ADC0 = true;
            else
            if (name == "ADC1") ADC1 = true;
            this.name = name;

            this.Title = name.ToUpper();

            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 250);
            Timer1.Start();//запускаю таймер 
        }

        ~ADC_panel()
        {
            Console.WriteLine("Удаляем панель ADC");
            if (name == "ADC0") ADC0 = false;
            else
            if (name == "ADC1") ADC1 = false;
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            if (name == "ADC0") ADC0 = false;
            else
            if (name == "ADC1") ADC1 = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;

            if (main != null)
            {

            }

            if (name == "ADC0")
            {
                if (main.b072.ADC0.MSG != "") this.Show_data(main.b072.ADC0.MSG);
                main.b072.ADC0.MSG = "";
            }
            else
            if (name == "ADC1")
            {
                if (main.b072.ADC1.MSG != "") this.Show_data(main.b072.ADC1.MSG);
                main.b072.ADC1.MSG = "";
            }
        }
        public void Show_data(string a)
        {
            //       richTextBox.Document.Blocks.Add(new Paragraph(new Run(a)));
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

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}




