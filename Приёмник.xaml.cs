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
        public Приёмник()
        {
            InitializeComponent();
            init = true;
        }

        ~Приёмник()
        {
            init = false;
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            init = false;
        }
    }
}
