using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stnd_072
{
 public   class BRD_state
    {
        int att0;//тут задаём значение аттенюатора в нулевом канале
        public int Att0
        {
            get { return att0; }
            set 
            { if (value < 32) att0 = value;
                else throw new ArgumentException("Wrong value!");
            }
        }

        int att1;//тут задаём значение аттенюатора в первом канале
        public int Att1
        {
            get { return att1; }
            set
            {
                if (value < 32) att1 = value;
                else throw new ArgumentException("Wrong value!");
            }
        }

        int att2;//тут задаём значение аттенюатора во втором канале
        public int Att2
        {
            get { return att2; }
            set
            {
                if (value < 32) att2 = value;
                else throw new ArgumentException("Wrong value!");
            }
        }

        int att3;//тут задаём значение аттенюатора в третьем канале
        public int Att3
        {
            get { return att3; }
            set
            {
                if (value < 32) att3 = value;
                else throw new ArgumentException("Wrong value!");
            }
        }

        public byte [] ATT_TDATA ()
        {
            byte[] m = new byte[4];

            m[0] = (byte)att0;
            m[1] = (byte)att1;
            m[2] = (byte)att2;
            m[3] = (byte)att3;
            return m; 
        }

        bool CH0 { get; set; }
        bool CH1 { get; set; }
        bool CH2 { get; set; }
        bool CH3 { get; set; }


    }
}
