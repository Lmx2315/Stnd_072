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
    /// Логика взаимодействия для Синтезатор.xaml
    /// </summary>
    public partial class Синтезатор : Window
    {
        public static bool init;

        public struct RABCIKL
        {
            public Int32 NUMBER_RECORD;//номер цикла
            public Int64 TIME_START;   //стартовое время цикла
            public Int64 FREQ;        //частота
            public Int64 FREQ_STEP;   //Считается изходя из девиации частоты или наоборот
            public Int32 FREQ_RATE;   //Считается изходя из девиации частоты или наоборот
            public Int16 N_cikl;      //число интервалов
            public Int64 deviation;   //девиация частоты
            public Int16 TYPE;        //тип пачки
            public Int32 Ti;          //интервал Излучения
            public Int32 Tp;          //интервал Приёма
            public Int32 Tblank1;     //интервал перед излучением
            public Int32 Tblank2;     //интервал перед приёмом
            public Int16 Att0;        //Аттенюатор в нулевом канале
            public Int16 Att1;        //Аттенюатор в первом канале
            public Int16 Att2;        //Аттенюатор в втором канале
            public Int16 Att3;        //Аттенюатор в третьем канале
            public Int16 PHASE0;      //Фаза 0 канал излучения
            public Int16 PHASE1;      //Фаза 1 канали излучения
            public Int16 PHASE2;      //Фаза 2 канали излучения
            public Int16 PHASE3;      //Фаза 3 канали излучения
            public Int16 DELAY0;      //Задержка в нС в 0 канале
            public Int16 DELAY1;      //Задержка в нС в 1 канале
            public Int16 DELAY2;      //Задержка в нС в 2 канале
            public Int16 DELAY3;      //Задержка в нС в 3 канале
            public Int16 Amplitude0;      //Задержка в нС в 0 канале
            public Int16 Amplitude1;      //Задержка в нС в 1 канале
            public Int16 Amplitude2;      //Задержка в нС в 2 канале
            public Int16 Amplitude3;      //Задержка в нС в 3 канале
            public Int16 Calibrovka;      //измерения на интервале излучения
            public Int16 Coherent;        //пачка когерентная
             
        }
        public Синтезатор( MainWindow main)
        {
            InitializeComponent();
            init = true;
        }

        ~ Синтезатор()
        {
            init = false;
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            init = false;
        }

        List<RABCIKL> list = new List<RABCIKL>();

        private void button1_Copy1_Click(object sender, RoutedEventArgs e)
        {
            RABCIKL c0 = new RABCIKL();
            c0.Amplitude0 = Convert.ToInt16(textBox_AMP0.Text);
            c0.Amplitude1 = Convert.ToInt16(textBox_AMP1.Text);
            c0.Amplitude2 = Convert.ToInt16(textBox_AMP2.Text);
            c0.Amplitude3 = Convert.ToInt16(textBox_AMP3.Text);

            c0.Att0 = Convert.ToInt16(textBox_ATT0.Text);
            c0.Att1 = Convert.ToInt16(textBox_ATT1.Text);
            c0.Att2 = Convert.ToInt16(textBox_ATT2.Text);
            c0.Att3 = Convert.ToInt16(textBox_ATT3.Text);

            c0.Calibrovka=Convert.ToInt16(checkBox_Calibrovka.IsChecked);
            c0.Coherent  =Convert.ToInt16(checkBox_Coherent.IsChecked);

            c0.DELAY0=Convert.ToInt16(textBox_DELAY0.Text);
            c0.DELAY1=Convert.ToInt16(textBox_DELAY1.Text);
            c0.DELAY2=Convert.ToInt16(textBox_DELAY2.Text);
            c0.DELAY3=Convert.ToInt16(textBox_DELAY3.Text);

            c0.deviation=Convert.ToInt64(textBox_dev_FREQ.Text);

            c0.FREQ=Convert.ToInt64(textBox_FREQ.Text);
            c0.FREQ_STEP=Convert.ToInt64(textBox_FREQ_STEP.Text);
            c0.FREQ_RATE=Convert.ToInt32(textBox_FREQ_RATE.Text);

            c0.N_cikl=Convert.ToInt16(textBox_N_intervals.Text);
            c0.NUMBER_RECORD=Convert.ToInt32(textBox_Number_record.Text);

            c0.PHASE0=Convert.ToInt16(textBox_PHASE0.Text);
            c0.PHASE1=Convert.ToInt16(textBox_PHASE1.Text);
            c0.PHASE2=Convert.ToInt16(textBox_PHASE2.Text);
            c0.PHASE3=Convert.ToInt16(textBox_PHASE3.Text);

            c0.Tblank1=Convert.ToInt32(textBox_Tdop_iz.Text);
            c0.Tblank2=Convert.ToInt32(textBox_Tdop_pr.Text);

            c0.Ti=Convert.ToInt32(textBox_Ti.Text);
            c0.Tp=Convert.ToInt32(textBox_Tp.Text);

            c0.TIME_START=Convert.ToInt64(textBox_TIME_START.Text);
            c0.TYPE=0;

            list.Add(c0);
            list.Add(c0);
           
        }

        private void button1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(List<RABCIKL>));

            FileStream fw = new FileStream("output.xml", FileMode.Create);
            xmlSerialaizer.Serialize(fw, list);
            fw.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            path = System.IO.Path.GetDirectoryName(path);
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "xml|*.xml|text|*.txt";
            of.Title = "Load an File";
            of.InitialDirectory = path;
            of.ShowDialog();

            if (of.CheckFileExists == true)
            {
                // получаем выбранный файл
                string filename = of.FileName;
                try
                {
                    XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(List<RABCIKL>));
                    FileStream fr = new FileStream(filename, FileMode.Open);
                    list = (List<RABCIKL>)xmlSerialaizer.Deserialize(fr);
                    fr.Close();
                }
                catch
                {

                }
            }
            
        }
    }
}
