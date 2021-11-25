// <copyright file="ReceiveFilter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using SuperSocket.Facility.Protocol;
    using SuperSocket.SocketBase.Protocol;

    public class ReceiveFilter : FixedHeaderReceiveFilter<StringRequestInfo>
    {
        public ReceiveFilter() : base(Marshal.SizeOf(typeof(Header)))
        {
        }

        protected override int GetBodyLengthFromHeader(byte[] buffer, int offset, int length)
        {
            byte[] dstBuffer = new byte[length];
            Buffer.BlockCopy(buffer, offset, dstBuffer, 0, length);
            Header protocolHeader = StructUtility.BytesToStruct<Header>(dstBuffer, 0, length);
            return protocolHeader.ContentSize;
        }

        protected override StringRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            string content = string.Empty;
            if (bodyBuffer != null)
            {
                byte[] dstBuffer = new byte[length];
                Buffer.BlockCopy(bodyBuffer, offset, dstBuffer, 0, length);
                content = System.Text.Encoding.Default.GetString(dstBuffer);
            }

            byte[] dstHeaderBuffer = new byte[header.Count()];
            Buffer.BlockCopy(header.ToArray(), 0, dstHeaderBuffer, 0, header.Count());
            Header protocolHeader = StructUtility.BytesToStruct<Header>(dstHeaderBuffer, 0, header.Count());

            return new StringRequestInfo(protocolHeader.Id.ToString(), content, new string[] { protocolHeader.Cmd.ToString() });
        }
    }
}