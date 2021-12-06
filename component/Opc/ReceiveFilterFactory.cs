// <copyright file="ReceiveFilterFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    using System.Net;
    using SuperSocket.SocketBase;
    using SuperSocket.SocketBase.Protocol;

    public class ReceiveFilterFactory : IReceiveFilterFactory<StringRequestInfo>
    {
        // private ReceiveFilter _receiveFilter = new ReceiveFilter();
        public IReceiveFilter<StringRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new ReceiveFilter();
        }
    }
}
