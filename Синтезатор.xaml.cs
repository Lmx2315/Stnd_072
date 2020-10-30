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
            public Int64 TIME_END;     //финишное  время цикла
            public Int64 FREQ;         //частота
            public Int64 FREQ_STEP;    //Считается изходя из девиации частоты или наоборот
            public Int32 FREQ_RATE;    //Считается изходя из девиации частоты или наоборот
            public Int16 N_cikl;       //число интервалов
            public Int64 deviation;    //девиация частоты
            public Int16 TYPE;         //тип пачки
            public Int32 Ti;           //интервал Излучения
            public Int32 Tp;           //интервал Приёма
            public Int32 Tblank1;      //интервал перед излучением
            public Int32 Tblank2;      //интервал перед приёмом
            public Int16 Att0;         //Аттенюатор в нулевом канале
            public Int16 Att1;         //Аттенюатор в первом канале
            public Int16 Att2;         //Аттенюатор в втором канале
            public Int16 Att3;         //Аттенюатор в третьем канале
            public Int16 PHASE0;       //Фаза 0 канал излучения
            public Int16 PHASE1;       //Фаза 1 канали излучения
            public Int16 PHASE2;       //Фаза 2 канали излучения
            public Int16 PHASE3;       //Фаза 3 канали излучения
            public Int16 DELAY0;       //Задержка в нС в 0 канале
            public Int16 DELAY1;       //Задержка в нС в 1 канале
            public Int16 DELAY2;       //Задержка в нС в 2 канале
            public Int16 DELAY3;       //Задержка в нС в 3 канале
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

            c0.N_cikl=Convert.ToInt16(textBox_N_intervals.Text);//число интервалов в цикле
            c0.NUMBER_RECORD=Convert.ToInt32(textBox_Number_record.Text);

            c0.PHASE0=Convert.ToInt16(textBox_PHASE0.Text);
            c0.PHASE1=Convert.ToInt16(textBox_PHASE1.Text);
            c0.PHASE2=Convert.ToInt16(textBox_PHASE2.Text);
            c0.PHASE3=Convert.ToInt16(textBox_PHASE3.Text);

            c0.Tblank1=Convert.ToInt32(textBox_Tdop_iz.Text);
            c0.Tblank2=Convert.ToInt32(textBox_Tdop_pr.Text);

            c0.Ti=Convert.ToInt32(textBox_Ti.Text);
            c0.Tp=Convert.ToInt32(textBox_Tp.Text);

     //     c0.TIME_START=Convert.ToInt64(textBox_TIME_START.Text);
        if (list.Count>0)
            {
                if (c0.TIME_START< list[(list.Count - 1)].TIME_END)  c0.TIME_START = list[(list.Count-1)].TIME_END;
            } else
            {
                c0.TIME_START = Convert.ToInt64(textBox_TIME_START.Text);
            }
            
            c0.TYPE=0;

            textBox_Dlitelnost_cikl.Text = (FUN_INTERVAL_CALC(c0)).ToString();//рассчитываем длительность текущего цикла
            c0.TIME_END = Convert.ToInt64(textBox_Dlitelnost_cikl.Text)+ c0.TIME_START;
            c0.NUMBER_RECORD = list.Count;

            list.Add(c0);
            textBox_N_cikl.Text = (list.Count).ToString();                //считаем общее количество циклов
            textBox_Number_record.Text = (c0.NUMBER_RECORD).ToString();  //номер текущего цикла

        }

        int FUN_INTERVAL_CALC (RABCIKL a)
        {
            int INTERVAL;
            INTERVAL = (a.Tblank1 + a.Ti + a.Tblank2 + a.Tp) * a.N_cikl;//расчитали длительность цикла
            return INTERVAL;
        }

        private void button1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            path = System.IO.Path.GetDirectoryName(path);
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter= "xml|*.xml|text|*.txt";
            sf.Title= "Save an File";
            sf.InitialDirectory = path;
            sf.ShowDialog();
            string filename = sf.FileName;
            XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(List<RABCIKL>));

            FileStream fw = new FileStream(filename, FileMode.Create);
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

                    textBox_N_cikl.Text=(list.Count).ToString(); //считаем общее количество циклов
                    FUN_CIKL_DISP(0);//обновляем экран по содержимому структуры
                }
                catch
                {

                }
            }
            
        }

        void FUN_LIST_UPDATE (int a)
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

            c0.Calibrovka = Convert.ToInt16(checkBox_Calibrovka.IsChecked);
            c0.Coherent = Convert.ToInt16(checkBox_Coherent.IsChecked);

            c0.DELAY0 = Convert.ToInt16(textBox_DELAY0.Text);
            c0.DELAY1 = Convert.ToInt16(textBox_DELAY1.Text);
            c0.DELAY2 = Convert.ToInt16(textBox_DELAY2.Text);
            c0.DELAY3 = Convert.ToInt16(textBox_DELAY3.Text);

            c0.deviation = Convert.ToInt64(textBox_dev_FREQ.Text);

            c0.FREQ = Convert.ToInt64(textBox_FREQ.Text);
            c0.FREQ_STEP = Convert.ToInt64(textBox_FREQ_STEP.Text);
            c0.FREQ_RATE = Convert.ToInt32(textBox_FREQ_RATE.Text);

            c0.N_cikl = Convert.ToInt16(textBox_N_intervals.Text);//число интервалов в цикле
            c0.NUMBER_RECORD = Convert.ToInt32(textBox_Number_record.Text);

            c0.PHASE0 = Convert.ToInt16(textBox_PHASE0.Text);
            c0.PHASE1 = Convert.ToInt16(textBox_PHASE1.Text);
            c0.PHASE2 = Convert.ToInt16(textBox_PHASE2.Text);
            c0.PHASE3 = Convert.ToInt16(textBox_PHASE3.Text);

            c0.Tblank1 = Convert.ToInt32(textBox_Tdop_iz.Text);
            c0.Tblank2 = Convert.ToInt32(textBox_Tdop_pr.Text);

            c0.Ti = Convert.ToInt32(textBox_Ti.Text);
            c0.Tp = Convert.ToInt32(textBox_Tp.Text);

            c0.TIME_START = Convert.ToInt64(textBox_TIME_START.Text);
            c0.TYPE = 0;
            c0.NUMBER_RECORD = list.Count;
            textBox_Dlitelnost_cikl.Text = FUN_INTERVAL_CALC(c0).ToString();//рассчитываем длительность текущего цикла
  
            if (list.Count > 0)
            {
                if (c0.TIME_START < list[(a-1)].TIME_END) c0.TIME_START = list[(a-1)].TIME_END;
                Console.WriteLine("a                    : " + a);
                Console.WriteLine("c0.TIME_START        : "+ c0.TIME_START);
                Console.WriteLine("list[(a-1)].TIME_END :" + list[(a - 1)].TIME_END);
            }
            else
            {
                c0.TIME_START = Convert.ToInt64(textBox_TIME_START.Text);
            }

            c0.TIME_END = c0.TIME_START + Convert.ToInt64(textBox_Dlitelnost_cikl.Text);

            if (list.Count != 0)//проверяем что в списке есть элементы
            {
                list.RemoveAt(a); 
                list.Insert(a, c0);
            }

            textBox_TIME_END.Text   = c0.TIME_END.ToString();
            textBox_TIME_START.Text = c0.TIME_START.ToString();
        }

        void FUN_CIKL_DISP (int a)
        {

            textBox_AMP0.Text = list[a].Amplitude0.ToString();
            textBox_AMP1.Text = list[a].Amplitude1.ToString();
            textBox_AMP2.Text = list[a].Amplitude2.ToString();
            textBox_AMP3.Text = list[a].Amplitude3.ToString();

            textBox_ATT0.Text = list[a].Att0.ToString();
            textBox_ATT1.Text = list[a].Att1.ToString();
            textBox_ATT2.Text = list[a].Att2.ToString();
            textBox_ATT3.Text = list[a].Att3.ToString();

            checkBox_Calibrovka.IsChecked = Convert.ToBoolean(list[a].Calibrovka);
            checkBox_Coherent.IsChecked   = Convert.ToBoolean(list[a].Coherent);

            textBox_DELAY0.Text = list[a].DELAY0.ToString();
            textBox_DELAY1.Text = list[a].DELAY1.ToString();
            textBox_DELAY2.Text = list[a].DELAY2.ToString();
            textBox_DELAY3.Text = list[a].DELAY3.ToString();

            textBox_dev_FREQ.Text  = list[a].deviation.ToString();
            textBox_FREQ.Text      = list[a].FREQ.ToString();
            textBox_FREQ_STEP.Text = list[a].FREQ_STEP.ToString();
            textBox_FREQ_RATE.Text = list[a].FREQ_RATE.ToString();

            textBox_N_intervals.Text = list[a].N_cikl.ToString();
  
            textBox_PHASE0.Text = list[a].PHASE0.ToString();
            textBox_PHASE1.Text = list[a].PHASE1.ToString();
            textBox_PHASE2.Text = list[a].PHASE2.ToString();
            textBox_PHASE3.Text = list[a].PHASE3.ToString();

            textBox_Tdop_iz.Text = list[a].Tblank1.ToString();
            textBox_Tdop_pr.Text = list[a].Tblank2.ToString();
            textBox_Ti.Text = list[a].Ti.ToString();
            textBox_Tp.Text = list[a].Tp.ToString();

            textBox_TIME_START.Text = list[a].TIME_START.ToString();
            textBox_Dlitelnost_cikl.Text = (FUN_INTERVAL_CALC(list[a])).ToString();//рассчитываем длительность текущего цикла
            textBox_Number_record.Text = a.ToString();                              //номер текущего цикла
            textBox_N_cikl.Text = (list.Count).ToString();
            textBox_TIME_END.Text = (list[a].TIME_START + Convert.ToInt64(textBox_Dlitelnost_cikl.Text)).ToString();
        }

        void FUN_CLR_DISP()
        {
            textBox_AMP0.Text = 0.ToString();
            textBox_AMP1.Text = 0.ToString();
            textBox_AMP2.Text = 0.ToString();
            textBox_AMP3.Text = 0.ToString();

            textBox_ATT0.Text = 0.ToString();
            textBox_ATT1.Text = 0.ToString();
            textBox_ATT2.Text = 0.ToString();
            textBox_ATT3.Text = 0.ToString();

            checkBox_Calibrovka.IsChecked = Convert.ToBoolean(0);
            checkBox_Coherent.IsChecked = Convert.ToBoolean(0);

            textBox_DELAY0.Text = 0.ToString();
            textBox_DELAY1.Text = 0.ToString();
            textBox_DELAY2.Text = 0.ToString();
            textBox_DELAY3.Text = 0.ToString();

            textBox_dev_FREQ.Text = 0.ToString();
            textBox_FREQ.Text = 0.ToString();
            textBox_FREQ_STEP.Text = 0.ToString();
            textBox_FREQ_RATE.Text = 0.ToString();

            textBox_N_intervals.Text = 0.ToString();

            textBox_PHASE0.Text = 0.ToString();
            textBox_PHASE1.Text = 0.ToString();
            textBox_PHASE2.Text = 0.ToString();
            textBox_PHASE3.Text = 0.ToString();

            textBox_Tdop_iz.Text = 0.ToString();
            textBox_Tdop_pr.Text = 0.ToString();
            textBox_Ti.Text = 0.ToString();
            textBox_Tp.Text = 0.ToString();

            textBox_TIME_START.Text = 0.ToString();
            textBox_Dlitelnost_cikl.Text = (0).ToString();//рассчитываем длительность текущего цикла
            textBox_Number_record.Text = 0.ToString();                              //номер текущего цикла
            textBox_N_cikl.Text = (0).ToString();
            textBox_TIME_END.Text = (0).ToString();
        }

        void NEW_LIST_FORM ()
        {
        //  RABCIKL c0 = new RABCIKL();
        //  list.Add(c0);
            textBox_N_cikl.Text = (0).ToString();                //считаем общее количество циклов
            textBox_Number_record.Text = (0).ToString();  //номер текущего цикла
        }

        private void textBox_Number_record_TextChanged(object sender, TextChangedEventArgs e)
        {
            int a = Convert.ToInt32(textBox_Number_record.Text);
            try
            {
                if (a<list.Count)
                {
                    FUN_CIKL_DISP(a);//обновляем экран по содержимому структуры
                }
                else
                {
                    if ((Convert.ToInt32(textBox_Number_record.Text))!=0)
                    {
                        textBox_Number_record.Text = (list.Count-1).ToString();
                        MessageBox.Show("Нет такого элемента!");
                    }
                    
                }
            }
            catch
            {
                Console.WriteLine("чёто не то!");
            }
        }

        private void button1_Copy2_Click(object sender, RoutedEventArgs e)
        {
            int a = Convert.ToInt32(textBox_Number_record.Text);
            try
            {
                if ((a < list.Count)&&(list.Count>0))
                {
                    list.RemoveAt(a);
                    if (list.Count > 0) FUN_CIKL_DISP(0);//обновляем экран по содержимому структуры
                    else if (list.Count==0)
                    {
                        textBox_N_cikl.Text = (list.Count).ToString();
                    }
                }
            }
            catch
            {
                Console.WriteLine("чёто не то!");
            }
        }

        private void button_enter(object sender, RoutedEventArgs e)
        {
            FUN_LIST_UPDATE(Convert.ToInt32(textBox_Number_record.Text));
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            list.Clear();       //удаляем из списка все элементы
            NEW_LIST_FORM();    //пока с нулевым элементом
            FUN_CLR_DISP();     // 
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int error = 0;
            //--------Проверка списка команд на исполнение--------
            if (list.Count == 0) MessageBox.Show("Список команд пустой!");
            else
            for (var i=0;i<list.Count;i++)
            {
               if (i>0)
                    {
                        if (list[i].TIME_START < list[i - 1].TIME_END) error++;
                    }
            }

            if (error != 0) MessageBox.Show("   Есть ошибки в списке команд!\n Ошибки времени начала команд.");
        }
    }
}
