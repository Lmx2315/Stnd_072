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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Инициализация : Window
    {
        public static bool init;
        public Инициализация()
        {
            InitializeComponent();
            init = true;
        }
        ~Инициализация()
        {
            init = false;
        }

        private void button_DAC0_init_Click(object sender, RoutedEventArgs e)
        {

        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            init = false;
        }

    }
}
