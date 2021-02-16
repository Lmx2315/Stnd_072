using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalisticTelnet;

namespace Stnd_072
{
    class generator_SMW200A
    {
        private string MODEL;

        private string CMD_MSG;
        public string host { set; get; }
        public int port { set; get; }//5025

        public generator_SMW200A(string m) //конструктор
        {
            MODEL = m;
        }

        public string SOURCE(int n) //команда установки частоты
        {
            string prompt = "error!";

            if (n == 1) prompt = ":SOURce1:";
           else
            if (n == 2) prompt = ":SOURce2:";
            
            CMD_MSG = CMD_MSG + prompt;

            return prompt;
        }
        public string FREQ(int freq) //команда установки частоты
        {
            string prompt = "error!";

           prompt = "freq " + freq.ToString() + " Hz;";
  
            CMD_MSG = CMD_MSG + prompt;

            return prompt;
        }

        public string POW(string pow)   //команда установки мощности
        {
            string prompt = "error!";

            prompt = "pow " + pow + " DBm;";

            CMD_MSG = CMD_MSG + prompt;

            return prompt;
        }

        public string OUT1(int outp)   //команда ВКЛ/ВЫКЛ выхода
        {
            string prompt = "error!";
            string z;

            if (outp == 1) z = " 1"; else z = " 0";
            
            prompt = ":OUTPut1:STATe "+z;

            CMD_MSG = CMD_MSG + prompt;

            return prompt;
        }

        public string OUT2(int outp)   //команда ВКЛ/ВЫКЛ выхода
        {
            string prompt = "error!";
            string z;

            if (outp == 1) z = " 1"; else z = " 0";

            prompt = ":OUTPut2:STATe " + z;

            CMD_MSG = CMD_MSG + prompt;

            return prompt;
        }

        public string SEND()
        {
            string prompt = "";
            string msg = "";

            //    try
            {
                TelnetConnection tc = new TelnetConnection(host, port);
                while (tc.IsConnected == false) { };
                
                prompt = CMD_MSG + "";
       
                tc.WriteLine(prompt);
                tc.CLOSE();
                //   msg = tc.Read();//  
            }
            //      catch
            {
                //         MessageBox.Show("Что то не то с IP/порт адресами!");
            }

            CMD_MSG = "";
            return msg;
        }


    }
}
