// <copyright file="ControlHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Automation;

    public class ControlHelper
    {
        private ControlHelper()
        {
        }

        #region 获取另一系统文本框值
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "FindEx")]
        public static extern IntPtr FindEx(IntPtr hwnd, IntPtr hwndChild, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);
        #endregion

        public static int WMSETTEXT = 0x000C; // 应用程序发送此消息来设置一个窗口的文本
        public static int WMGETTEXT = 0x000D; // 应用程序发送此消息来复制对应窗口的文本到缓冲区
        public static int WMGETTEXTLENGTH = 0x000E; // 得到与一个窗口有关的文本的长度（不包含空字符
        public static int WMLBUTTONDOWN = 0x201; // Left mousebutton down
        public static int WMLBUTTONUP = 0x202; // Left mousebutton up
        public static int WMLBUTTONDBLCLK = 0x203; // Left mousebutton doubleclick
        public static int WMRBUTTONDOWN = 0x204; // Right mousebutton down
        public static int WMRBUTTONUP = 0x205; // Right mousebutton up
        public static int WMRBUTTONDBLCLK = 0x206; // Right mousebutton doubleclick
        public static int WMKEYDOWN = 0x100; // Key down
        public static int WMKEYUP = 0x101; // Key up

        private static ControlHelper instance;
        private static MainForm mainform = null;
        private static bool bIsRunning = false;
        private static bool bIsCheck = false;

        public static ControlHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new ControlHelper();
            }

            return instance;
        }

        public static void Start(KeyValueConfigurationCollection config, bool bIsRunning, MainForm mainform)
        {
            Config(config, mainform);
            ControlHelper.bIsRunning = bIsRunning;
        }

        public static void Stop()
        {
           ControlHelper.bIsRunning = false;
        }

        public static void Config(KeyValueConfigurationCollection config, MainForm mainform)
        {
            if (config["ControlIsCheck"] != null)
            {
                ControlHelper.bIsCheck = DgiotHelper.StrTobool(config["ControlIsCheck"].Value);
            }

            ControlHelper.mainform = mainform;
        }

        public static void Check(bool isCheck, MainForm mainform)
        {
            ControlHelper.bIsCheck = isCheck;
            ControlHelper.mainform = mainform;
        }

        public static void Do_control(Dictionary<string, object> json)
        {
            string cmdType = "read";
            string windowName = "";
            string controlType = "all";
            string handle = "null";
            var example = new ControlHelper();
            JsonObject result = new JsonObject();
            JsonObject control = new JsonObject();

            result.Add("TimeStamp", FromDateTime(DateTime.UtcNow));

            // result.Add("LocalDate", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            if (json.ContainsKey("cmdtype"))
            {
                try
                {
                    cmdType = (string)json["cmdtype"];
                    if (json.ContainsKey("windowname"))
                    {
                        windowName = (string)json["windowname"];
                    }

                    if (json.ContainsKey("controltype"))
                    {
                        controlType = (string)json["controltype"];
                    }

                    if (json.ContainsKey("handle"))
                    {
                        handle = (string)json["handle"];
                    }

                    switch (cmdType)
                    {
                        case "Read":
                            example.ReadControl(windowName, controlType, control);
                            result.Add("Control", control);
                            break;
                        case "Scan":
                            result.Add("Start", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                            example.ScanControl(windowName, control);
                            result.Add("End", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                            result.Add("Control", control);
                            break;
                        case "Invoke":
                            example.InvokeControl(windowName, Convert.ToInt32(handle), control);
                            result.Add("Control", control);

                            break;
                        case "Screen":
                            example.ScreenWindows(control);
                            result.Add("Control", control);
                            break;
                        default:
                            // Console.WriteLine("shuwa_capture Target Read|Scan|Invoke [Filter]");
                            break;
                    }

                    Console.WriteLine(result.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}", ex.ToString());
                }
            }
        }

        private void ScreenWindows(JsonObject json)
        {
            AutomationElement desktop = AutomationElement.RootElement;
            Condition conditions = new AndCondition(
              new PropertyCondition(AutomationElement.IsEnabledProperty, true),
              new PropertyCondition(AutomationElement.ControlTypeProperty,
                  ControlType.Window));

            // Find all children that match the specified conditions.
            AutomationElementCollection elementCollection =
                desktop.FindAll(TreeScope.Children, conditions);
            for (int i = 0; i < elementCollection.Count; i++)
            {
                JsonObject obj = new JsonObject();
                obj.Add("Type", elementCollection[i].Current.ControlType.ProgrammaticName.Split('.')[1]);
                object boundingRectNoDefault =
                    elementCollection[i].GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);
                if (boundingRectNoDefault != AutomationElement.NotSupported)
                {
                    string str = SimpleJson.SerializeObject(boundingRectNoDefault);
                    JsonObject rect = (JsonObject)SimpleJson.DeserializeObject(str);
                    rect.Remove("Location");
                    rect.Remove("Size");
                    rect.Remove("BottomLeft");
                    rect.Remove("TopLeft");
                    rect.Remove("TopRight");
                    rect.Remove("BottomRight");
                    rect.Remove("BottomLeft");
                    rect.Remove("IsEmpty");

                    // rect.Remove("Width");
                    // rect.Remove("Height");
                    // rect.Remove("X");
                    // rect.Remove("Y");
                    rect.Remove("Left");
                    rect.Remove("Top");
                    rect.Remove("Right");
                    rect.Remove("Bottom");
                    obj.Add("Rect", rect);
                }

                IntPtr hwndp = (IntPtr)elementCollection[i].Current.NativeWindowHandle;
                obj.Add("Name", elementCollection[i].Current.Name);
                json.Add("" + elementCollection[i].Current.NativeWindowHandle, obj);
            }
        }

        private void InvokeControl(string windowName, int handler, JsonObject json)
        {
            Condition condition1 = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            Condition condition2 = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
            Condition condition = new AndCondition(condition1, condition2);

            AutomationElementCollection windowsCollection = AutomationElement.RootElement.FindAll(
                TreeScope.Children,
                new AndCondition(
                    new PropertyCondition(AutomationElement.IsEnabledProperty, true),
                    new PropertyCondition(AutomationElement.NameProperty, windowName),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)));

            for (int i = 0; i < windowsCollection.Count; i++)
            {
                if (this.WalkInvokeControl(windowsCollection[i], handler, json))
                {
                    break;
                }
            }
        }

        private bool WalkInvokeControl(AutomationElement rootElement, int handler, JsonObject json)
        {
            // Conditions for the basic views of the subtree (content, control, and raw)
            // are available as fields of TreeWalker, and one of these is used in the
            // following code.
            AutomationElement elementNode = TreeWalker.ControlViewWalker.GetFirstChild(rootElement);
            while (elementNode != null)
            {
                if (elementNode.Current.NativeWindowHandle == handler)
                {
                    this.InvokeControl(elementNode);
                    json.Add("Invoke", "successed");
                    return true;
                }
                else
                {
                    this.WalkInvokeControl(elementNode, handler, json);
                    elementNode = TreeWalker.ControlViewWalker.GetNextSibling(elementNode);
                }
            }

            return false;
        }

        /// --------------------------------------------------------------------
        /// <summary>
        /// Obtains an InvokePattern control pattern from a control
        /// and calls the InvokePattern.Invoke() method on the control.
        /// </summary>
        /// <param name="targetControl">
        /// The control of interest.
        /// </param>
        /// --------------------------------------------------------------------
        private void InvokeControl(AutomationElement targetControl)
        {
            InvokePattern invokePattern = null;
            try
            {
                invokePattern =
                    targetControl.GetCurrentPattern(InvokePattern.Pattern)
                    as InvokePattern;
            }
            catch (ElementNotEnabledException)
            {
                // Object is not enabled
                return;
            }
            catch (InvalidOperationException)
            {
                // object doesn't support the InvokePattern control pattern
                return;
            }

            invokePattern.Invoke();
        }

        private void ReadControl(string windowsName, String controlType, JsonObject json)
        {
            Condition condition1 = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            Condition condition2 = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
            Condition condition = new AndCondition(condition1, condition2);
            AutomationElement root = AutomationElement.RootElement;
            AutomationElement window = root.FindFirst(TreeScope.Children,
                new AndCondition(
                    new PropertyCondition(AutomationElement.IsEnabledProperty, true),
                    new PropertyCondition(AutomationElement.NameProperty, windowsName),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)));
            if (null != window)
            {
                        WalkControlElements(window, controlType, json);
            }
        }

        private void WalkControlElements(AutomationElement rootElement, string controlType, JsonObject json)
        {
            AutomationElement elementNode = TreeWalker.ControlViewWalker.GetFirstChild(rootElement);
            while (elementNode != null)
            {
                IntPtr hwndp = (IntPtr)elementNode.Current.NativeWindowHandle;
                if (hwndp != IntPtr.Zero &&
                    (controlType.Equals("all") || elementNode.Current.ControlType.ProgrammaticName.Equals("ControlType." + controlType)))
                {
                    const int buffer_size = 256;
                    StringBuilder buffer = new StringBuilder(buffer_size);
                    SendMessage(hwndp, WMGETTEXT, buffer_size, buffer);
                    json.Add("" + elementNode.Current.NativeWindowHandle, buffer.ToString());
                }

                WalkControlElements(elementNode, controlType, json);
                elementNode = TreeWalker.ControlViewWalker.GetNextSibling(elementNode);
            }
        }

        private void ScanControl(string windowsName, JsonObject json)
        {
            Condition condition1 = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            Condition condition2 = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
            Condition condition = new AndCondition(condition1, condition2);
            AutomationElement root = AutomationElement.RootElement;
            AutomationElement window = root.FindFirst(TreeScope.Children,
                new AndCondition(
                    new PropertyCondition(AutomationElement.IsEnabledProperty, true),
                    new PropertyCondition(AutomationElement.NameProperty, windowsName),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)));
            if (null != window)
            {
                GetEditText(window, 0, 1, root.Current.NativeWindowHandle, "root_desktop", json);
                this.WalkControlElements(window, 1, window.Current.Name, json);
            }
        }

        private void WalkControlElements(AutomationElement rootElement, int level, String parent, JsonObject json)
        {
            // Conditions for the basic views of the subtree (content, control, and raw)
            // are available as fields of TreeWalker, and one of these is used in the
            // following code.
            AutomationElement grandelementNode = null;
            AutomationElement elementNode = TreeWalker.ControlViewWalker.GetFirstChild(rootElement);
            if (level > 1)
            {
                grandelementNode = TreeWalker.ControlViewWalker.GetParent(rootElement);
            }

            int index = 0;
            while (elementNode != null)
            {
                index++;
                if (rootElement.Current.ControlType.ProgrammaticName.Split('.')[1] == "Group")
                {
                    GetEditText(elementNode, level, index, grandelementNode.Current.NativeWindowHandle, parent, json);
                }
                else
                {
                    GetEditText(elementNode, level, index, rootElement.Current.NativeWindowHandle, parent, json);
                }

                WalkControlElements(elementNode, level + 1,  elementNode.Current.Name, json);
                elementNode = TreeWalker.ControlViewWalker.GetNextSibling(elementNode);
            }
        }

        public static void GetEditText(AutomationElement elementNode, int level, int index, int parnetHandle, String parent, JsonObject json)
        {
            JsonObject obj = new JsonObject();
            obj.Add("Type", elementNode.Current.ControlType.ProgrammaticName.Split('.')[1]);
            obj.Add("Level", level);
            obj.Add("Index", index);
            object boundingRectNoDefault =
                elementNode.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);
            if (boundingRectNoDefault != AutomationElement.NotSupported)
            {
                String str = SimpleJson.SerializeObject(boundingRectNoDefault);
                JsonObject rect = (JsonObject)SimpleJson.DeserializeObject(str);
                rect.Remove("Location");
                rect.Remove("Size");
                rect.Remove("BottomLeft");
                rect.Remove("TopLeft");
                rect.Remove("TopRight");
                rect.Remove("BottomRight");
                rect.Remove("BottomLeft");
                rect.Remove("IsEmpty");
                rect.Remove("IsEmpty");

                // rect.Remove("Width");
                // rect.Remove("Height");
                // rect.Remove("X");
                // rect.Remove("Y");
                rect.Remove("Left");
                rect.Remove("Top");
                rect.Remove("Right");
                rect.Remove("Bottom");
                obj.Add("Rect", rect);
            }

            IntPtr hwndp = (IntPtr)elementNode.Current.NativeWindowHandle;
            obj.Add("PName", parent);
            obj.Add("PHandle", "" + parnetHandle);
            obj.Add("Name", elementNode.Current.Name);
            if (hwndp != IntPtr.Zero)
            {
                const int buffer_size = 256;
                StringBuilder buffer = new StringBuilder(buffer_size);
                SendMessage(hwndp, WMGETTEXT, buffer_size, buffer);
                if (buffer.ToString().Equals(""))
                {
                    obj.Add("Value", elementNode.Current.Name);
                }
                else
                {
                    obj.Add("Value", buffer.ToString());
                }

                json.Add("" + elementNode.Current.NativeWindowHandle, obj);
            }
            else
            {
                obj.Add("Value", elementNode.Current.Name);
                if (parnetHandle != 0)
                {
                    json.Add("" + parnetHandle + "_" + index, obj);
                }
            }
        }

        private static DateTime baseTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 将unixtime转换为.NET的DateTime
        /// </summary>
        /// <param name="timeStamp">秒数</param>
        /// <returns>转换后的时间</returns>
        public static DateTime FromUnixTime(long timeStamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime((timeStamp * 10000000) + baseTime.Ticks));
        }

        /// <summary>
        /// 将.NET的DateTime转换为unix time
        /// </summary>
        /// <param name="dateTime">待转换的时间</param>
        /// <returns>转换后的unix time</returns>
        public static long FromDateTime(DateTime dateTime)
        {
            return (TimeZone.CurrentTimeZone.ToUniversalTime(dateTime).Ticks - baseTime.Ticks) / 10000000;
        }
    }
}