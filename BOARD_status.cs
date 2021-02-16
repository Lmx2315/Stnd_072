using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stnd_072
{
    public class BOARD_status
    {
        public int INIT { get; set; }
        public int TEMP0 { get; set; }
        public int TEMP1 { get; set; }
        public int BRD_NUMBER { get; set; }

        public int ERROR { get; set; }
        public string MSG { get; set; }

        public int zATT0 { get; set; }
        public int zATT1 { get; set; }
        public int zATT2 { get; set; }
        public int zATT3 { get; set; }

        public void ATT0(int a)
        {
            zATT0 = a;
        }

        public void ATT1(int a)
        {
            zATT1 = a;
        }

        public void ATT2(int a)
        {
            zATT2 = a;
        }

        public void ATT3(int a)
        {
            zATT3 = a;
        }

    }
}
