using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Windows;

namespace Stnd_072
{
   public class UDP_server
    {
        struct Command
        {
            public UInt32 Cmd_size;
            public UInt32 Cmd_type;
            public UInt64 Cmd_id;
            public UInt64 Cmd_time;
            public string Cmd_data;
            public byte[] A;
        }
        struct Message
        {
            public UInt32 Msg_size;
            public UInt32 Msg_type;
            public UInt64 Num_cmd_in_msg;
            public Command CMD;
        }
        struct Frame
        {
            public UInt16 Frame_size;
            public UInt16 Frame_number;
            public UInt16 Stop_bit;
            public UInt32 Msg_uniq_id;
            public UInt64 Sender_id;
            public UInt64 Receiver_id;
            public Message MSG;
        }

        Frame MSG1 = new Frame();

        Byte[] Rcv_buffer0 = new byte[64000];
        Byte[] Rcv_buffer1 = new byte[64000];

        public bool FLAG_MSG_RCV = false;
        UdpClient _server = null;
        IPEndPoint _client = null;
        Config cfg = null;
        STATUS_b072 b072 = null;

        public UDP_server(string ip,string port,Config cfg, STATUS_b072 b072)//свои адрес и порт!
        {           
            
           try
            {
                IPEndPoint serverEnd = new IPEndPoint(IPAddress.Parse(ip), UInt16.Parse(port));
                _server = new UdpClient(serverEnd);
                _server.Client.ReceiveBufferSize = 8192 * 200;//увеличиваем размер приёмного буфера!!!

                //Start listening.

                Thread listenThread = new Thread(new ThreadStart(Listening));
                listenThread.Start();

                //Change state to indicate the server starts.
                this.cfg = cfg;
                this.b072 = b072;
                Debug.WriteLine("Waiting for a client...");
            }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message);
               MessageBox.Show("Нет абонента!");
           }
        }

        private void Listening()
        {
            //Listening loop.
            try
            {
                while (true)
                {
                    //receieve a message form a client.
                    byte[] data = _server.Receive(ref _client);
                  //  Debug.WriteLine("UDP rcv");
                    Array.Copy(data, Rcv_buffer0, data.Length);//копируем массив отсчётов в форму обработки   
                    UDP_BUF_DESCRIPT(Rcv_buffer0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void UDP_BUF_DESCRIPT(Byte[] RCV)
        {
            int i = 0;
            int j = 0;
            int offset = 0;
            int tmp = 0;
            byte[] a = new byte[4];

            FLAG_MSG_RCV = false;

            MSG1.MSG.CMD.A = new byte[256];//массив с принимаемыми данными
      //      Debug.WriteLine("------------------");

            MSG1.Frame_size = Convert.ToUInt16((RCV[0 + tmp] << 8) + RCV[1 + tmp]);
            MSG1.Frame_number = Convert.ToUInt16((RCV[2 + tmp] << 8) + RCV[3 + tmp]);
            MSG1.Stop_bit = Convert.ToUInt16(1);
            MSG1.Msg_uniq_id = Convert.ToUInt32((RCV[4 + tmp] << 24) + (RCV[5 + tmp] << 16) + (RCV[6 + tmp] << 8) + (RCV[7 + tmp] << 0));
            MSG1.Sender_id = Convert.ToUInt64((RCV[8 + tmp] << 56) + (RCV[9 + tmp] << 48) + (RCV[10 + tmp] << 40) + (RCV[11 + tmp] << 32) + (RCV[12 + tmp] << 24) + (RCV[13 + tmp] << 16) + (RCV[14 + tmp] << 8) + (RCV[15 + tmp] << 0));
            MSG1.Receiver_id = Convert.ToUInt64((RCV[16] << 56) + (RCV[17] << 48) + (RCV[18] << 40) + (RCV[19] << 32) + (RCV[20] << 24) + (RCV[21] << 16) + (RCV[22] << 8) + (RCV[23] << 0));
            MSG1.MSG.Msg_size = Convert.ToUInt32((RCV[24] << 24) + (RCV[25] << 16) + (RCV[26] << 8) + (RCV[27] << 0));
            MSG1.MSG.Msg_type = Convert.ToUInt32((RCV[28] << 24) + (RCV[29] << 16) + (RCV[30] << 8) + (RCV[31] << 0));
            MSG1.MSG.Num_cmd_in_msg = Convert.ToUInt64((RCV[32] << 56) + (RCV[33] << 48) + (RCV[34] << 40) + (RCV[35] << 32) + (RCV[36] << 24) + (RCV[37] << 16) + (RCV[38] << 8) + (RCV[39] << 0));

            /*
            Debug.WriteLine("    Frame_size:" + MSG1.Frame_size);
            Debug.WriteLine("  Frame_number:" + MSG1.Frame_number);
            Debug.WriteLine("   Msg_uniq_id:" + MSG1.Msg_uniq_id);
            Debug.WriteLine("     Sender_id:" + MSG1.Sender_id);
            Debug.WriteLine("   Receiver_id:" + MSG1.Receiver_id);
            Debug.WriteLine("      Msg_size:" + MSG1.MSG.Msg_size);
            Debug.WriteLine("      Msg_type:" + MSG1.MSG.Msg_type);
            Debug.WriteLine("Num_cmd_in_msg:" + MSG1.MSG.Num_cmd_in_msg);
            */
            offset = 40;

            for (i = 0; i < Convert.ToInt32(MSG1.MSG.Num_cmd_in_msg); i++)
            {
                MSG1.MSG.CMD.Cmd_size = Convert.ToUInt32((RCV[offset + 0]  << 24) + (RCV[offset + 1] << 16) + (RCV[offset + 2] << 8) + (RCV[offset + 3] << 0));
                MSG1.MSG.CMD.Cmd_type = Convert.ToUInt32((RCV[offset + 4]  << 24) + (RCV[offset + 5] << 16) + (RCV[offset + 6] << 8) + (RCV[offset + 7] << 0));
                MSG1.MSG.CMD.Cmd_id   = Convert.ToUInt64((RCV[offset + 8]  << 56) + (RCV[offset + 9] << 48) + (RCV[offset + 10] << 40) + (RCV[offset + 11] << 32) + (RCV[offset + 12] << 24) + (RCV[offset + 13] << 16) + (RCV[offset + 14] << 8) + (RCV[offset + 15] << 0));
                MSG1.MSG.CMD.Cmd_time = Convert.ToUInt64((RCV[offset + 16] << 56) + (RCV[offset + 17] << 48) + (RCV[offset + 18] << 40) + (RCV[offset + 19] << 32) + (RCV[offset + 20] << 24) + (RCV[offset + 21] << 16) + (RCV[offset + 22] << 8) + (RCV[offset + 23] << 0));

                if (MSG1.MSG.CMD.Cmd_type == Convert.ToInt32(cfg.MSG_CMD_OK))
                {
                    //  FLAG_TIMER_1 = 0;
                //    Debug.WriteLine("Принята команда MSG_CMD_OK!");
                }


                if (MSG1.MSG.CMD.Cmd_type == Convert.ToInt32(cfg.MSG_STATUS_OK))
                {
                  //FLAG_TIMER_1 = 0;
                  //Debug.WriteLine("Принята команда MSG_STATUS_OK!");
                }
                /*
                Debug.WriteLine("Cmd_size:" + MSG1.MSG.CMD.Cmd_size);
                Debug.WriteLine("Cmd_type:" + MSG1.MSG.CMD.Cmd_type);
                Debug.WriteLine("Cmd_id  :" + MSG1.MSG.CMD.Cmd_id);
                Debug.WriteLine("Cmd_time:" + MSG1.MSG.CMD.Cmd_time);
                */
                //---------------------------------------------------------------------------------

                for (j = 0; j < Convert.ToInt32(MSG1.MSG.CMD.Cmd_size); j++)//заполняем массив принятыми данными - DATA
                {
                    MSG1.MSG.CMD.Cmd_data = Convert.ToString(RCV[offset + 24 + j]);
                    MSG1.MSG.CMD.A[j] = RCV[offset + 24 + j];
                }

                if (MSG1.MSG.CMD.Cmd_type== Convert.ToInt32(cfg.MSG_STATUS_OK))
                {
                    int n = 0;

                    STATUS_b072.timer_obmen = 0;//сбрасываем таймер контроля длительности обмена

                    b072.DAC0.dac_pll_locked    = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.ALARM_ERROR       = (MSG1.MSG.CMD.A[n++]<<8)+(MSG1.MSG.CMD.A[n++]);
                    b072.DAC0.SYNC_N_ERROR      = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.INIT              = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.alarms_from_lanes = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.memin_pll_lfvolt  = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.alarm_rw0_pll     = MSG1.MSG.CMD.A[n++]; 
                    b072.DAC0.alarm_sysref_err  = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.alarm_l_error_0   = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.alarm_fifo_flags_0= MSG1.MSG.CMD.A[n++]; 
                    b072.DAC0.alarm_l_error_1   = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.alarm_fifo_flags_1= MSG1.MSG.CMD.A[n++];
                    b072.DAC0.error_count_link0 = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.DAC0.error_count_link1 = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.DAC0.D_ALARM           = MSG1.MSG.CMD.A[n++];
                    b072.DAC0.TEMP              = MSG1.MSG.CMD.A[n++];
                    /*
                    Debug.WriteLine("");
                    Debug.WriteLine("DAC0.dac_pll_locked     :" + b072.DAC0.dac_pll_locked);
                    Debug.WriteLine("DAC0.ALARM_ERROR        :" + b072.DAC0.ALARM_ERROR);
                    Debug.WriteLine("DAC0.SYNC_N_ERROR       :" + b072.DAC0.SYNC_N_ERROR);
                    Debug.WriteLine("DAC0.INIT               :" + b072.DAC0.INIT);
                    Debug.WriteLine("DAC0.alarms_from_lanes  :" + b072.DAC0.alarms_from_lanes);
                    Debug.WriteLine("DAC0.memin_pll_lfvolt   :" + b072.DAC0.memin_pll_lfvolt);
                    Debug.WriteLine("DAC0.alarm_rw0_pll      :" + b072.DAC0.alarm_rw0_pll);
                    Debug.WriteLine("DAC0.alarm_sysref_err   :" + b072.DAC0.alarm_sysref_err);
                    Debug.WriteLine("DAC0.alarm_l_error_0    :" + b072.DAC0.alarm_l_error_0);
                    Debug.WriteLine("DAC0.alarm_fifo_flags_0 :" + b072.DAC0.alarm_fifo_flags_0);
                    Debug.WriteLine("DAC0.alarm_l_error_1    :" + b072.DAC0.alarm_l_error_1);
                    Debug.WriteLine("DAC0.alarm_fifo_flags_1 :" + b072.DAC0.alarm_fifo_flags_1);
                    Debug.WriteLine("DAC0.error_count_link0  :" + b072.DAC0.error_count_link0);
                    Debug.WriteLine("DAC0.error_count_link1  :" + b072.DAC0.error_count_link1);
                    Debug.WriteLine("");
                    */
                    b072.DAC1.dac_pll_locked     = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.ALARM_ERROR        = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.DAC1.SYNC_N_ERROR       = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.INIT               = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.alarms_from_lanes  = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.memin_pll_lfvolt   = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.alarm_rw0_pll      = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.alarm_sysref_err   = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.alarm_l_error_0    = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.alarm_fifo_flags_0 = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.alarm_l_error_1    = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.alarm_fifo_flags_1 = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.error_count_link0  = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.DAC1.error_count_link1  = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.DAC1.D_ALARM            = MSG1.MSG.CMD.A[n++];
                    b072.DAC1.TEMP               = MSG1.MSG.CMD.A[n++];
                    /*
                    Debug.WriteLine("");
                    Debug.WriteLine("DAC1.dac_pll_locked     :" + b072.DAC1.dac_pll_locked);
                    Debug.WriteLine("DAC1.ALARM_ERROR        :" + b072.DAC1.ALARM_ERROR);
                    Debug.WriteLine("DAC1.SYNC_N_ERROR       :" + b072.DAC1.SYNC_N_ERROR);
                    Debug.WriteLine("DAC1.INIT               :" + b072.DAC1.INIT);
                    Debug.WriteLine("DAC1.alarms_from_lanes  :" + b072.DAC1.alarms_from_lanes);
                    Debug.WriteLine("DAC1.memin_pll_lfvolt   :" + b072.DAC1.memin_pll_lfvolt);
                    Debug.WriteLine("DAC1.alarm_rw0_pll      :" + b072.DAC1.alarm_rw0_pll);
                    Debug.WriteLine("DAC1.alarm_sysref_err   :" + b072.DAC1.alarm_sysref_err);
                    Debug.WriteLine("DAC1.alarm_l_error_0    :" + b072.DAC1.alarm_l_error_0);
                    Debug.WriteLine("DAC1.alarm_fifo_flags_0 :" + b072.DAC1.alarm_fifo_flags_0);
                    Debug.WriteLine("DAC1.alarm_l_error_1    :" + b072.DAC1.alarm_l_error_1);
                    Debug.WriteLine("DAC1.alarm_fifo_flags_1 :" + b072.DAC1.alarm_fifo_flags_1);
                    Debug.WriteLine("DAC1.error_count_link0  :" + b072.DAC1.error_count_link0);
                    Debug.WriteLine("DAC1.error_count_link1  :" + b072.DAC1.error_count_link1);
                    Debug.WriteLine("");
                    */

                    b072.ADC0.INIT               = MSG1.MSG.CMD.A[n++];
                    b072.ADC0.rx_datak_adc       = MSG1.MSG.CMD.A[n++];
                    b072.ADC0.rx_errdetect_adc   = MSG1.MSG.CMD.A[n++]; 
                    b072.ADC0.align_ok_adc       = MSG1.MSG.CMD.A[n++]; 
                    b072.ADC0.rx_ready_adc       = MSG1.MSG.CMD.A[n++];
                    b072.ADC0.sync_n_adc         = MSG1.MSG.CMD.A[n++]; 
                    b072.ADC0.rx_syncstatus_adc  = MSG1.MSG.CMD.A[n++];
                    b072.ADC0.align_adc          = MSG1.MSG.CMD.A[n++]; 
                    b072.ADC0.error_adc          = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.ADC0.error_sysref_adc   = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    /*
                    Debug.WriteLine("");
                    Debug.WriteLine("ADC0.INIT               :" + b072.ADC0.INIT);
                    Debug.WriteLine("ADC0.rx_datak_adc       :" + b072.ADC0.rx_datak_adc);
                    Debug.WriteLine("ADC0.rx_errdetect_adc   :" + b072.ADC0.rx_errdetect_adc);
                    Debug.WriteLine("ADC0.align_ok_adc       :" + b072.ADC0.align_ok_adc.ToString("X"));
                    Debug.WriteLine("ADC0.rx_ready_adc       :" + b072.ADC0.rx_ready_adc.ToString("X"));
                    Debug.WriteLine("ADC0.sync_n_adc         :" + b072.ADC0.sync_n_adc);
                    Debug.WriteLine("ADC0.rx_syncstatus_adc  :" + b072.ADC0.rx_syncstatus_adc.ToString("X"));
                    Debug.WriteLine("ADC0.align_adc          :" + b072.ADC0.align_adc.ToString("X"));
                    Debug.WriteLine("ADC0.error_adc          :" + b072.ADC0.error_adc);
                    Debug.WriteLine("ADC0.error_sysref_adc   :" + b072.ADC0.error_sysref_adc);
                    Debug.WriteLine("");
                    */
                    b072.ADC1.INIT               = MSG1.MSG.CMD.A[n++];
                    b072.ADC1.rx_datak_adc       = MSG1.MSG.CMD.A[n++];
                    b072.ADC1.rx_errdetect_adc   = MSG1.MSG.CMD.A[n++];
                    b072.ADC1.align_ok_adc       = MSG1.MSG.CMD.A[n++];
                    b072.ADC1.rx_ready_adc       = MSG1.MSG.CMD.A[n++];
                    b072.ADC1.sync_n_adc         = MSG1.MSG.CMD.A[n++];
                    b072.ADC1.rx_syncstatus_adc  = MSG1.MSG.CMD.A[n++];
                    b072.ADC1.align_adc          = MSG1.MSG.CMD.A[n++];
                    b072.ADC1.error_adc          = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.ADC1.error_sysref_adc   = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    /*
                    Debug.WriteLine("");
                    Debug.WriteLine("ADC1.INIT               :" + b072.ADC1.INIT);
                    Debug.WriteLine("ADC1.rx_datak_adc       :" + b072.ADC1.rx_datak_adc.ToString("X"));
                    Debug.WriteLine("ADC1.rx_errdetect_adc   :" + b072.ADC1.rx_errdetect_adc);
                    Debug.WriteLine("ADC1.align_ok_adc       :" + b072.ADC1.align_ok_adc.ToString("X"));
                    Debug.WriteLine("ADC1.rx_ready_adc       :" + b072.ADC1.rx_ready_adc);
                    Debug.WriteLine("ADC1.sync_n_adc         :" + b072.ADC1.sync_n_adc);
                    Debug.WriteLine("ADC1.rx_syncstatus_adc  :" + b072.ADC1.rx_syncstatus_adc.ToString("X"));
                    Debug.WriteLine("ADC1.align_adc          :" + b072.ADC1.align_adc.ToString("X"));
                    Debug.WriteLine("ADC1.error_adc          :" + b072.ADC1.error_adc);
                    Debug.WriteLine("ADC1.error_sysref_adc   :" + b072.ADC1.error_sysref_adc);
                    Debug.WriteLine("");
                    */
                    b072.LMK.INIT                =  MSG1.MSG.CMD.A[n++];
                    b072.LMK.lb                  = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.LMK.RB_DAC_VALUE        = (MSG1.MSG.CMD.A[n++] << 8) + (MSG1.MSG.CMD.A[n++]);
                    b072.LMK.PLL2_LD             =  MSG1.MSG.CMD.A[n++];
                    b072.LMK.PLL1_LD             =  MSG1.MSG.CMD.A[n++];
                    b072.LMK.STATUS_LD1          =  MSG1.MSG.CMD.A[n++];
                    b072.LMK.STATUS_LD2          =  MSG1.MSG.CMD.A[n++];

                    /*
                    Debug.WriteLine("");
                    Debug.WriteLine("LMK.lb       :" + b072.LMK.lb.ToString("X"));
                    Debug.WriteLine("RB_DAC_VALUE :" + b072.LMK.RB_DAC_VALUE.ToString("X"));
                    Debug.WriteLine("LMK.PLL2_LD  :" + b072.LMK.PLL2_LD);
                    Debug.WriteLine("LMK.PLL1_LD  :" + b072.LMK.PLL1_LD);
                    Debug.WriteLine("");
                    */
                    b072.FPGA.INIT               = MSG1.MSG.CMD.A[n++];
                    b072.FPGA.TEMP               = MSG1.MSG.CMD.A[n++];
                    b072.FPGA.STATUS_REF         = MSG1.MSG.CMD.A[n++];
                    b072.FPGA.STATUS_SYNC        = MSG1.MSG.CMD.A[n++];
                    b072.FPGA.STATUS_1HZ         = MSG1.MSG.CMD.A[n++];

                    b072.FPGA.ERROR_1HZ          = (MSG1.MSG.CMD.A[n++] << 24) + 
                                                   (MSG1.MSG.CMD.A[n++] << 16) + 
                                                   (MSG1.MSG.CMD.A[n++] <<  8) + 
                                                   (MSG1.MSG.CMD.A[n++] <<  0);

                    b072.FPGA.SYNC0_MIN          = (MSG1.MSG.CMD.A[n++] << 24) +
                                                   (MSG1.MSG.CMD.A[n++] << 16) +
                                                   (MSG1.MSG.CMD.A[n++] << 8) +
                                                   (MSG1.MSG.CMD.A[n++] << 0);

                    b072.FPGA.SYNC0_MAX          = (MSG1.MSG.CMD.A[n++] << 24) +
                                                   (MSG1.MSG.CMD.A[n++] << 16) +
                                                   (MSG1.MSG.CMD.A[n++] << 8) +
                                                   (MSG1.MSG.CMD.A[n++] << 0);

                    b072.FPGA.SYNC1_MIN          = (MSG1.MSG.CMD.A[n++] << 24) +
                                                   (MSG1.MSG.CMD.A[n++] << 16) +
                                                   (MSG1.MSG.CMD.A[n++] << 8) +
                                                   (MSG1.MSG.CMD.A[n++] << 0);

                    b072.FPGA.SYNC1_MAX          = (MSG1.MSG.CMD.A[n++] << 24) +
                                                   (MSG1.MSG.CMD.A[n++] << 16) +
                                                   (MSG1.MSG.CMD.A[n++] << 8) +
                                                   (MSG1.MSG.CMD.A[n++] << 0);

                    b072.FPGA.SYNC2_MIN          = (MSG1.MSG.CMD.A[n++] << 24) +
                                                   (MSG1.MSG.CMD.A[n++] << 16) +
                                                   (MSG1.MSG.CMD.A[n++] << 8) +
                                                   (MSG1.MSG.CMD.A[n++] << 0);

                    b072.FPGA.SYNC2_MAX          = (MSG1.MSG.CMD.A[n++] << 24) +
                                                   (MSG1.MSG.CMD.A[n++] << 16) +
                                                   (MSG1.MSG.CMD.A[n++] << 8) +
                                                   (MSG1.MSG.CMD.A[n++] << 0);

                    b072.BRD.INIT                = MSG1.MSG.CMD.A[n++];
                    b072.BRD.TEMP0               = MSG1.MSG.CMD.A[n++];
                    b072.BRD.TEMP1               = MSG1.MSG.CMD.A[n++];
                    b072.BRD.BRD_NUMBER          = MSG1.MSG.CMD.A[n++];

                    /*
                    Debug.WriteLine("b072.BRD.INIT      :" + b072.BRD.INIT);
                    Debug.WriteLine("b072.BRD.TEMP0     :" + b072.BRD.TEMP0);
                    Debug.WriteLine("b072.BRD.TEMP1     :" + b072.BRD.TEMP1);
                    Debug.WriteLine("b072.BRD.BRD_NUMBER:" + b072.BRD.BRD_NUMBER);
                    
                     Debug.WriteLine("");
                     Debug.WriteLine("b072.DAC0.D_ALARM :" + b072.DAC0.D_ALARM);
                     Debug.WriteLine("b072.DAC1.D_ALARM :" + b072.DAC1.D_ALARM);
                     Debug.WriteLine("b072.LMK.STATUS_LD1 :" + b072.LMK.STATUS_LD1);
                     Debug.WriteLine("b072.LMK.STATUS_LD2 :" + b072.LMK.STATUS_LD2);
                     Debug.WriteLine("BRD.BRD_NUMBER :" + b072.BRD.BRD_NUMBER);
                     */
                }

          

                offset = offset + 24 + j;
            }

            FLAG_MSG_RCV = true;
        }

    }
}
