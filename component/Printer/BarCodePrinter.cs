using CodeLib;
using Dgiot_dtu;
using LitJson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dgiot_dtu.component.Printer
{
    class BarCodePrinter
    {
        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(String Name); //调用win api将指定名称的打印机设置为默认打印
        /*页面打印委托*/
        public delegate void DoPrintDelegate(Graphics g, ref bool HasMorePage);

        public PrintDocument iSPriner = null;
        bool m_bUseDefaultPaperSetting = false;

        DoPrintDelegate DoPrint = null;

        public BarCodePrinter()
        {
            iSPriner = new PrintDocument();
            iSPriner.PrintPage += new PrintPageEventHandler(this.OnPrintPage);
        }

        public void Dispose()
        {
            if (iSPriner != null) iSPriner.Dispose();
            iSPriner = null;

        }

        /*设置打印机名*/
        public string PrinterName
        {
            get { return iSPriner.PrinterSettings.PrinterName; }
            set { iSPriner.PrinterSettings.PrinterName = value; }
        }

        /*设置打印文档名*/
        public string DocumentName
        {
            get { return iSPriner.DocumentName; }
            set { iSPriner.DocumentName = value; }
        }

        /*设置是否使用缺省纸张*/
        public bool UseDefaultPaper
        {
            get { return m_bUseDefaultPaperSetting; }
            set
            {
                m_bUseDefaultPaperSetting = value;
                if (!m_bUseDefaultPaperSetting)
                {
                    //如果不适用缺省纸张则创建一个自定义纸张，注意，必须使用这个版本的构造函数才是自定义的纸张
                    //PaperSize ps = new PaperSize("Custom Size 1", 827, 1169);
                    //将缺省的纸张设置为新建的自定义纸张
                    //iSPriner.DefaultPageSettings.PaperSize = ps;
                }
            }
        }

        /*纸张宽度 单位定义为毫米mm*/
        public float PaperWidth
        {
            get { return iSPriner.DefaultPageSettings.PaperSize.Width / 100f * 25.4f; }
            set
            {
                LogHelper.Log("codeX: " + value);
                LogHelper.Log("Kind: " + iSPriner.DefaultPageSettings.PaperSize.Kind);
                //注意，只有自定义纸张才能修改该属性，否则将导致异常
                if (iSPriner.DefaultPageSettings.PaperSize.Kind == PaperKind.Custom)
                    iSPriner.DefaultPageSettings.PaperSize.Width = (int)(value / 25.4 * 100);
            }
        }

        /*纸张高度 单位定义为毫米mm*/
        public float PaperHeight
        {
            get { return (int)iSPriner.PrinterSettings.DefaultPageSettings.PaperSize.Height / 100f * 25.4f; }
            set
            {
                //注意，只有自定义纸张才能修改该属性，否则将导致异常
                if (iSPriner.DefaultPageSettings.PaperSize.Kind == PaperKind.Custom)
                    iSPriner.DefaultPageSettings.PaperSize.Height = (int)(value / 25.4 * 100);
            }
        }

        /*页面打印*/
        private void OnPrintPage(object sender, PrintPageEventArgs ev)
        {
            //调用委托绘制打印内容
            if (DoPrint != null)
            {
                bool bHadMore = false;
                DoPrint(ev.Graphics, ref bHadMore);
                ev.HasMorePages = bHadMore;
            }
            else
            {
                foreach (JsonData item in PrinterHelper.GetJson())
                {
                    if (item.ContainsKey("type"))
                    {
                        String type = item["type"].ToString();
                        if (type == "paper" || type == "printer" || type.Equals("scancodetext"))
                        {
                        }
                        else if (item.ContainsKey("text")
                            && item.ContainsKey("fontFamily")
                            && item.ContainsKey("fontSize")
                            && item.ContainsKey("x")
                            && item.ContainsKey("y")
                            && item.ContainsKey("width")
                            && item.ContainsKey("height"))
                        {
                            String text = item["text"].ToString();
                            String fontFamily = item["fontFamily"].ToString();
                            int fontSize = int.Parse(item["fontSize"].ToJson());
                            float x = float.Parse(item["x"].ToJson());
                            float y = float.Parse(item["y"].ToJson());
                            float width = float.Parse(item["width"].ToJson());
                            float height = float.Parse(item["height"].ToJson());
                            if (type == "scancode")
                            {
                                Image image = GetBarCode(text, fontSize, (int)width, (int)height, fontFamily);
                                ev.Graphics.DrawImage(image, x + 10, y + 10, width, height); //单位1/100英寸
                            }
                            else
                            {
                                //LogHelper.Log("type11: " + type);
                                Font font = new Font(new FontFamily(fontFamily), fontSize);
                                ev.Graphics.DrawString(text, font, Brushes.Black, x, y);
                            }
                        }
                        else
                        {

                        }
                    }      
                }
                ev.HasMorePages = false;
            }
        }

        public Image GetBarCode(String Data, int FontSize, int Width, int Height, String familyName)
        {
            TYPE type = CodeLib.TYPE.CODE128B;
            Font Label = new Font(familyName, FontSize * 4);
            AlignmentPositions Align = AlignmentPositions.CENTER;
            Barcode b = new CodeLib.Barcode();
            b.IncludeLabel = true;
            b.LabelFont = Label;
            b.Alignment = Align;
            b.LabelPosition = LabelPositions.BOTTOMCENTER;
            b.Encode(type, Data, Width * 4, Height * 4).Save("./test.jpg", ImageFormat.Jpeg);
            return b.Encode(type, Data, Width * 4, Height * 4); //像素
        }

        /* 开始打印*/
        public void Print(DoPrintDelegate doPrint)
        {
            DoPrint = doPrint;
            this.iSPriner.Print();
        }

        /* 开始打印*/
        public void doPrint()
        {
            this.SetPaperSize();
            this.iSPriner.Print();
        }

        /* 设置纸大小*/
        public void SetPaperSize()
        {  
            foreach (JsonData item in PrinterHelper.GetJson())
            {
                if (item.ContainsKey("type"))
                {
                    String type = item["type"].ToString();
                    if (type == "paper")
                    {
                        if (item.ContainsKey("width") && item.ContainsKey("height"))
                        {
                            int paperwidth = Knova.pxToInch(int.Parse(item["width"].ToJson()));
                            int paperheight = Knova.pxToInch(int.Parse(item["height"].ToJson()));
                            iSPriner.DefaultPageSettings.PaperSize = new PaperSize("Size", paperwidth, paperheight); //单位1/100英寸
                        }                        
                    }
                    else if (type == "printer")
                    {
                        if (item.ContainsKey("text"))
                        {
                            String text = item["text"].ToString();
                            LogHelper.Log("printer: " + text);
                            SetDefaultPrinter(text);
                        }
                    }

                }             
            }
        }
    }
}
