using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stnd_072
{
    public class ADC_status
    {
        public int INIT { get; set; }
        public int rx_datak_adc { get; set; }
        public int rx_errdetect_adc { get; set; }
        public int align_ok_adc { get; set; }
        public int rx_ready_adc { get; set; }
        public int sync_n_adc { get; set; }
        public int rx_syncstatus_adc { get; set; }
        public int align_adc { get; set; }
        public int error_adc { get; set; }
        public int error_sysref_adc { get; set; }

        public int ERROR { get; set; }
        public string MSG { get; set; }
    }
}
