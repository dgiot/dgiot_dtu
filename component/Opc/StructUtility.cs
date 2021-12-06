// <copyright file="StructUtility.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace Da
{
    public class StructUtility
    {
        public static byte[] Package(Header header, string xml)
        {
            byte[] txtBuffer = System.Text.Encoding.ASCII.GetBytes(xml);
            header.ContentSize = txtBuffer.Length;
            byte[] headerBuffer = StructToBytes(header, Marshal.SizeOf(header));
            byte[] msg = new byte[headerBuffer.Length + txtBuffer.Length];
            Buffer.BlockCopy(headerBuffer, 0, msg, 0, headerBuffer.Length);
            Buffer.BlockCopy(txtBuffer, 0, msg, headerBuffer.Length, txtBuffer.Length);
            return msg;
        }

        public static void Unpackage(byte[] buffer, out Header header, out string xml)
        {
            int size = Marshal.SizeOf(typeof(Header));
            header = BytesToStruct<Header>(buffer, 0, size);
            byte[] msg = new byte[header.ContentSize];
            Buffer.BlockCopy(buffer, size, msg, 0, (int)header.ContentSize);
            xml = System.Text.Encoding.ASCII.GetString(msg);
        }

        private static byte[] StructToBytes(object structObj, int size)
        {
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static T BytesToStruct<T>(byte[] bytes, int startIndex, int length)
        {
            if (bytes == null)
            {
                return default(T);
            }

            if (bytes.Length <= 0)
            {
                return default(T);
            }

            IntPtr buffer = Marshal.AllocHGlobal(length);
            try
            {
                Marshal.Copy(bytes, startIndex, buffer, length);
                return (T)Marshal.PtrToStructure(buffer, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
