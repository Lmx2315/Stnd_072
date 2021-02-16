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
    public partial class BOARD_panel : Window
    {
        public static bool BOARD;

        string name;

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();
        public BOARD_panel(MainWindow main, string name)
        {
            InitializeComponent();
            if (name == "BOARD") BOARD = true;
            this.name = name;
            this.Title = name.ToUpper();

            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 250);
            Timer1.Start();//запускаю таймер 
        }

        BOARD_panel()
        {
            Console.WriteLine("Удаляем панель BOARD");
            if (name == "BOARD") BOARD = false;
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            if (name == "BOARD") BOARD = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;

            if (main != null)
            {

            }

            if (name == "BOARD")
            {
                label_Temp0.Content = main.b072.BRD.TEMP0.ToString();
                label_Temp1.Content = main.b072.BRD.TEMP1.ToString();
                if (main.b072.BRD.MSG != "") this.Show_data(main.b072.BRD.MSG);
                main.b072.BRD.MSG = "";
            }
        }
        public void Show_data(string a)
        {
            string myText = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            if (a != myText)
            {
                richTextBox.Document.Blocks.Clear();
                richTextBox.AppendText(a);
                richTextBox.ScrollToEnd();
            }
        }

        private void button_clr_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Document.Blocks.Clear();
        }
    }
}




