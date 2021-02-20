using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FFTLibrary;

namespace Stnd_072
{
    public class UDP_DATA_SERVER
    {
        static int BUF_SIZE = 131072;          //это размер максимального БПФ*4 (т.е. в байтах)
        static int FFT_SIZE_MAX = BUF_SIZE / 4;//максимальный размер БПФ
        public uint FFT_SIZE;            //текущий размер БПФ
        static int  BUF_ETH = 64;
        List<byte[]>      _data = new List<byte[]>(BUF_ETH);
             byte[]  UDP_packet = new byte[1446];
        IPAddress my_ip;
        UInt16 my_port;

        IPEndPoint _client = null;
        UdpClient  _server = null;

        int FLAG_UDP_RCV;
        
        int FLAG_DATA0 = 0;//флаг готовности буфера данных к обработке
        int FLAG_DATA1 = 0;

        public int FLAG_FFT0 = 0;//флаг готовности к расчёту БПФ
        public int FLAG_FFT1 = 0;

        public int FLAG_DISPAY0 = 0;//флаг показывает возможность вывода на экран
        public int FLAG_DISPAY1 = 0;
        public string selectedWindowName="Hann";

        int CH0 = 0;       //номер нулевого канала
        int CH1 = 1;       //номер первого  канала

        int[] DATA0_i = new int[FFT_SIZE_MAX];
        int[] DATA0_q = new int[FFT_SIZE_MAX];

        int[] DATA1_i = new int[FFT_SIZE_MAX];
        int[] DATA1_q = new int[FFT_SIZE_MAX];

        double[][] MeM0_0 = new double[100][];//массив памяти для сглаживания вывода спектроанализатора
        double[][] MeM0_1 = new double[100][];//массив памяти для сглаживания вывода спектроанализатора

        double[][] MeM1_0 = new double[100][];//массив памяти для сглаживания вывода спектроанализатора
        double[][] MeM1_1 = new double[100][];//массив памяти для сглаживания вывода спектроанализатора

        public STRUCT_FFT_DATA data0 = new STRUCT_FFT_DATA(FFT_SIZE_MAX);
        public STRUCT_FFT_DATA data1 = new STRUCT_FFT_DATA(FFT_SIZE_MAX);

        public int FLAG_filtr = 0;//флаг описывает состояние используется ли фильтр для сглаживания выходных данных
        int FLAG_THREAD = 1;

        public UDP_DATA_SERVER(MainWindow a,string IP,string PORT)
        {
            my_ip = IPAddress.Parse(IP);
            my_port = UInt16.Parse(PORT);
            FFT_SIZE = a.FFT_SIZE;
            START();
        }

        private void START()
        {
            Console.WriteLine("my_ip  :" + my_ip);
            Console.WriteLine("my_port:" + my_port);
            IPEndPoint serverEnd = new IPEndPoint(my_ip, my_port);
                         _server = new UdpClient(serverEnd);
                      _server.Client.ReceiveBufferSize = 1000000;//

            for (int i = 0; i < BUF_ETH; i++) _data.Add(UDP_packet); //создаём кольцевой буфер для приёма UDP пакетов
            //Start listening.
            Thread _listenThread = new Thread(new ThreadStart(Listening));//тред приёма данных по UDP
            _listenThread.Start();
            _listenThread.IsBackground = true;//делает поток фоновым который завершается по закрытию основного приложения

            //Start copy-ing.
            Thread _copyThread = new Thread(new ThreadStart(DATA_COPY));//тред копирования данных в буфер обработки
            _copyThread.Start();
            _copyThread.IsBackground = true;

            //Start fft-ing.
            Thread _fftThread0 = new Thread(new ThreadStart(FFT_WORK0));//тред расчёта fft
            _fftThread0.Start();
            _fftThread0.IsBackground = true;

            
            Thread _fftThread1 = new Thread(new ThreadStart(FFT_WORK1));//тред расчёта fft
            _fftThread1.Start();
            _fftThread1.IsBackground = true;
            
        }

        int INDEX = 0;
        private void Listening()
        {
            //Listening loop.
            while ((true)&&(FLAG_THREAD == 1))
            {
                try
                { //receieve a message form a client.
                    if (FLAG_UDP_RCV==0)
                    {
                        _data[INDEX] = _server.Receive(ref _client);
                                 
                        if (INDEX < (BUF_ETH - 1)) INDEX = INDEX + 1;
                        else
                        {
                            INDEX = 0;
                            FLAG_UDP_RCV = 1;
                        }
                    }                  
                }
                catch (Exception excp)
                {
                    Console.WriteLine(excp.Message);
                }
            }
        }

