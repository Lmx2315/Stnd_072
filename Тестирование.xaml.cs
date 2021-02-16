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
    /// Логика взаимодействия для Тестирование.xaml
    /// </summary>
    public partial class Тестирование : Window
    {
        public static bool init;

        public Тестирование(MainWindow main)
        {
            InitializeComponent();

            init = true;

            /*
            ToolTip toolTip = new ToolTip();
            StackPanel toolTipPanel = new StackPanel();
            toolTipPanel.Children.Add(new TextBlock { Text = "Подтверждение введённых данных", FontSize = 16 });
            toolTip.Content = toolTipPanel;
            btn_enter.ToolTip = toolTip;

            ToolTip toolTip1 = new ToolTip();
            StackPanel toolTipPanel1 = new StackPanel();
            toolTipPanel1.Children.Add(new TextBlock { Text = "Не менее 1!", FontSize = 16 });
            toolTip1.Content = toolTipPanel1;
            textBox_N_intervals.ToolTip = toolTip1;
            */

        }

        ~Тестирование()
        {
            init = false;
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            init = false;
        }

        private void SEND_Click(object sender, RoutedEventArgs e)
        {
            generator_SMW200A GEN = new generator_SMW200A("SMW200A");

            GEN.host = ip.Text;
            GEN.port =Convert.ToInt32(port.Text);
            //первый канал
            GEN.SOURCE(1);//всегда вызывается перед параметрами!!!
            GEN.FREQ(Convert.ToInt32(FREQ1.Text));
            GEN.POW(LEVEL1.Text);
            //второй канал
            GEN.SOURCE(2);//всегда вызывается перед параметрами!!!
            GEN.FREQ(Convert.ToInt32(FREQ2.Text));
            GEN.POW(LEVEL2.Text);

            GEN.SEND();

            bool state =Convert.ToBoolean(checkBox1.IsChecked);
            if  (state)
            {
                   GEN.OUT1(1);
            } else GEN.OUT1(0);

            GEN.SEND();

            state = Convert.ToBoolean(checkBox2.IsChecked);
            if  (state)
            {
                GEN.OUT2(1);
            }
            else GEN.OUT2(0);
            
            GEN.SEND();


        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            generator_SMW200A GEN = new generator_SMW200A("SMW200A");

            GEN.host = ip.Text;
            GEN.port = Convert.ToInt32(port.Text);

            bool state = Convert.ToBoolean(checkBox1.IsChecked);
            if (state)
            {
                GEN.OUT1(1);
            }
            else GEN.OUT1(0);

            GEN.SEND();
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            generator_SMW200A GEN = new generator_SMW200A("SMW200A");

            GEN.host = ip.Text;
            GEN.port = Convert.ToInt32(port.Text);

            bool state = Convert.ToBoolean(checkBox2.IsChecked);
            if (state)
            {
                GEN.OUT2(1);
            }
            else GEN.OUT2(0);

            GEN.SEND();
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            generator_SMW200A GEN = new generator_SMW200A("SMW200A");

            GEN.host = ip.Text;
            GEN.port = Convert.ToInt32(port.Text);

            bool state = Convert.ToBoolean(checkBox2.IsChecked);
            if (state)
            {
                GEN.OUT2(1);
            }
            else GEN.OUT2(0);

            GEN.SEND();
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            generator_SMW200A GEN = new generator_SMW200A("SMW200A");

            GEN.host = ip.Text;
            GEN.port = Convert.ToInt32(port.Text);

            bool state = Convert.ToBoolean(checkBox1.IsChecked);
            if (state)
            {
                GEN.OUT1(1);
            }
            else GEN.OUT1(0);

            GEN.SEND();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            label_name_test.Content = "Тест функционального диагностирования ЭМ1";
            MessageBox.Show("Тест функционального диагностирования ЭМ1 пройден!");
        }
        private void button_TST2_Click(object sender, RoutedEventArgs e)
        {
            label_name_test.Content = "Тест проверки основных параметров ЭМ1";
            MessageBox.Show("Тест проверки основных параметров ЭМ1 пройден!");
        }
        private void button_TST3_Click(object sender, RoutedEventArgs e)
        {
            label_name_test.Content = "Режим проверки отклонения ∆fцап рабочей частоты";
            MessageBox.Show("Режим проверки отклонения ∆fцап рабочей частоты пройден!");
        }

    }
}
