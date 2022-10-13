using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace dgiot_dtu.component.Printer
{
    class PdfPrinter
    {
        /*页面打印委托*/
        public delegate void DoPrintDelegate(Graphics g, ref bool HasMorePage);

        PrintDocument iSPriner = null;
        bool m_bUseDefaultPaperSetting = false;

        DoPrintDelegate DoPrint = null;

        public PdfPrinter()
        {
            iSPriner = new PrintDocument();
            iSPriner.PrintPage += new PrintPageEventHandler
                (this.OnPrintPage);
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
                    PaperSize ps = new PaperSize("Custom Size 1", 827, 1169);
                    //将缺省的纸张设置为新建的自定义纸张
                    iSPriner.DefaultPageSettings.PaperSize = ps;
                }
            }
        }

        /*纸张宽度 单位定义为毫米mm*/
        public float PaperWidth
        {
            get { return iSPriner.DefaultPageSettings.PaperSize.Width / 100f * 25.4f; }
            set
            {
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
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(@"D:\1.pdf");
                doc.PrintDocument.Print();
            }
        }

        /* 开始打印*/
        public void Print(DoPrintDelegate doPrint)
        {
            DoPrint = doPrint;
            this.iSPriner.Print();
        }

        /* 开始打印*/
        public void Print()
        { 
            this.iSPriner.Print();
        }

        // public static Image GetImagetable(string url, out string imageStrCookie)
        public static Image GetPdf(string url)
        {
            // HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://121.5.171.21/dgiot_file/device/topo/");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            return Image.FromStream(response.GetResponseStream());
        }
    }
}
