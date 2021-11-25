// <copyright file="Header.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    public struct Header
    {
        public int Id;
        public int Cmd;
        public int ErrorCode;
        public int ContentSize;

        public Header(int id, int cmd, int errorcode, int payloadLength)
        {
            Id = id;
            Cmd = cmd;
            ErrorCode = errorcode;
            ContentSize = payloadLength;
        }
    }
}
