using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stnd_072
{
   public class STRUCT_FFT_DATA
    {
        public double AMAX, BMAX, CMAX, M1X, M1Y, M2X, M2Y, M3X, M3Y;
        public double[] TSAMPL ;
        public double[] MAG_LOG ;
        public double[] time_series;
        public int SCH_UDP_PKG;//счётчик числа пришедших пакетов
        public int SCH_FFT_PKG0;//счётчик числа сформированых для БПФ пакетов
        public int SCH_FFT_PKG1;//счётчик числа сформированых для БПФ пакетов

        public STRUCT_FFT_DATA (int N)
        {
            TSAMPL = new double[N];
            MAG_LOG = new double[N];
            time_series = new double[N];
        }
    }
}
