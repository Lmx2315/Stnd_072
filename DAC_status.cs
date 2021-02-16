using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stnd_072
{
    public class DAC_status
    {
        public int dac_pll_locked { get; set; }
        public int ALARM_ERROR { get; set; }
        public int SYNC_N_ERROR { get; set; }
        public int INIT { get; set; }
        public int alarms_from_lanes { get; set; }
        public int memin_pll_lfvolt { get; set; }
        public int alarm_rw0_pll { get; set; }
        public int alarm_sysref_err { get; set; }
        public int alarm_l_error_0 { get; set; }
        public int alarm_fifo_flags_0 { get; set; }
        public int alarm_l_error_1 { get; set; }
        public int alarm_fifo_flags_1 { get; set; }
        public int error_count_link0 { get; set; }
        public int error_count_link1 { get; set; }
        public int D_ALARM { get; set; }
        public int TEMP { get; set; }

        public int ERROR { get; set; }
        public string MSG { get; set; }

    }
}
