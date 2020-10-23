using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Stnd_072
{
   public class UDP_sender
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

        public enum CMD:byte
        {
            CMD_TIME_SETUP = 1,
            CMD_STATUS = 100,
            CMD_ATT = 72
        }

        Frame FRAME;

        static UInt32 CMD_ID;

        string ip_dest;
        int port_dest;

        public UDP_sender (string ip,string port)
        {
             ip_dest = ip;
           port_dest = Convert.ToInt32(port);
        }

        public void UDP_SEND(CMD CMD, byte[] CMD_data, uint CMD_size, ulong CMD_time)
        {
            byte[] UDP_packet = new byte[1440];
            int DATA_lenght = 0;
            int i = 0;
            byte CMD_type = Convert.ToByte(CMD);

            UInt64 sch_cmd = 0;
            try
            {
                FRAME.Frame_size = 0;
                FRAME.Frame_number = 0;
                FRAME.Stop_bit = 1;
                FRAME.Msg_uniq_id = 1;
                FRAME.Sender_id = 1;
                FRAME.Receiver_id = 2;
                FRAME.MSG.Msg_size = 10;
                FRAME.MSG.Msg_type = 1;
                FRAME.MSG.Num_cmd_in_msg = 1;
                sch_cmd = 1;//считаем число команд в файле

                //-------------------фреймовая часть пакета-----------------------
                UDP_packet[0] = Convert.ToByte((FRAME.Frame_size >> 8) & 0xff);
                UDP_packet[1] = Convert.ToByte((FRAME.Frame_size >> 0) & 0xff);

                UDP_packet[2] = Convert.ToByte((FRAME.Frame_number >> 8) & 0xff);
                UDP_packet[3] = Convert.ToByte((FRAME.Frame_number >> 0) & 0xff);//

                UDP_packet[4] = Convert.ToByte((FRAME.Msg_uniq_id >> 24) & 0xff);
                UDP_packet[5] = Convert.ToByte((FRAME.Msg_uniq_id >> 16) & 0xff);
                UDP_packet[6] = Convert.ToByte((FRAME.Msg_uniq_id >> 8) & 0xff);
                UDP_packet[7] = Convert.ToByte((FRAME.Msg_uniq_id >> 0) & 0xff);

                UDP_packet[8] = Convert.ToByte((FRAME.Sender_id >> 56) & 0xff);
                UDP_packet[9] = Convert.ToByte((FRAME.Sender_id >> 48) & 0xff);
                UDP_packet[10] = Convert.ToByte((FRAME.Sender_id >> 40) & 0xff);
                UDP_packet[11] = Convert.ToByte((FRAME.Sender_id >> 32) & 0xff);
                UDP_packet[12] = Convert.ToByte((FRAME.Sender_id >> 24) & 0xff);
                UDP_packet[13] = Convert.ToByte((FRAME.Sender_id >> 16) & 0xff);
                UDP_packet[14] = Convert.ToByte((FRAME.Sender_id >> 8) & 0xff);
                UDP_packet[15] = Convert.ToByte((FRAME.Sender_id >> 0) & 0xff);

                UDP_packet[16] = Convert.ToByte((FRAME.Receiver_id >> 56) & 0xff);
                UDP_packet[17] = Convert.ToByte((FRAME.Receiver_id >> 48) & 0xff);
                UDP_packet[18] = Convert.ToByte((FRAME.Receiver_id >> 40) & 0xff);
                UDP_packet[19] = Convert.ToByte((FRAME.Receiver_id >> 32) & 0xff);
                UDP_packet[20] = Convert.ToByte((FRAME.Receiver_id >> 24) & 0xff);
                UDP_packet[21] = Convert.ToByte((FRAME.Receiver_id >> 16) & 0xff);
                UDP_packet[22] = Convert.ToByte((FRAME.Receiver_id >> 8) & 0xff);
                UDP_packet[23] = Convert.ToByte((FRAME.Receiver_id >> 0) & 0xff);

                UDP_packet[24] = Convert.ToByte((FRAME.MSG.Msg_size >> 24) & 0xff);
                UDP_packet[25] = Convert.ToByte((FRAME.MSG.Msg_size >> 16) & 0xff);
                UDP_packet[26] = Convert.ToByte((FRAME.MSG.Msg_size >> 8) & 0xff);
                UDP_packet[27] = Convert.ToByte((FRAME.MSG.Msg_size >> 0) & 0xff);

                UDP_packet[28] = Convert.ToByte((FRAME.MSG.Msg_type >> 24) & 0xff);
                UDP_packet[29] = Convert.ToByte((FRAME.MSG.Msg_type >> 16) & 0xff);
                UDP_packet[30] = Convert.ToByte((FRAME.MSG.Msg_type >> 8) & 0xff);
                UDP_packet[31] = Convert.ToByte((FRAME.MSG.Msg_type >> 0) & 0xff);

                UDP_packet[32] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 56) & 0xff);
                UDP_packet[33] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 48) & 0xff);
                UDP_packet[34] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 40) & 0xff);
                UDP_packet[35] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 32) & 0xff);
                UDP_packet[36] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 24) & 0xff);
                UDP_packet[37] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 16) & 0xff);
                UDP_packet[38] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 8) & 0xff);
                UDP_packet[39] = Convert.ToByte((FRAME.MSG.Num_cmd_in_msg >> 0) & 0xff);
                //-----------------------------------------------------------------------------------------
                int j = 0;
                DATA_lenght = 40;//это число байт из упаковки выше 39+1
                while (sch_cmd > 0)
                {
                    FRAME.MSG.CMD.Cmd_size = Convert.ToUInt16(1 + CMD_data.Length);
                    FRAME.MSG.CMD.Cmd_id = CMD_ID++;

                    UDP_packet[40 + j] = Convert.ToByte((CMD_size >> 24) & 0xff);
                    UDP_packet[41 + j] = Convert.ToByte((CMD_size >> 16) & 0xff);
                    UDP_packet[42 + j] = Convert.ToByte((CMD_size >> 8) & 0xff);
                    UDP_packet[43 + j] = Convert.ToByte((CMD_size >> 0) & 0xff);

                    UDP_packet[44 + j] = Convert.ToByte((CMD_type >> 24) & 0xff);
                    UDP_packet[45 + j] = Convert.ToByte((CMD_type >> 16) & 0xff);
                    UDP_packet[46 + j] = Convert.ToByte((CMD_type >> 8) & 0xff);
                    UDP_packet[47 + j] = Convert.ToByte((CMD_type >> 0) & 0xff);

                    UDP_packet[48 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 56) & 0xff);
                    UDP_packet[49 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 48) & 0xff);
                    UDP_packet[50 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 40) & 0xff);
                    UDP_packet[51 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 32) & 0xff);
                    UDP_packet[52 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 24) & 0xff);
                    UDP_packet[53 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 16) & 0xff);
                    UDP_packet[54 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 8) & 0xff);
                    UDP_packet[55 + j] = Convert.ToByte((FRAME.MSG.CMD.Cmd_id >> 0) & 0xff);

                    UDP_packet[56 + j] = Convert.ToByte((CMD_time >> 56) & 0xff);
                    UDP_packet[57 + j] = Convert.ToByte((CMD_time >> 48) & 0xff);
                    UDP_packet[58 + j] = Convert.ToByte((CMD_time >> 40) & 0xff);
                    UDP_packet[59 + j] = Convert.ToByte((CMD_time >> 32) & 0xff);
                    UDP_packet[60 + j] = Convert.ToByte((CMD_time >> 24) & 0xff);
                    UDP_packet[61 + j] = Convert.ToByte((CMD_time >> 16) & 0xff);
                    UDP_packet[62 + j] = Convert.ToByte((CMD_time >> 8) & 0xff);
                    UDP_packet[63 + j] = Convert.ToByte((CMD_time >> 0) & 0xff);

                    for (i = 0; i < CMD_size; i++) UDP_packet[64 + i + j] = CMD_data[i];

                    DATA_lenght = DATA_lenght + Convert.ToInt32(CMD_size) + 24;

                    sch_cmd--;
                    j = j + Convert.ToInt32(CMD_size) + 24;
                }

                //-----шлём данные по UDP--------------

                UdpClient client = new UdpClient();
                client.Connect(ip_dest, port_dest);
                int number_bytes = client.Send(UDP_packet, DATA_lenght);
                //           Debug.WriteLine("DATA_lenght                  :" + DATA_lenght);
                //           Debug.WriteLine("FRAME.MSG.CMD.Cmd_data.Length:" + FRAME.MSG.CMD.Cmd_data.Length);
                //           Debug.WriteLine("FRAME.MSG.CMD.Cmd_data       :" + FRAME.MSG.CMD.Cmd_data);
                client.Close();

            }
            catch
            {
                Console.WriteLine("какой-то косяк в UDP!");
            }
        }
    }
}
