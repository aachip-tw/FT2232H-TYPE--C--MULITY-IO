aachipDLL2.dll 專門使用於8位元應用
本dll是在ftd2xx_net下增加的專用APP
需配合本公司專用的FTDI2232H MULITY IO TYPE C USB 卡來使用


1. public bool spiinit(byte clkspeed,string AX)
clkspeed:(byte)由66MHZ除頻率用1=66M BPS,2=33M.最小為1最大255,可以先由0X1F當標準慢慢減少再做寫讀的比較,直到不穩定的前一個來確定速度
AX=A 或 B ;選擇APORT,或是BPORT;

examples:  
     bool testx = false;
     testx=myFtdiDevice.spiinit(0x1f,"B");  //第一個是數字,第二個是選擇右A 或左B 
  
  
2. public bool spiwriteCMD(byte CMDH,byte SRAMADDRESSLA8A15,byte SRAMADDRESSLA0A7)
CMDH:(byte)8BIT 想讓FPGA處理各個命令
SRAMADDRESSLA8A15:(byte)8BIT 當需要時才用,平時為0 ;例如當中斷位置時用
SRAMADDRESSLA0A7:(byte)8BIT  當需要時才用,平時為0

examples:
   testx = myFtdiDevice.spiwriteCMD(0xf1, 0, 0); 
   f1可以設計為reset fpga system;


3.public bool spiwrite_DATA(byte CMDH,byte SRAMADDRESSLA8A15,byte SRAMADDRESSLA0A7, byte[] sfrirambuf,uint writelength)
寫大筆資料用;最大64K BYTE 0~FFFF
CMDH:(byte)指定哪一個區域用,例如內部SRAM,或是 外部SRAM
SRAMADDRESSLA8A15:(byte)8BIT 當需要時才用,平時為0 ;從0開始
SRAMADDRESSLA0A7:(byte)8BIT  當需要時才用,平時為0;從0開始
sfrirambuf :(byte)0~ 65535將整個要寫進FPGA內的資料先存到這裡
writelength:uint 0~ffff 告知fpga 需要寫入資料的長度

examples:
    uint wlength = 0x4000; //整個要寫資料的長度
    byte[] sfrirambuf=new byte[wlength]; //重新定義以符合要寫進去長度的矩陣
    testx = false;
    
    for(int i=0;i<wlength;i++)
    {
        sfrirambuf[i] = PROG[i];
    }
    testx = myFtdiDevice.spiwrite_DATA(01,0,0, sfrirambuf,wlength); //專用



4.public bool spiread_DATA(byte CMDH, byte SRAMADDRESSLA8A15, byte SRAMADDRESSLA0A7, uint readlength) // 讀回資料放到RAMBUF[i]
讀大筆資料用;最大64K BYTE 0~FFFF
CMDH:(byte)指定哪一個區域用,例如內部SRAM,或是 外部SRAM
SRAMADDRESSLA8A15:(byte)8BIT 當需要時才用,平時為0 ;從0開始
SRAMADDRESSLA0A7:(byte)8BIT  當需要時才用,平時為0;從0開始
readlength:uint 0~ffff 告知fpga 需要讀資料的長度

examples:
         byte[] BUFFER = new byte[wlength];
         testx = false;
         testx = myFtdiDevice.spiread_DATA(02, 0, 0, 16384,BUFFER); // 讀回資料放到BUFFER
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




   
5.public bool Chk_Di_Input_lo(out bool isLow) //測試spi data input是否為0
通常spi data input di在運作時都為hi;你可以設計當某一個條件符合時spi data input 為0 再利用c# timer 中斷時去讀di 是否為0

examples:
            testx = false;
            testx = myFtdiDevice.Chk_Di_Input_lo(out bool isLow); //測試spi data input是否為0
			if(isLow==true)
			{
				MessageBox.Show("di為lo");			
			}
        
