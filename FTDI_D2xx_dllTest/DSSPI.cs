using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTD2XX_NET;

namespace WindowsFormsApplication1
{
    class DSSPI
    {
        // IO Buffer Sizes
            const int FT_In_Buffer_Size = 0x100000;    // 1024k
//    FT_In_Buffer_Size = $10000;    // 64k
//    FT_In_Buffer_Size = $8000;    // 32k
            const int FT_In_Buffer_Index = FT_In_Buffer_Size - 1;
//          FT_Out_Buffer_Size = $10000;    // 64k
            const int FT_Out_Buffer_Size = 0x8000;    // 32k
            const int FT_Out_Buffer_Index = FT_Out_Buffer_Size - 1;


        public FTDI myFtdiDevice;
        public FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList;
        int ClkDiv = 0;
        int OutIndex;
        byte[] FT_Out_Buffer = new byte[FT_Out_Buffer_Index];
        byte[] dataBuffer = new byte[100];
        byte[] OutBuff = new byte[60000];
        byte[] In_Buff = new byte[FT_In_Buffer_Index];
        byte Saved_port_Value;
        bool PortAIsOpen = false;
        bool working = false;
        public UInt32 ftdiDeviceCount = 0;
        public FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
        
        General general = new General();

        //constructor
        public DSSPI()
        {
            
            myFtdiDevice = new FTDI();

            // Determine the number of FTDI devices connected to the machine
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            
           
        }


        private void OutputLoByte(byte Value, byte Direction)
        {
            AddToBuffer(Const_FTDI.OPCODE_SET_LOBYTE_AND_DIR); // Command 80  Set Data bits LowByte
            AddToBuffer(Value);
            AddToBuffer(Direction);

        }

        private void OutputHighByte(byte Value, byte Direction)
        {
            AddToBuffer(Const_FTDI.OPCODE_SET_HIBYTE_AND_DIR); // Command 82  Set Data bits HighByte
            AddToBuffer(Value);
            AddToBuffer(Direction);

        }

        private void SetClkDiv(int val)
        {

            ClkDiv = val;
            AddToBuffer(0x86);
            AddToBuffer(val & 0xff);  //lowbyte
            AddToBuffer(val & 0xff00 >> 8); //highbyte

        }


        private void AddToBuffer(int i)
        {
            FT_Out_Buffer[OutIndex] = (byte)(i & 0xff);
            OutIndex++;

        }

        private void ResetBuffer()
        {
            OutIndex = 0;
        }

        private FTDI.FT_STATUS SendBuffer()
        {
            uint bytesSent = 0;
            return myFtdiDevice.Write(FT_Out_Buffer, OutIndex, ref bytesSent);

        }

        public bool Init_Controller_MDIO(string DName, bool ResetController)
        {
            if (OpenPort())
            {
                myFtdiDevice.SetLatency(16);
                if (ResetController)
                {

                        ftStatus = myFtdiDevice.SetBitMode(0x00, FTDI.FT_BIT_MODES.FT_BIT_MODE_RESET);

                        ftStatus = myFtdiDevice.SetBitMode(0x00, FTDI.FT_BIT_MODES.FT_BIT_MODE_MPSSE);
             
                }

                if (Sync_To_MPSSE())
                {
                    ResetBuffer();
                    OutputLoByte(0x0, 0x01);
                    OutputHighByte(0x0, 0x0f);
                    SetClkDiv(ClkDiv);
                    AddToBuffer(Const_FTDI.OPCODE_DISABLE_LOOPBACK);              // Command 85 Loopback disable
                    SendBuffer();
                    return true;
                }
                else
                {
                    return false;
                }


            }
            else
            {
                return false;
            }


        }



