using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stnd_072
{
    public class LMK_status
    {
        public int INIT { get; set; }
        public int lb { get; set; }
        public int RB_DAC_VALUE { get; set; }
        public int PLL2_LD { get; set; }
        public int PLL1_LD { get; set; }
        public int STATUS_LD1 { get; set; }
        public int STATUS_LD2 { get; set; }

        public int ERROR { get; set; }
        public string MSG { get; set; }
    }
}