        private void DATA_COPY()
        {
            while (true)
            {
                while (FLAG_UDP_RCV == 1)
                {
                    byte[] m;//буфер0 приёма данных
                    byte[] BUF0 = new byte[BUF_SIZE];
                    byte[] BUF1 = new byte[BUF_SIZE];
                    int[] zi = new int[FFT_SIZE_MAX];
                    int[] zq = new int[FFT_SIZE_MAX];
                    uint time0_0 = 0;
                    uint time0_1;

                    uint time1_0 = 0;
                    uint time1_1;
                    int OFFSET0 = 0;
                    int OFFSET1 = 0;
                    uint NBUF= FFT_SIZE*4;

                    lock (_data)
                    {
                        for (int i = 0; i < (BUF_ETH - 1); i++)
                        {
                            m = _data[i];

                            if ((m[1] == CH0) && (FLAG_DATA0 == 0))
                            {
                                //считываем время пакета
                                time0_1 = (Convert.ToUInt32(m[2]) << 24) + (Convert.ToUInt32(m[3]) << 16) + (Convert.ToUInt32(m[4]) << 8) + (Convert.ToUInt32(m[5]) << 0);

                                if (time0_1 == (time0_0 + 1))
                                {
                                    Array.Copy(m, 6, BUF0, OFFSET0, (m.Length - 6));//копируем массив отсчётов в массив обработки с текущей позиции
                                    OFFSET0 = OFFSET0 + m.Length - 6;

                                    if (OFFSET0 > NBUF)//
                                    {
                                        time0_0 = 0;
                                        OFFSET0 = 0;
                                        FLAG_DATA0 = 1;
                                        //data0.SCH_FFT_PKG0++;
                                    }
                                }
                                else
                                {
                                    OFFSET0 = 0;
                                }
                                time0_0 = time0_1;
                            }
                            else
                            if ((m[1] == CH1) && (FLAG_DATA1 == 0))
                            {
                                //считываем время пакета
                                time1_1 = (Convert.ToUInt32(m[2]) << 24) + (Convert.ToUInt32(m[3]) << 16) + (Convert.ToUInt32(m[4]) << 8) + (Convert.ToUInt32(m[5]) << 0);

                                if (time1_1 == (time1_0 + 1))
                                {
                                    Array.Copy(m, 6, BUF1, OFFSET1, (m.Length - 6));//копируем массив отсчётов в массив обработки с текущей позиции
                                    OFFSET1 = OFFSET1 + m.Length - 6;

                                    if (OFFSET1 > NBUF)//
                                    {
                                        time1_0 = 0;
                                        OFFSET1 = 0;
                                        FLAG_DATA1 = 1;
                                        data0.SCH_FFT_PKG1++;
                                    }
                                }
                                else
                                {
                                    OFFSET1 = 0;
                                }
                                time1_0 = time1_1;
                            }
                            else
                            if ((FLAG_DATA0 == 1) && (FLAG_DATA1 == 1)) break; //выходим из цикла если пакеты собраны
                        }

                        if (FLAG_DATA0 == 1) 
                        { 
                            (zi, zq) = BUF_convert(BUF0);
                            Array.Copy(zi, DATA0_i, zi.Length);
                            Array.Copy(zq, DATA0_q, zq.Length);
                            FLAG_FFT0  = 1;
                            FLAG_DATA0 = 0;
                        }
                        if (FLAG_DATA1 == 1) 
                        { 
                            (zi, zq) = BUF_convert(BUF1);
                            Array.Copy(zi, DATA1_i, zi.Length);
                            Array.Copy(zq, DATA1_q, zq.Length);
                            FLAG_FFT1  = 1;
                            FLAG_DATA1 = 0;
                        }

                        FLAG_UDP_RCV = 0;
                    }
                }

                Thread.Sleep(0);
            }
        }


        (int [],int[]) BUF_convert(byte[] m)//разбирает массив данных на I и Q составляющие
        {
            int i;
            int k = 0;
            int l = 0;

            int[] zi = new int[FFT_SIZE_MAX];
            int[] zq = new int[FFT_SIZE_MAX];

            for (i = 0; i < m.Length; i++)//
            {
                if (k == 0) zi[l] =         Convert.ToInt32(m[i]) << 8;
                if (k == 1) zi[l] = zi[l] + Convert.ToInt32(m[i]);

                if (k == 2) zq[l] =         Convert.ToInt32(m[i]) << 8;
                if (k == 3) zq[l] = zq[l] + Convert.ToInt32(m[i]);

                if (k != 3) k = k + 1;
                else
                {
                    k = 0;
                    l = l + 1;
                }
            }
            return (zi,zq);
        }

