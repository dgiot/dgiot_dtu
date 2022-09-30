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

    internal class PrinterHelper
    {

        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印

        private static PrinterHelper instance = null;
        private static bool bIsRunning = false;

        private static string textData = "C01010100005DGF0100189";
        private static int  num = 0;
        private static List<Dictionary<string, object>> scoreList = new List<Dictionary<string, object>>();
        private static Dictionary<string, string> dictionary = new Dictionary<string, string>();
        private static PrintDocument fPrintDocument = new PrintDocument();





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


        public static void PrintPage(Dictionary<string, object> json)
        {
            Clearjson();
     
            foreach (string key in json.Keys)
            {
          if(int.Parse(key)> num)
                {
                    num = int.Parse(key);
                }
            }
            for (int x=1; x<=num;x++)
            {
                scoreList.Add((Dictionary<string, object>)json[x.ToString()]);
            }



            num = 0;



            PrintDialog PD = new PrintDialog();
            PageSettings pageSettings = new PageSettings();
            //pageSettings.PaperSize = new PaperSize("Size", 30, 40);
            string Name = fPrintDocument.PrinterSettings.PrinterName;
            LogHelper.Log("PrinterName: " + Name.ToString());
            System.Drawing.Printing.PrinterSettings Ps = new System.Drawing.Printing.PrinterSettings();
            //SetDefaultPrinter("Deli DL-888B(NEW)");
            fPrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Size", 393, 275); //单位1/100英寸
            SetDefaultPrinter(Name);
            PD.PrinterSettings = Ps;
            fPrintDocument.PrintPage += document_PrintPage;
            PD.Document = fPrintDocument;
            fPrintDocument.Print();
        }

        static void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            int x1 = -5;
            int y1 = 15;
            double x2= (260 * 0.63)+x1;
            double y2 = (140 * 0.63) + y1;

        
            foreach (Dictionary<string, object> body in scoreList) {
                if (body["name"].ToString() == "barcode") {
                    e.Graphics.DrawImage(PrinterHelper.Get_image(body["label"].ToString()), int.Parse(body["x"].ToString()), int.Parse(body["y"].ToString()), int.Parse(body["height"].ToString()), int.Parse(body["width"].ToString()));
                }
                else
                {
                    e.Graphics.DrawString(body["label"].ToString(), new Font(new FontFamily(body["font"].ToString()), int.Parse(body["size"].ToString())), System.Drawing.Brushes.Black, int.Parse(body["x"].ToString()), int.Parse(body["y"].ToString()));
                }


            }
            
            
 

           

            e.HasMorePages = false;
        }

        public static Image Get_image(String Data)
        {
            CodeLib.Barcode b = new CodeLib.Barcode();
            CodeLib.TYPE type = CodeLib.TYPE.CODE128B;
            b.IncludeLabel = true;
            string familyName = "宋体";
            Font Label  = new Font(familyName, 10.0F);
            b.LabelFont = Label;
            CodeLib.AlignmentPositions Align = CodeLib.AlignmentPositions.CENTER;
            b.Alignment = Align;
            b.LabelPosition = CodeLib.LabelPositions.BOTTOMCENTER;
            b.Encode(type, textData, 300, 100).Save("./test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return b.Encode(type, Data, 157, 80);  
        }

       // public static Image GetImagetable(string url, out string imageStrCookie)
        public static Image GetImageTable(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://121.5.171.21/dgiot_file/device/topo/");

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();

            return Image.FromStream(response.GetResponseStream());
        }
    }
  }