        public bool Sync_To_MPSSE()
        {
            FTDI.FT_STATUS res;
            uint RxQueueSize = 0;
            uint numberBytesRead = 0;


            ResetBuffer();
            AddToBuffer(0xaa);
            //FT_Out_Buffer[0] = 0xAA; // bad command
            //res = myFtdiDevice.Write(FT_Out_Buffer, 1, ref bytesSent);
            res = SendBuffer();
            general.PauseForMilliSeconds(100);
            res = myFtdiDevice.GetRxBytesAvailable(ref RxQueueSize);
            res = myFtdiDevice.Read(dataBuffer, RxQueueSize, ref numberBytesRead);

            for (int i = 0; i < RxQueueSize; i++)
            {
                if ((dataBuffer[i] == 0xfa) & (i <= RxQueueSize - 2) & (dataBuffer[i + 1] == 0xaa))
                {
                    break;
                }
                else
                {
                    return false;
                }
            }

            ResetBuffer();
            AddToBuffer(0xAB);
            //FT_Out_Buffer[0] = 0xAB; // bad command
            //res = myFtdiDevice.Write(FT_Out_Buffer, 1, ref bytesSent);
            res = SendBuffer();
            general.PauseForMilliSeconds(100);
            res = myFtdiDevice.GetRxBytesAvailable(ref RxQueueSize);
            res = myFtdiDevice.Read(dataBuffer, RxQueueSize, ref numberBytesRead);

            for (int i = 0; i < RxQueueSize; i++)
            {
                if ((dataBuffer[i] == 0xfa) & (i <= RxQueueSize - 2) & (dataBuffer[i + 1] == 0xab))
                {
                    return true;

                }
                else
                {
                    return false;

                }

            }

            return false;



        }

        private void ScanOut(int BitCount, byte[] OutBuff)
        {
            int Mod_BitCount;
            int i;
            int j = 0; 


            if (PortAIsOpen)
            {
                Mod_BitCount = BitCount - 1;
                if ((Mod_BitCount / 8) > 0)
                {
                    i = (Mod_BitCount / 8) - 1;
                    AddToBuffer(0x11);
                    AddToBuffer(i & 0xff);
                    AddToBuffer((i / 256) & 0xff);
                    while (j <= i)
                    {
                        AddToBuffer(OutBuff[j]);  ///  check this was a pointer in delphi
                        j = j + 1;
                    }
                }
                if ((Mod_BitCount % 8) > 0)
                {
                    i = Mod_BitCount % 8;
                    AddToBuffer(0x13);
                    AddToBuffer(i & 0xff);
                    AddToBuffer(OutBuff[j]);

                }

            }
        }

        private void ScanOut_AA(int BitCount, byte[] OutBuff)
        {
            int Mod_BitCount;
            int i;
            int j = 0;


            if (PortAIsOpen)
            {
                Mod_BitCount = BitCount - 1;
                if ((Mod_BitCount / 8) > 0)
                {
                    i = (BitCount / 8) - 1;
                    AddToBuffer(0x11);
                    AddToBuffer(i & 0xff);
                    AddToBuffer((i / 256) & 0xff);
                    while (j <= i)
                    {
                        AddToBuffer(OutBuff[j]);  ///  check this was a pointer in delphi
                        j = j + 1;
                    }
                }
                if ((Mod_BitCount % 8) > 0)
                {
                    i = BitCount % 8;
                    AddToBuffer(0x13);
                    AddToBuffer(i & 0xff);
                    AddToBuffer(OutBuff[j]);

                }

            }
        }