        // Calculate log(number) in the indicated log base.
        private double LogBase(double number, double log_base)
        {
            return Math.Log(number) / Math.Log(log_base);
        }

        private (double[],double[]) DATA_TO_DBL (int[]zi, int[]zq,uint len)
        {
            double[] mi = new double[len];
            double[] mq = new double[len];
            uint z;
            for (int i=0;i< len; i++)
            {
                if (zi[i] > 32767)//значит число отрицательное
                {
                    z = (uint)(zi[i]);
                    z = (~z) & 0xffff;
                    zi[i] = -1 * Convert.ToInt32(z + 1);
                }
                else zi[i] = Convert.ToInt32(zi[i]);//значит число положительное

                if (zq[i] > 32767)//значит число отрицательное
                {
                    z = (uint)(zq[i]);
                    z = (~z) & 0xffff;
                    zq[i] = -1 * Convert.ToInt32(z + 1);
                }
                else zq[i] = Convert.ToInt32(zq[i]);//значит число положительное

                mi[i] = Convert.ToDouble(zi[i]);
                mq[i] = Convert.ToDouble(zq[i]);
            }

            return (mi,mq);
        }

        void filtr_usr(double[] data, double[][] a, int k)//входные данные, входной зубчатый массив памяти (в нулевом массиве текущий массив данных )и глубина усреднения
        {
            int i;
            int j;
            double[] o = new double[data.Length];

            for (j = 0; j < data.Length; j++)
            {
                for (i = 0; i < k; i++) if (i < (k - 1)) a[k - i - 1][j] = a[k - i - 2][j]; else a[k - i - 1][j] = data[j];

                for (i = 0; i < k; i++) o[j] = (o[j] + a[i][j]);
            }
            for (j = 0; j < data.Length; j++) o[j] = o[j] / k;

            Array.Copy(o, data, data.Length);
        }

        (int n, double Amax) MAX_f(double[] m, uint N)
        {
            int i = 0;
            double max = 0;
            int k = 0;

            for (i = 0; i < N; i++)
            {
                if (max < m[i])
                {
                    max = m[i];
                    k = i;
                }
            }
            return (k, max);
        }

