using System.Text;
//using log4net;

namespace PortListener.Core.Utilities
{
    public class StringHelper
    {
        #region Fields

        //private static readonly ILog _objLogger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        #endregion

        /// <summary>
        /// Converts an ASCII string to it's string representation as hex bytes
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHexString(string s)
        {
            return ToHexString( Encoding.ASCII.GetBytes(s) );
        }

        public static string ToHexString(byte[] arrBytes)
        {
            return ToHexString(arrBytes, 0, arrBytes.Length);
        }

        /// <summary>
        /// Converts an array of bytes to a string of hex bytes
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <param name="iOffset"></param>
        /// <param name="iLength"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] arrBytes, int iOffset, int iLength)
        {
            if (arrBytes.Length == 0)
                return "";

            var sb = new StringBuilder("");

            var i = 0;
            for (; i < iLength - 1; i++)
            {
                sb.AppendFormat("{0:X2} ", arrBytes[i]);
            }
            sb.AppendFormat("{0:X2}", arrBytes[i]);

            return sb.ToString();

        }

        public static string ToHexString(byte data)
        {
            return string.Format("{0:X2}", data);
        }
    }
}
