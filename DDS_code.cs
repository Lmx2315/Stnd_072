using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stnd_072
{
    public class DDS_code
    {
        static UInt32 FREQ_TIMER  = 48; //частота тактов системного времени ПЛИС 48 МГЦ

        static UInt32 FREQ_DDS   = 96_000_000; //частота тактов DDS в ПЛИС 48 МГЦ

        static byte   Accum_base = 48; //разрядность аакумулятора DDS

        public UInt64 zTIME { get; set; }           //время срабатывания данной команды	
        public UInt64 zFREQ { get; set; }           //Частота DDS в плис (0-10 МГц) код     (freq/96_000_000)*2^48
        public  Int64 zFREQ_STEP { get; set; }      //шаг приращения частоты DDS в плис код (freq/96_000_000)*2^48
        public UInt32 zFREQ_RATE { get; set; }      //временной шаг приращения частоты, причём: 0 - 1/96 МГц; 1 - 2/96 МГц
        public UInt16 zN_impulse { get; set; }      //число импульсов
        public byte zTYPE_impulse { get; set; }     //не когерентная пачка (лчм перезапускается в каждом импульсе)	[1][0]=10 - не когерентная и цифруем интервал приёма!
        public UInt32 zInterval_Ti { get; set; }    //интервал времени излучения N*(1/48 МГц)
        public UInt32 zInterval_Tp { get; set; }    //интервал времени приёма    N*(1/48 МГц)
        public UInt32 zTblank1 { get; set; }        //интервал времени перед излучением N*(1/48 МГц)
        public UInt32 zTblank2 { get; set; }        //интервал времени перед приёмом    N*(1/48 МГц)

        public int zAmp0 { get; set; } //Уровень сигнала в 0-м канале синтезатора
        public int zAmp1 { get; set; } //Уровень сигнала в 0-м канале синтезатора
        public int zAmp2 { get; set; } //Уровень сигнала в 0-м канале синтезатора
        public int zAmp3 { get; set; } //Уровень сигнала в 0-м канале синтезатора

        public int zPhase0 { get; set; } //начальная фаза в 0-м канале синтезатора
        public int zPhase1 { get; set; } //начальная фаза в 1-м канале синтезатора
        public int zPhase2 { get; set; } //начальная фаза в 2-м канале синтезатора
        public int zPhase3 { get; set; } //начальная фаза в 3-м канале синтезатора

        public void TIME (UInt64 t)//пока напрямую переводит микросекунды в код
        {
            UInt64 time_sec;
            UInt64 code;
            time_sec = t;
            code = time_sec * FREQ_TIMER; //
            zTIME = code;
        }

        public void FREQ (double freq)//переводим частоту в Гц в код
        {
            UInt64 z;
            Console.WriteLine("freq:"+ freq);
            z = (UInt64)(((freq-430000000) / FREQ_DDS )* ((ulong)(Math.Pow(2,Accum_base))));
            zFREQ = z;
        }

        public void FREQ_STEP(double freq)//переводим частоту шага приращения Гц в код (частотный шаг может быть отрицательным для получения отрицательной девиации)
        {
            Int64 z;
            z = (Int64)(freq / FREQ_DDS * (Math.Pow(2, Accum_base)));
            zFREQ_STEP = z;
        }

        public void FREQ_RATE(UInt32 t)//переводим время приращения частоты из нс в код
        {
            UInt64 time_sec;
            UInt64 code;
            time_sec = Convert.ToUInt64(t);
            code = time_sec / FREQ_TIMER; //
            zFREQ_RATE = (uint)code;
        }

        public void N_impulse(UInt32 t)//количество однотипных интервалов излучения и приёма в одной команде
        {
            zN_impulse = (ushort)t;
        }

        public void TYPE(UInt32 t)//количество однотипных интервалов излучения и приёма в одной команде
        {
            zTYPE_impulse = (byte)t;
        }

        public void Interval_Ti(UInt32 t)//переводим время приращения частоты из нс в код
        {
            UInt64 time_sec;
            UInt64 code;
            time_sec = Convert.ToUInt64(t);
            code = time_sec * FREQ_TIMER; //
            zInterval_Ti = (uint)code;
        }

        public void Interval_Tp(UInt32 t)//переводим время приращения частоты из нс в код
        {
            UInt64 time_sec;
            UInt64 code;
            time_sec = Convert.ToUInt64(t);
            code = time_sec * FREQ_TIMER; //
            zInterval_Tp = (uint)code;
        }

        public void Tblank1(UInt32 t)//переводим время приращения частоты из нс в код
        {
            UInt64 time_sec;
            UInt64 code;
            time_sec = Convert.ToUInt64(t);
            code = time_sec * FREQ_TIMER; //
            zTblank1 = (uint)code;
        }

        public void Tblank2(UInt32 t)//переводим время приращения частоты из нс в код
        {
            UInt64 time_sec;
            UInt64 code;
            time_sec = Convert.ToUInt64(t);
            code = time_sec * FREQ_TIMER; //
            zTblank2 = (uint)code;
        }

        public void Amp0(int a)
        {
            double z = Math.Round((a / 100.0)*1000);
            zAmp0 = (int)z;
        }

        public void Amp1(int a)
        {
            double z = Math.Round((a / 100.0) * 1000);
            zAmp1 = (int)z;
        }

        public void Amp2(int a)
        {
            double z = Math.Round((a / 100.0) * 1000);
            zAmp2 = (int)z;
        }

        public void Amp3(int a)
        {
            double z = Math.Round((a / 100.0) * 1000);
            zAmp3 = (int)z;
        }

        public void Phase0(int a)
        {
            double h16 = (Math.Pow(2,16))/360;//цена деления фазы
            double z = Math.Round(a*h16);
            zPhase0 = (int)z;
        }

        public void Phase1(int a)
        {
            double h16 = (Math.Pow(2, 16)) / 360;//цена деления фазы
            double z = Math.Round(a * h16);
            zPhase1 = (int)z;
        }

        public void Phase2(int a)
        {
            double h16 = (Math.Pow(2, 16)) / 360;//цена деления фазы
            double z = Math.Round(a * h16);
            zPhase2 = (int)z;
        }

        public void Phase3(int a)
        {
            double h16 = (Math.Pow(2, 16)) / 360;//цена деления фазы
            double z = Math.Round(a * h16);
            zPhase3 = (int)z;
        }
    }
}