        private void FFT_WORK0 ()
        {
            for (int i = 0; i < 100; i++)
            {
                MeM0_0[i] = new double[FFT_SIZE_MAX];//формируем зубчатый массив памяти
                MeM0_1[i] = new double[FFT_SIZE_MAX];//формируем зубчатый массив памяти
            }

            while (true)
            {
                //lock (FFT_SIZE)
                {
                    uint SIZE = FFT_SIZE;

                    if (FLAG_FFT0 == 1)
                    {
                        double[] fft_array_x = new double[SIZE];
                        double[] fft_array_y = new double[SIZE];

                        DSPLib.DSP.Window.Type windowToApply = (DSPLib.DSP.Window.Type)Enum.Parse(typeof(DSPLib.DSP.Window.Type), selectedWindowName);
                        (fft_array_x, fft_array_y) = DATA_TO_DBL(DATA0_i, DATA0_q, SIZE);

                        // Apply window to the time series data
                        double[] wc = DSPLib.DSP.Window.Coefficients(windowToApply, SIZE);
                        double windowScaleFactor = DSPLib.DSP.Window.ScaleFactor.Signal(wc);
                        double[] windowedTimeSeries_i = DSPLib.DSP.Math.Multiply(fft_array_x, wc);
                        double[] windowedTimeSeries_q = DSPLib.DSP.Math.Multiply(fft_array_y, wc);

                        Array.Copy(windowedTimeSeries_i, data0.time_series, windowedTimeSeries_i.Length);//

                        FFTLibrary.Complex fft_z = new FFTLibrary.Complex();

                        int k = Convert.ToInt16(LogBase(SIZE, 2));//порядок БПФ
                        fft_z.FFT(1, k, windowedTimeSeries_i, windowedTimeSeries_q);

                        System.Numerics.Complex[] cpxResult = new System.Numerics.Complex[SIZE];//создаём массив комплексных чисел
                        for (int i = 0; i < SIZE; i++) cpxResult[i] = new System.Numerics.Complex(windowedTimeSeries_i[i], windowedTimeSeries_q[i]);

                        // Convert the complex result to a scalar magnitude 
                        double[] magResult = DSPLib.DSP.ConvertComplex.ToMagnitude(cpxResult);
                        magResult = DSPLib.DSP.Math.Multiply(magResult, 1);

                        // Convert and Plot Log Magnitude
                        double[] mag = DSPLib.DSP.ConvertComplex.ToMagnitude(cpxResult);
                        //          mag = DSPLib.DSP.Math.Multiply(mag, 1);
                        double[] magLog = DSPLib.DSP.ConvertMagnitude.ToMagnitudeDBV(mag);

                        double[] t = new double[SIZE];
                        double[] A = new double[SIZE];

                        for (int j = 0; j < SIZE; j++)
                        {
                            //	 A[j] = magLog[j];
                            if (j < (SIZE / 2)) A[j] = magLog[j + (SIZE / 2)];
                            if (j > ((SIZE / 2) - 1)) A[j] = magLog[j - (SIZE / 2)];
                            t[j] = -6000 + (12000 * j / (SIZE));//важно сначала умножить а потом поделить!!!! атоноль
                                                                    // Debug.WriteLine("t[]:"+t[j]);
                        }

                        for (int j = 0; j < SIZE; j++)
                        {
                            magLog[j] = A[SIZE - 1 - j];
                            if (magLog[j] < 0) magLog[j] = 0;
                        }

                        if (FLAG_filtr == 1) filtr_usr(magLog, MeM0_0, 10);
                        else
                        if (FLAG_filtr == 2) filtr_usr(magLog, MeM0_1, 30);

                        int k_max = 0;
                        double m1x, m1y;
                        double m2x, m2y;
                        double m3x, m3y;
                        double A_max = 0;
                        double B_max = 0;
                        double C_max = 0;
                        int step = 0;
                        int pstep = 0;
                        double[] m_sort = new double[SIZE];
                        int NFFT = Convert.ToInt32(SIZE);

                        Array.Copy(magLog, m_sort, SIZE);

                        (k_max, A_max) = MAX_f(magLog, SIZE);       //определяем Х координату первого максимума
                        m1x = t[k_max];
                        m1y = A_max;

                        if (NFFT > 2048) step = (NFFT / 80) - 20; else step = (NFFT / 80);

                        pstep = step / 2;

                        if (k_max > pstep && k_max < (SIZE - step)) for (int i = 0; i < step; i++) m_sort[k_max + i - pstep] = 0;

                        (k_max, B_max) = MAX_f(m_sort, SIZE);      //определяем Х координату второго максимума
                        m2x = t[k_max];
                        m2y = B_max;                                   //определяем второй максимум

                        if (k_max > pstep && k_max < (SIZE - step)) for (int i = 0; i < step; i++) m_sort[k_max + i - pstep] = 0;

                        (k_max, C_max) = MAX_f(m_sort, SIZE);

                        m3y = C_max;
                        m3x = t[k_max];

                        Array.Copy(magLog, data0.MAG_LOG, magLog.Length);
                        Array.Copy(t, data0.TSAMPL, t.Length);

                        data0.AMAX = A_max;
                        data0.BMAX = B_max;
                        data0.CMAX = C_max;
                        data0.M1X = m1x;
                        data0.M1Y = m1y;
                        data0.M2X = m2x;
                        data0.M2Y = m2y;
                        data0.M3X = m3x;
                        data0.M3Y = m3y;

                        data0.SCH_FFT_PKG0++;


                        FLAG_FFT0 = 0;//обработка завершена
                        FLAG_DISPAY0 = 1;                        
                    }

                }
                Thread.Sleep(0);
            }

        }