        private bool ScanIn(int BitCount)
        {
            int Mod_BitCount;
            int i;
            int j = 0;
            if (PortAIsOpen)
            {
                Mod_BitCount = BitCount - 1;
                if ((Mod_BitCount / 8) > 0)
                {
                    i = (Mod_BitCount / 8) - 1;
                    AddToBuffer(0x20);
                    AddToBuffer(i & 0xff);
                    AddToBuffer((i / 256) & 0xff);
                }

                if ((Mod_BitCount % 8) > 0)
                {
                    i = Mod_BitCount % 8;
                    AddToBuffer(0x22);
                    AddToBuffer(i & 0xff);
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        private static byte[] ReverseBytes(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }

        public void ClosePort()
        {

            if (PortAIsOpen)
            {
                FTDI.FT_STATUS res;
                res = myFtdiDevice.Close();
                if (res == FTDI.FT_STATUS.FT_OK)
                {
                    PortAIsOpen = false;
                }
            }
        }
    
    

        public void Erase_AT45_Chip()
        {
            OutIndex = 0;
            setFtdiByteAndDirectionForWright();

            byte[] temp = ReverseBytes(BitConverter.GetBytes(Const_AT45.chipErase));

            OutBuff = temp;

            ScanOut(32, OutBuff);
            SendBytes(OutIndex);

        }

        private void setFtdiByteAndDirectionForWright()
        {            
            AddToBuffer(Const_FTDI.OPCODE_SET_LOBYTE_AND_DIR);
            AddToBuffer(0x02);
            AddToBuffer(0x0b);
        }

        public void Erase_AT45_Block()
        {
            OutIndex = 0;
            setFtdiByteAndDirectionForWright();
         
            OutBuff[0] = Const_AT45.blockErase;
            OutBuff[1] = 0x00;
            OutBuff[2] = 0x00;
            OutBuff[3] = 0x00;

            ScanOut(32, OutBuff);
            SendBytes(OutIndex);

        }

        public string Read_AT45_ID()
        {
            OutIndex = 0;
            setFtdiByteAndDirectionForWright();

            OutBuff[0] = Const_AT45.manufacturerAndDeviceIDRead;
            ScanOut(8, OutBuff);
            setFtdiByteAndDirectionForRead();
            ScanIn(32);

            AddToBuffer(Const_FTDI.OPCODE_SEND_IMMEDIATE);  //chip flush its buffer back to PC
            SendBytes(OutIndex);
            Read_Data(32);

            return string.Format("{0:X}{1:X}", In_Buff[0], In_Buff[1]);

           // return BitConverter.ToInt32(In_Buff,0);
        }

        public void BufferProg_AT45(int iByte, int length, byte[] BuffChunk)
        {
            int i = 0;
            int j = 0;

            OutIndex = 0;
            for (i = 0; i <= 500; i++)
            {
                In_Buff[i] = 0; // clears out buffer
               
            } 
            AddToBuffer(0x80);
            AddToBuffer(0x02);
            Saved_port_Value = 0x00;
            AddToBuffer(0x0b);

            i = 0x84;
            i = i & 0xff;
            OutBuff[0] = (byte)i;

            i = 0xff;
            OutBuff[1] = (byte)i;

            for (j = 4; j< BuffChunk.Count()+4; j++)   // j <= 264 + 4; j++)
            {
                OutBuff[j] = BuffChunk[j - 4];
            }
            ScanOut((32 + (264 * 8)), OutBuff);
            SendBytes(OutIndex);
        }

        public void Buffer_to_Flash_AT45(int page)
        {int i =0;
            OutIndex = 0;
            AddToBuffer(0x80); 
            AddToBuffer(0x02);                               
            Saved_port_Value = 0;
            AddToBuffer(0x0b); 

            //move data from device buffer 1 to Flash ##################################

i = 0x83;
i = i & 0xFF;                                   //AT45 Program with erase = 83
OutBuff[0] = (byte)i;                                 // Stick in buffer

i = (page >> 7) & 0xff;                        // part 1 0f Page address in buffer
OutBuff[1] = (byte)i;                                 // Stick in buffer

i = (page << 1) & 0xff;                        //part 2 of page address, 1 bit don't care
OutBuff[2] =(byte)i;

i = 0xff;                                          // 1 byte don't care
OutBuff[3] = (byte)i;                                  // Stick in buffer

ScanOut(32,OutBuff);
SendBytes(OutIndex); // send off the command

        }



        private void setFtdiByteAndDirectionForRead()
        {

            // Change DO to and input to recive data (high z)
            AddToBuffer(Const_FTDI.OPCODE_SET_LOBYTE_AND_DIR);
            AddToBuffer(0x00);  //all low
            AddToBuffer(0x09); //inputs on GPIO11-14    

        }

        public void Read_AT45_Array(UInt32 page, UInt32 startbyte, UInt32 length, ref byte[] BuffPass)
        {
            
            OutIndex = 0;
            setFtdiByteAndDirectionForWright();
            OutBuff[0] = Const_AT45.continuousArrayReadLegacy;
            OutBuff[1] = (byte)((page >> 7) & 0xff);
            OutBuff[2] = (byte)(((page << 1) & 0xff)|(( startbyte >> 8) & 0xff));
            OutBuff[3] = (byte)(startbyte & 0xff);
            ScanOut(64, OutBuff);

            setFtdiByteAndDirectionForRead();
            ScanIn((int)(length * 8));

            AddToBuffer(Const_FTDI.OPCODE_SEND_IMMEDIATE);

            SendBytes(OutIndex);
            Read_Data((int)(length * 8));

            for (int i = 0; i < length - 1; i++)
            {
                BuffPass[i] = In_Buff[i];
            }

        }

        public void Read_More_AT45(UInt32 page, UInt32 startbyte, UInt32 length, ref byte[] BuffPass)
        {
           
            OutIndex = 0;
            for (int i = 0; i < 264; i++)
            {
                In_Buff[i] = 0;
            }

           // setFtdiByteAndDirectionForRead(); may need ?
            ScanIn((int)( length * 8));
            AddToBuffer(Const_FTDI.OPCODE_SEND_IMMEDIATE);
            SendBytes(OutIndex);
            Read_Data((int)(length * 8));
            for (int i = 0; i < length; i++)
            {
                BuffPass[i] = In_Buff[i];
            }

        }

        private void SendBytes(int NumberOfBytes)
        {
            uint i = 0;
            myFtdiDevice.Write(FT_Out_Buffer, NumberOfBytes, ref i);
            OutIndex = OutIndex - (int)i;

        }


        public int StatusRead_AT45()
        {
            OutIndex = 0;
            int i;
            for (i = 0; i < 500; i++)
            {
                In_Buff[i] = 0;
            }

            AddToBuffer(0x80);
            AddToBuffer(0x02);
            AddToBuffer(0x0b);

            i = 0x57;
            i = i & 0xFF;
            OutBuff[0] = (byte)i;

            ScanOut_AA(16, OutBuff);

            AddToBuffer(0x80);
            AddToBuffer(0x00);
            AddToBuffer(0x09);

            ScanIn(16);

            AddToBuffer(0x87);

            SendBytes(OutIndex);

            Read_Data(16);

            return (In_Buff[0] & 0xff);

        }

        private void Read_Data(int BitCount)
        {
            int Mod_BitCount = BitCount - 1;
            int NoBytes = Mod_BitCount / 8;
            int BitShift = Mod_BitCount % 8;
            byte[] Temp_Buffer = new byte[64000];

            if (BitShift > 0)
            {
                NoBytes = NoBytes + 1;

            }

            uint i = 0;
            int TotalBytes = 0;
            uint FT_Q_BYTES = 0;
            uint bytesRead = 0;
            FTDI.FT_STATUS res = FTDI.FT_STATUS.FT_OTHER_ERROR;

            while (TotalBytes < NoBytes)
            {
                while (FT_Q_BYTES == 0)
                {
                    res = myFtdiDevice.GetRxBytesAvailable(ref FT_Q_BYTES);
                }

                if (res != FTDI.FT_STATUS.FT_OK)
                {
                break;
                }

                if (FT_Q_BYTES > 0)
                {
                    res = myFtdiDevice.SetTimeouts(5000, 0);
                    if (res != FTDI.FT_STATUS.FT_OK)
                    {
                        throw new Exception(string.Format("Error to set timeouts, ftStatus: {0}", res));
                        //break;
                    }

                    res = myFtdiDevice.Read(In_Buff, FT_Q_BYTES, ref bytesRead);
                    if (res != FTDI.FT_STATUS.FT_OK)
                    {
                        throw new Exception(string.Format("Error read, ftStatus: {0}", res));
                        //break;
                    }
                }

                for (i = 0; i <= bytesRead - 1; i++)
                {
                    //Temp_Buffer[TotalBytes] = FT_In_Buffer[i];
                    TotalBytes = TotalBytes + 1;
                }
            }

            //for (int j = 0; j > NoBytes - 1; j++)
            //{
            //    In_Buff[j] = Temp_Buffer[j];
            //}
        }

        public bool Toggle_CS()
        {
            OutIndex = 0;

            AddToBuffer(0x80);
            AddToBuffer(0x08);
            AddToBuffer(0x0b);

            OutBuff[0] = 0;
            OutBuff[1] = 0;
            ScanOut(6, OutBuff);


            AddToBuffer(0x80);
            AddToBuffer(0x02);
            AddToBuffer(0x0b);

            SendBytes(OutIndex);

            return true;

        }

        private bool OpenPort() //  (string PortName)
        {
            if (!PortAIsOpen)
            {
                FTDI.FT_STATUS res;
                UInt32 ftdiDeviceCount = 0;
                ftdiDeviceList = null;

                res = myFtdiDevice.CyclePort();
                res = myFtdiDevice.ResetDevice();
                res = myFtdiDevice.ResetPort();

                res = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
                if (res == FTDI.FT_STATUS.FT_OK)
                {
                    if (ftdiDeviceCount > 0)
                    {

                        ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

                        // Populate our device list
                        res = myFtdiDevice.GetDeviceList(ftdiDeviceList);
                        if ((res == FTDI.FT_STATUS.FT_OK) & (ftdiDeviceList[0] != null))
                        {
                            res = myFtdiDevice.OpenByDescription(ftdiDeviceList[0].Description);
                            if (res == FTDI.FT_STATUS.FT_OK)
                            {
                                // textOutput.AppendText("Failed to open device (error " + res.ToString() + ")");


                                //res = myFtdiDevice.CyclePort();
                                //res = myFtdiDevice.ResetDevice();
                                //res = myFtdiDevice.ResetPort();

                                PortAIsOpen = true;
                                return true;
                            }

                            else
                            {

                                return false;
                            }


                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }


                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;

            }
        }

    }
}
