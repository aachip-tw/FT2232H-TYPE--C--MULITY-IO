using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using FTD2XX_NET;



namespace test2
{
    public partial class Form1 : Form
    {
        public string SerialNumber;
        public byte[] PROG = new byte[0X10000];    //
        public byte[] BUFFER = new byte[0X10000];    //
        //   wework2:
        FTDI ftdi = new FTDI();     //?身銝?tdi鋆蔭,???榻tdi,敺?賭誑ftdi?箔蜓閬?蝵桀?
        FTDI.FT_DEVICE_INFO_NODE[] devicelist = new FTDI.FT_DEVICE_INFO_NODE[2];    //??fdti2232h??蝵?
        FTDI myFtdiDevice = new FTDI();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //-------------------------------------------------------------------------------
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BIN 檔案|*.bin";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string binFilePath = openFileDialog.FileName;
                string filepathname;
                filepathname = binFilePath;
                UInt32 dl = 0;
           
            
           
             

                try
                {
                    // 讀取 TEST1.BIN 檔案並存儲到 BinaryReader 中
                    using (BinaryReader inFile = new BinaryReader(File.Open(binFilePath, FileMode.Open)))
                    {
                        // 將二進制數據處理邏輯寫在這裡
                        // 例如：inFile.ReadBytes()、inFile.ReadInt32() 等
                        // 注意處理二進制數據的格式和邏輯
                        dl = System.Convert.ToUInt32(inFile.BaseStream.Length);  //
                                            //
                        int ld = Convert.ToInt32(dl);
                       PROG = inFile.ReadBytes(ld);

                        inFile.Close();         //關閉HD
                                                //   SRAMADDRESSLA8A15 = (byte)(SRAMADDRESSLA8A15 + 1); //做完256 byte a8+1


                        Array.Resize(ref PROG, 16384);      //重新將讀到的資料轉成長度16384(如果小於16384)
              
                    }
                }
                catch (Exception)
                {
                    // MessageBox.Show("無法讀取 BIN 檔案: " + ex.Message);
                }

            }

            //--------------------------------------------------------------------------------
            uint devicecount = 0;
            myFtdiDevice.GetNumberOfDevices(ref devicecount);
            bool testx = false;
            testx=myFtdiDevice.spiinit(0x1f,"B");  //第一個是數字,第二個是選擇右A 或左B
            // 创建 FT2232H_EEPROM_STRUCTURE 结构实例
            //讀取整個EEPROM的資訊
            FTDI.FT_STATUS ftStatus;
            FTDI.FT2232H_EEPROM_STRUCTURE ee2232h = new FTDI.FT2232H_EEPROM_STRUCTURE();
     
            // 读取 EEPROM 用户区域
            ftStatus = myFtdiDevice.ReadFT2232HEEPROM(ee2232h);
        /*
            if(ftStatus == FT_ERROR)
            {
                myFtdiDevice.FT_ResetDevice;
            }
        */

            SerialNumber = ee2232h.SerialNumber; //讀取SERIAL NUMBER

            //    string ux = myFtdiDevice.SerialNumber();
            testx = false;
            testx = myFtdiDevice.spiwriteCMD(0xf1, 0, 0);  //set command 第一個是命令,第二個是高位置,第三個是低位置

            uint wlength = 0x4000; //整個要寫資料的長度
            byte[] sfrirambuf=new byte[wlength];
            testx = false;
            
            for(int i=0;i<wlength;i++)
            {
                sfrirambuf[i] = PROG[i];
            }
            testx = myFtdiDevice.spiwrite_DATA(01,0,0, sfrirambuf,wlength); //專用

            byte[] BUFFER = new byte[wlength];
            testx = false;
            testx = myFtdiDevice.spiread_DATA(02, 0, 0, 16384,BUFFER); // 讀回資料放到RAMBUF[i]
            bool comp = false;
            for (int i = 0; i < wlength; i++)
            {
                if (PROG[i] != BUFFER[i])
                {
                    comp = false;
                    break;
                }
                comp = true;
            }

            if(comp==true)
            {
                MessageBox.Show("比對成功");
            }

            testx = false;
            testx = myFtdiDevice.Chk_Di_Input_lo(out bool isLow); //測試spi data input是否為0
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
