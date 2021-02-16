using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stnd_072
{
    public class FPGA_status
    {
        public int INIT { get; set; }
        public int TEMP { get; set; }
        public int STATUS_REF { get; set; }
        public int STATUS_SYNC { get; set; }
        public int STATUS_1HZ { get; set; }
        public int ERROR_1HZ { get; set; }
        public int SYNC0_MIN { get; set; }
        public int SYNC0_MAX { get; set; }
        public int SYNC1_MIN { get; set; }
        public int SYNC1_MAX { get; set; }
        public int SYNC2_MIN { get; set; }
        public int SYNC2_MAX { get; set; }
        public int ERROR { get; set; }
        public string MSG { get; set; }
    }
}
