using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data;
using FTD2XX_NET;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        DSSPI dsspi;
        General general = new General();
        bool working = false;

        public Form1()
        {
            InitializeComponent();

            dsspi = new DSSPI();
            
            // Determine the number of FTDI devices connected to the machine
            //ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            
            // Check status
            if (dsspi.ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                textOutput.AppendText(String.Format("there are {0} devices. Status {1}", dsspi.ftdiDeviceCount, dsspi.ftStatus));

            }
            else
            {
                // Wait for a key press
                //Console.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                //Console.ReadKey();
                //return;
            }

            // If no devices available, return
            if (dsspi.ftdiDeviceCount == 0)
            {
                textOutput.AppendText("\n");
                textOutput.AppendText("Failed to get number of devices (error " + dsspi.ftStatus.ToString() + ")");

                return;
            }

            // Allocate storage for device info list
            dsspi.ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[dsspi.ftdiDeviceCount];

            // Populate our device list
            dsspi.ftStatus = dsspi.myFtdiDevice.GetDeviceList(dsspi.ftdiDeviceList);

            if (dsspi.ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                for (UInt32 i = 0; i < dsspi.ftdiDeviceCount; i++)
                {



                    textOutput.AppendText("\nDevice Index: " + i.ToString());
                    textOutput.AppendText("\nFlags: " + String.Format("{0:x}", dsspi.ftdiDeviceList[i].Flags));
                    textOutput.AppendText("\nType: " + dsspi.ftdiDeviceList[i].Type.ToString());
                    textOutput.AppendText("\nID: " + String.Format("{0:x}", dsspi.ftdiDeviceList[i].ID));
                    textOutput.AppendText("\nLocation ID: " + String.Format("{0:x}", dsspi.ftdiDeviceList[i].LocId));
                    textOutput.AppendText("\nSerial Number: " + dsspi.ftdiDeviceList[i].SerialNumber.ToString());
                    textOutput.AppendText("\nDescription: " + dsspi.ftdiDeviceList[i].Description.ToString());
                    textOutput.AppendText("\n");

                }
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            bool correctDevice = Check_For_Dev_A(dsspi.ftdiDeviceList[0].Description.ToString());
            if (correctDevice)
            {
               dsspi.Init_Controller_MDIO("a", true);

            }

        }

        private bool Check_For_Dev_A(string name)
        {
            if (name != "")
            {
                if ((name == "Memory Module Programmer A")|(name == "Dual RS232 A"))
                {
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


        private void button3_Click(object sender, EventArgs e)
        {
            if (dsspi.Init_Controller_MDIO("a", true))
            {
                dsspi.Erase_AT45_Chip();
                DeviceBusy();
                dsspi.Toggle_CS();
                dsspi.Erase_AT45_Block(); //for old devices erase first block
                DeviceBusy();
                dsspi.ClosePort();
            }
            else
            {
                //error int controler fail
            }
        }

        private void DeviceBusy()
        {
            textOutput.AppendText("\n****a");
           // Application.DoEvents();
            int delay = 0;
            while (delay < 10000)
            {
                delay++;
            }

            int Result = 0;
            while ((Result & 0x80) != 0x80)
            {
                if (dsspi.Toggle_CS())
                {
                    delay = 0;
                    while (delay < 10000)
                    {
                        delay++;
                    }

                    Result = dsspi.StatusRead_AT45();
                   // general.PauseForMilliSeconds(100);
                    textBox1.Text = "busy";
                    textOutput.AppendText("1.");
                    // textBox1.Refresh();
                   // Application.DoEvents();
                    textOutput.AppendText("2.");
                }
                else
                {
                    Result = 0x0;
                }
            }
            dsspi.Toggle_CS();
            textBox1.Text = "done";
        }

        private void button2_Click(object sender, EventArgs e)
        {

            textOutput.AppendText("\nChip ID: " + Read_AT45DB081D_ID());
            
             
        }

        private string Read_AT45DB081D_ID()
        {
           // if (!working)
           // {
            
                working = true;
                if (dsspi.Init_Controller_MDIO("a", true))
                {

                    DeviceBusy();
                    string temp = dsspi.Read_AT45_ID();
                    dsspi.ClosePort();

                    working = false;

                    return temp;
                }
                else
                {
                    //error init controler fail
                    return "error init fail";
                }
          //  }
          //  else
          //  {
         //       return "busy";
         //   }

        }

        private string Program_AT45DB081D()
        {int page = 0;

            if (dsspi.Init_Controller_MDIO("a", true))
            {
                byte[] test = new byte[] { 1, 2, 3, 4 };

                DeviceBusy();
                dsspi.BufferProg_AT45(0, test.Count(), test);
                dsspi.Toggle_CS();
                dsspi.Buffer_to_Flash_AT45(page);
                DeviceBusy();
                dsspi.Toggle_CS();
                
                dsspi.ClosePort();

                return "a"; // temp;

            }
            else
            {
                //error init controler fail
                return "error init fail";
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textOutput.Clear();
            Read_AT45DB081D_Array(Convert.ToUInt32(textBox2.Text),Convert.ToUInt32(textBox3.Text), Convert.ToUInt32(textBox4.Text));

        }

        private void Read_AT45DB081D_Array(UInt32 startPage, UInt32 startByte, UInt32 length)
        {
            UInt32 reads = length / 264;
            UInt32 remainder = length % 264;
            byte[] buffPass = new byte[300];
            uint page = startPage;
            uint sByte = startByte;

            if (dsspi.Init_Controller_MDIO("a", true))
            {
                textOutput.AppendText("\n****0");
                if (reads < 1)
                {
                    textOutput.AppendText("\n****1");
                    DeviceBusy();
                    textOutput.AppendText("\n****2");
                    dsspi.Read_AT45_Array(startPage, startByte, length, ref buffPass);
                    textOutput.AppendText("\n****3");
                    dsspi.ClosePort();

                    textOutput.AppendText("\n****4");
                    for (int i = 0; i < length; i++)
                    {
                        textOutput.AppendText(" " + string.Format("{0:X}", buffPass[i]));
                    }

                }
                else
                {
                    for (int goes = 0; goes < reads; goes++)
                    {
                        if (goes == 0)
                        {
                            dsspi.Read_AT45_Array(page, sByte, 264, ref buffPass);
                        }
                        else
                        {
                            dsspi.Read_More_AT45(page, sByte, 264, ref buffPass);
                        }

                        for (int i = 0; i < 264; i++)
                        {
                            textOutput.AppendText(" " + string.Format("{0:X}", buffPass[i]));
                        }
                        page++;
                        textOutput.AppendText(" " + string.Format("Page={0}\n", page));

                        if (remainder > 0)
                        {
                            dsspi.Read_More_AT45(page, sByte, remainder, ref buffPass);
                            for (int i = 0; i < remainder; i++)
                            {
                                textOutput.AppendText(" " + string.Format("{0:X}", buffPass[i]));
                            }
                        }

                    }
                }
            }
            else
            {
                //error init controler fail
                textOutput.AppendText("error init fail");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Program_AT45DB081D();
        }

    }
}
