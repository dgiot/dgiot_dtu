// <copyright file="AccessHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Configuration;
    using LitJson;
    using Spire.License;
    using Spire.Pdf;
    using dgiot_dtu.component.Printer;
    using System.Drawing.Printing;
    using static System.Drawing.Printing.PrinterSettings;
    using System.Text;

    internal partial class PrinterHelper
    {
        private static PrinterHelper instance = null;
        private static bool bIsRunning = false;
        private static PdfPrinter pdfPriner = null;
        private static BarCodePrinter barCodePriner = null;
        private static JsonData json = new JsonData();
        private static string productId = string.Empty;
        private static string devAddr = string.Empty;

        public static PrinterHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new PrinterHelper();
                pdfPriner = new PdfPrinter();
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

        public static JsonData GetJson()
        {
            return json; 
        }

        private static void SetJson(JsonData jsonData)
        {
            json.Clear();
            json = jsonData;
        }

        public static void PrintBarCode(JsonData jsonData)
        {
            SetJson(jsonData);
            barCodePriner = new BarCodePrinter();
            barCodePriner.doPrint();
        }

        public static void PrintPdf(JsonData jsonData)
        {
            SetJson(jsonData);
            pdfPriner.Print();
            GetPrinter();
        }

        public static void GetPrinter()
        {
            productId = ConfigHelper.GetConfig("MqttUserName");
            devAddr = ConfigHelper.GetConfig("DtuAddr");
            //sDefault = sys.print_machine;//获取设置的默认打印机
            JsonObject Printers = new JsonObject();
           
            foreach (string sPrint in PrinterSettings.InstalledPrinters)//获取所有打印机名称
            {
                // LogHelper.Log("Printers: " + sPrint);
                Printers.Add(sPrint, sPrint);
            }
            StringCollection Printer = PrinterSettings.InstalledPrinters;
            
            string topic = "$dg/thing/" + productId + "/" + devAddr + "/firmware/report";
            MqttClientHelper.Publish(topic, Encoding.UTF8.GetBytes(Printers.ToString()));
        }
    }
}