        private void FFT_WORK1()
        {
            for (int i = 0; i < 100; i++)
            {
                MeM1_0[i] = new double[FFT_SIZE_MAX];//формируем зубчатый массив памяти
                MeM1_1[i] = new double[FFT_SIZE_MAX];//формируем зубчатый массив памяти
            }

            while (true)
            {

                //lock (FFT_SIZE)
                {
                    uint SIZE = FFT_SIZE;
                    if (FLAG_FFT1 == 1)
                    {
                        double[] fft_array_x = new double[SIZE];
                        double[] fft_array_y = new double[SIZE];

                        DSPLib.DSP.Window.Type windowToApply = (DSPLib.DSP.Window.Type)Enum.Parse(typeof(DSPLib.DSP.Window.Type), selectedWindowName);
                        (fft_array_x, fft_array_y) = DATA_TO_DBL(DATA1_i, DATA1_q, SIZE);

                        // Apply window to the time series data
                        double[] wc = DSPLib.DSP.Window.Coefficients(windowToApply, SIZE);
                        double windowScaleFactor = DSPLib.DSP.Window.ScaleFactor.Signal(wc);
                        double[] windowedTimeSeries_i = DSPLib.DSP.Math.Multiply(fft_array_x, wc);
                        double[] windowedTimeSeries_q = DSPLib.DSP.Math.Multiply(fft_array_y, wc);

                        Array.Copy(windowedTimeSeries_i, data1.time_series, windowedTimeSeries_i.Length);//

                        FFTLibrary.Complex fft_z = new FFTLibrary.Complex();

                        int k = Convert.ToInt16(LogBase(SIZE, 2));//порядок БПФ
                        fft_z.FFT(1, k, windowedTimeSeries_i, windowedTimeSeries_q);

                        System.Numerics.Complex[] cpxResult = new System.Numerics.Complex[SIZE];//создаём массив комплексных чисел
                        for (int i = 0; i < SIZE; i++) cpxResult[i] = new System.Numerics.Complex(windowedTimeSeries_i[i], windowedTimeSeries_q[i]);

                        // Convert the complex result to a scalar magnitude 
                        double[] magResult = DSPLib.DSP.ConvertComplex.ToMagnitude(cpxResult);
                        magResult = DSPLib.DSP.Math.Multiply(magResult, 1);

                        // Convert and Plot Log Magnitude
                        double[] mag = DSPLib.DSP.ConvertComplex.ToMagnitude(cpxResult);
                        //          mag = DSPLib.DSP.Math.Multiply(mag, 1);
                        double[] magLog = DSPLib.DSP.ConvertMagnitude.ToMagnitudeDBV(mag);

                        double[] t = new double[SIZE];
                        double[] A = new double[SIZE];

                        for (int j = 0; j < SIZE; j++)
                        {
                            //	 A[j] = magLog[j];
                            if (j < (SIZE / 2)) A[j] = magLog[j + (SIZE / 2)];
                            if (j > ((SIZE / 2) - 1)) A[j] = magLog[j - (SIZE / 2)];
                            t[j] = -6000 + (12000 * j / (SIZE));//важно сначала умножить а потом поделить!!!! атоноль
                                                                    // Debug.WriteLine("t[]:"+t[j]);
                        }

                        for (int j = 0; j < SIZE; j++)
                        {
                            magLog[j] = A[SIZE - 1 - j];
                            if (magLog[j] < 0) magLog[j] = 0;
                        }

                        if (FLAG_filtr == 1) filtr_usr(magLog, MeM1_0, 10);
                        else
                        if (FLAG_filtr == 2) filtr_usr(magLog, MeM1_1, 30);

                        int k_max = 0;
                        double m1x, m1y;
                        double m2x, m2y;
                        double m3x, m3y;
                        double A_max = 0;
                        double B_max = 0;
                        double C_max = 0;
                        int step = 0;
                        int pstep = 0;
                        double[] m_sort = new double[SIZE];
                        int NFFT = Convert.ToInt32(SIZE);

                        Array.Copy(magLog, m_sort, SIZE);

                        (k_max, A_max) = MAX_f(magLog, SIZE);       //определяем Х координату первого максимума
                        m1x = t[k_max];
                        m1y = A_max;

                        if (NFFT > 2048) step = (NFFT / 80) - 20; else step = (NFFT / 80);

                        pstep = step / 2;

                        if (k_max > pstep && k_max < (SIZE - step)) for (int i = 0; i < step; i++) m_sort[k_max + i - pstep] = 0;

                        (k_max, B_max) = MAX_f(m_sort, SIZE);      //определяем Х координату второго максимума
                        m2x = t[k_max];
                        m2y = B_max;                                   //определяем второй максимум

                        if (k_max > pstep && k_max < (SIZE - step)) for (int i = 0; i < step; i++) m_sort[k_max + i - pstep] = 0;

                        (k_max, C_max) = MAX_f(m_sort, SIZE);

                        m3y = C_max;
                        m3x = t[k_max];

                        Array.Copy(magLog, data1.MAG_LOG, magLog.Length);
                        Array.Copy(t, data1.TSAMPL, t.Length);

                        data1.AMAX = A_max;
                        data1.BMAX = B_max;
                        data1.CMAX = C_max;
                        data1.M1X = m1x;
                        data1.M1Y = m1y;
                        data1.M2X = m2x;
                        data1.M2Y = m2y;
                        data1.M3X = m3x;
                        data1.M3Y = m3y;

                        data1.SCH_FFT_PKG0++;


                        FLAG_FFT1 = 0;//обработка завершена
                        FLAG_DISPAY1 = 1;

                    }

                }
                Thread.Sleep(0);
            }

        }


    }
}
