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

    internal partial class PrinterHelper
    {
        private static PrinterHelper instance = null;
        private static bool bIsRunning = false;
        private static PdfPrinter pdfPriner = null;
        private static BarCodePrinter barCodePriner = null;
        private static JsonData json = new JsonData();

        public static PrinterHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new PrinterHelper();
                pdfPriner = new PdfPrinter();
                barCodePriner = new BarCodePrinter();
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
            //sDefault = sys.print_machine;//获取设置的默认打印机
            foreach (string sPrint in PrinterSettings.InstalledPrinters)//获取所有打印机名称
            {
                LogHelper.Log("Data: " + sPrint);
            }
        }
    }
}
