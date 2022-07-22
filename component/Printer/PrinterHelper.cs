// <copyright file="AccessHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu {
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

internal class PrinterHelper {

  [DllImport("winspool.drv")]
  public static extern bool
  SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印

  private static PrinterHelper instance = null;
  private static bool bIsRunning = false;

  private static string textData = "C01010100005DGF0100189";
  private static PrintDocument fPrintDocument = new PrintDocument();

  public static void Start(MqttClient mqttClient) {

    string Name = "ec71804a3d";
    mqttClient.SubscribeAsync(new TopicFilter(
        "device/printer", MqttQualityOfServiceLevel.AtLeastOnce));
    LogHelper.Log("mqtt client subscribe topic: " + "device/" + Name + "/#");
  }

  public static PrinterHelper GetInstance() {
    if (instance == null) {
      instance = new PrinterHelper();
    }

    return instance;
  }

  public static void Start(KeyValueConfigurationCollection config,
                           bool bIsRunning) {
    Config(config);
    PrinterHelper.bIsRunning = bIsRunning;
  }

  public static void Stop() { PrinterHelper.bIsRunning = false; }

  public static void Config(KeyValueConfigurationCollection config) {}

  Brush brushHeaderFont = new SolidBrush(Color.Black);
  Pen LinePen = new Pen(Color.Black, 1);
  public PrinterHelper() { InitPrint(); }
  /// <summary>
  /// 初始化打印机
  /// </summary>
  private void InitPrint() {}

  /// <summary>
  /// 打印
  /// </summary>
  public void Print() {}

  public static void SetTextData(string data) { textData = data; }

  public static void PrintPage(string json) {
    SetTextData(json);
    PrintDialog PD = new PrintDialog();
    PageSettings pageSettings = new PageSettings();
    // pageSettings.PaperSize = new PaperSize("Size", 30, 40);
    string Name = fPrintDocument.PrinterSettings.PrinterName;
    System.Drawing.Printing.PrinterSettings Ps =
        new System.Drawing.Printing.PrinterSettings();
    // SetDefaultPrinter("Deli DL-888B(NEW)");
    fPrintDocument.DefaultPageSettings.PaperSize =
        new PaperSize("Size", 157, 118); //单位1/100英寸
    SetDefaultPrinter(Name);
    PD.PrinterSettings = Ps;
    fPrintDocument.PrintPage += document_PrintPage;
    PD.Document = fPrintDocument;
    fPrintDocument.Print();
  }

  static void document_PrintPage(object sender, PrintPageEventArgs e) {
    int x1 = -5;
    int y1 = 15;
    double x2 = (260 * 0.63) + x1;
    double y2 = (140 * 0.63) + y1;
    e.Graphics.DrawImage(PrinterHelper.Get_image(), x1, y1, (int)x2, (int)y2);
    e.HasMorePages = false;
  }

  public static Image Get_image() {
    CodeLib.Barcode b = new CodeLib.Barcode();
    CodeLib.TYPE type = CodeLib.TYPE.CODE128B;
    b.IncludeLabel = true;
    string familyName = "宋体";
    Font Label = new Font(familyName, 40.0F);
    b.LabelFont = Label;
    CodeLib.AlignmentPositions Align = CodeLib.AlignmentPositions.CENTER;
    b.Alignment = Align;
    b.LabelPosition = CodeLib.LabelPositions.BOTTOMCENTER;
    return b.Encode(type, textData, 523, 200);
  }
}
}