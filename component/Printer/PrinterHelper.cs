// <copyright file="AccessHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using MQTTnet.Core.Client;
    using MQTTnet.Core.Packets;
    using MQTTnet.Core.Protocol;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;
    using Spire.License;
    using Spire.Pdf;

    internal class PrinterHelper
    {

        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印

        private static PrinterHelper instance = null;
        private static bool bIsRunning = false;

        private static List<Dictionary<string, object>> scoreList = new List<Dictionary<string, object>>();
        private static Dictionary<string, string> dictionary = new Dictionary<string, string>();
        private static PrintDocument fPrintDocument = new PrintDocument();

        //创建PdfDocument类的对象，并加载PDF文档
        private static PdfDocument doc = new PdfDocument();
       // doc.LoadFromFile(cjdFile);
       //此行代码为选择打印机名称来打印
       //doc.PrintSettings.PrinterName="打印机名称";
       //直接打印会调用电脑的默认打印机进行打印，请在控制面板->设备的打印机中配置默认打印机
       //doc.Print();

        public static PrinterHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new PrinterHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, bool bIsRunning)
        {
            Config(config);
            PrinterHelper.bIsRunning = bIsRunning;
        }

        public static void Stop()
        {
            PrinterHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
        }




        Brush brushHeaderFont = new SolidBrush(Color.Black);
        Pen LinePen = new Pen(Color.Black, 1);
        public PrinterHelper()
        {
            InitPrint();
        }
        /// <summary>
        /// 初始化打印机
        /// </summary>
        private void InitPrint()
        {
  
        }

        /// <summary>
        /// 打印
        /// </summary>
        public void Print()
        {
        }



        public static void Setjson(List<Dictionary<string, object>> data)
        {
            scoreList.AddRange(data);

        }


        public static void Clearjson()
        {
            scoreList.Clear();

        }


        public static void PrintPage(object[] json)
        {
           // Get_image("587D11DD9F", 12.0F, 300, 70);
            Clearjson();

            foreach (object v in json)
            {
                scoreList.Add((Dictionary<string, object>)v);
            }

             PrintDialog PD = new PrintDialog();
             PageSettings pageSettings = new PageSettings();
             string Name = fPrintDocument.PrinterSettings.PrinterName;
             LogHelper.Log("PrinterName: " + Name.ToString());
             System.Drawing.Printing.PrinterSettings Ps = new System.Drawing.Printing.PrinterSettings();
             // SetDefaultPrinter("Deli DL-888B(NEW)");
             // fPrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Size", 393, 275); //单位1/100英寸
             SetDefaultPrinter(Name);
             PD.PrinterSettings = Ps;
             fPrintDocument.PrintPage += document_PrintPage;
             PD.Document = fPrintDocument;
             fPrintDocument.Print();
        }

        static void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            foreach (Dictionary<string, object> body in scoreList)
            {

                if (body["type"].ToString() == "scancode")
                {
                    Image image = PrinterHelper.Get_image(body["text"].ToString(), float.Parse(body["fontSize"].ToString()), int.Parse(body["width"].ToString()), int.Parse(body["height"].ToString()), body["fontFamily"].ToString());
                    int widthInch = pxToInch(int.Parse(body["width"].ToString()));
                    int heightInch = pxToInch(int.Parse(body["height"].ToString()));
                    // LogHelper.Log("widthInch: " + widthInch);
                    // LogHelper.Log("heightInch: " + heightInch);
                    e.Graphics.DrawImage(image, int.Parse(body["x"].ToString()), int.Parse(body["y"].ToString()), widthInch, heightInch); //单位1/100英寸
                }
                else if (body["type"].ToString() == "paper")
                {
                    int paperwidth = pxToInch(int.Parse(body["width"].ToString()));
                    int paperheight = pxToInch(int.Parse(body["height"].ToString()));
                    // LogHelper.Log("paperwidth: " + paperwidth);
                    // LogHelper.Log("paperheight: " + paperheight);

                    fPrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Size", paperwidth, paperheight); //单位1/100英寸
                }
                else
                {
                    e.Graphics.DrawString(body["text"].ToString(), new Font(new FontFamily(body["fontFamily"].ToString()), int.Parse(body["fontSize"].ToString())), System.Drawing.Brushes.Black, int.Parse(body["x"].ToString()), int.Parse(body["y"].ToString()));
                }
            }
            e.HasMorePages = false;
        }

        public static Image Get_image(String Data, float FontSize, int Width, int Height, String familyName)
        {
            // LogHelper.Log("Data: " + Data);
            // LogHelper.Log("FontSize: " + FontSize);
            // LogHelper.Log("Width: " + Width.ToString());
            // LogHelper.Log("Height: " + Height.ToString());

            CodeLib.TYPE type = CodeLib.TYPE.CODE128B;
            Font Label = new Font(familyName, FontSize * 4);
            CodeLib.AlignmentPositions Align = CodeLib.AlignmentPositions.CENTER;

            CodeLib.Barcode b = new CodeLib.Barcode();
            b.IncludeLabel = true;
            b.LabelFont = Label;
            b.Alignment = Align;
            b.LabelPosition = CodeLib.LabelPositions.BOTTOMCENTER;
            b.Encode(type, Data, Width * 4, Height * 4).Save("./test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return b.Encode(type, Data, Width * 4, Height * 4); //像素
        }

        // public static Image GetImagetable(string url, out string imageStrCookie)
        public static Image GetImageTable(string url)
        {
           // HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://121.5.171.21/dgiot_file/device/topo/");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();

            return Image.FromStream(response.GetResponseStream());
        }

        // 毫米转换成百分之一英寸
        public static int GetInch(float mm)
        {
            return (int)(mm * 0.0393700787402 * 100);
        }

        //pt 磅或者点数，是point简称 1磅=0.03527厘米=1/72英寸
        //inch英寸， 1英寸=2.54厘米=96像素(分辨率为96dpi)
        //px 像素, pixel的简称（本表参照显示器96dbi显示进行换算，像素不能出现小数点，一般是取小显示
        //---------------------------------------------------
        //|中文字号  | 英文字号（磅）| 毫米      |  像素    |
        //---------------------------------------------------
        //| 1英寸    |  72pt         | 25.30mm   |  95.6px  |
        //---------------------------------------------------
        //| 大特号   |  63pt         | 22.14mm   |  83.7px  |
        //---------------------------------------------------
        //| 特号     |  54pt         | 18.97mm   |  71.7px  |
        //---------------------------------------------------
        //| 初号     |  42pt         | 14.82mm   |  56px    |
        //---------------------------------------------------
        //| 小初     |  36pt         | 12.70mm   |  48px    |
        //---------------------------------------------------
        //| 一号     |  26pt         | 9.17mm    |  34.7px  |
        //---------------------------------------------------
        //| 小一     |  24pt         | 8.47mm    |  32px    |
        //---------------------------------------------------
        //| 二号     |  22pt         | 7.76mm    |  29.3px  |
        //---------------------------------------------------
        //| 小二     |  18pt         | 6.35mm    |  24px    |
        //---------------------------------------------------
        //| 三号     |  16pt         | 5.64mm    |  21.3px  |
        //---------------------------------------------------
        //| 小三     |  15pt         | 5.29mm    |  20px    |
        //---------------------------------------------------
        //| 四号     |  14pt         | 4.94mm    |  18.7px  |
        //---------------------------------------------------
        //| 小四     |  12pt         | 4.23mm    |  16px    |
        //---------------------------------------------------
        //| 五号     |  10.5pt       | 3.70mm    |  14px    |
        //---------------------------------------------------
        //| 小五     |  9pt          | 3.18mm    |  12px    |
        //---------------------------------------------------
        //| 六号     |  7.5pt        | 2.56mm    |  10px    |
        //---------------------------------------------------
        //| 小六     |  6.5pt        | 2.29mm    |  8.7px   |
        //---------------------------------------------------
        //| 七号     |  5.5pt        | 1.94mm    |  7.3px   |
        //---------------------------------------------------
        //| 八号     |  5pt          | 1.76mm    |  6.7px   |
        //---------------------------------------------------

        public static int pxToInch(float px)
        {
            return (int)(px / 96 * 100);
        }
    }
  }
