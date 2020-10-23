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

namespace Stnd_072
{
    /// <summary>
    /// Логика взаимодействия для Приёмник.xaml
    /// </summary>
    public partial class Приёмник : Window
    {
        public static bool init;
        MainWindow main = null;
        public Приёмник(MainWindow a)
        {
            InitializeComponent();
            init = true;
            main = a;
            if (a != null)
            {
                
            }
        }

        ~Приёмник()
        {
            init = false;
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            init = false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            byte[] a = new byte[4];
            string log_txt;
            a[0]=Convert.ToByte(textBox_Att0.Text);
            a[1]=Convert.ToByte(textBox_Att1.Text);
            a[2]=Convert.ToByte(textBox_Att2.Text);
            a[3]=Convert.ToByte(textBox_Att3.Text);

            log_txt = "Отправляем значения аттенюатора 0=" + a[0].ToString() + ",1=" + a[1].ToString() + ",2=" + a[2].ToString() + ",3=" + a[3].ToString();

            if (main != null)
            {
                Console.WriteLine(log_txt);
                    Log.Write    (log_txt);
                main.udp0_sender.UDP_SEND(UDP_sender.CMD.CMD_ATT,a,4,0);
            }
        }
    }
}
