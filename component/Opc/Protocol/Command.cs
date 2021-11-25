// <copyright file="Command.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Da
{
    public enum Command
    {
        Get_Nodes_Req = 0x400000,  //
        Get_Nodes_Rsp = 0x400001,

        Start_Monitor_Nodes_Req = 0x400002,
        Start_Monitor_Nodes_Rsp = 0x400003,

        Stop_Monitor_Nodes_Req = 0x400004,
        Stop_Monitor_Nodes_Rsp = 0x400005,

        Read_Nodes_Values_Req = 0x400006,
        Read_Nodes_Values_Rsp = 0x400007,

        Write_Nodes_Values_Req = 0x400008,
        Write_Nodes_Values_Rsp = 0x400009,

        Notify_Nodes_Values_Ex = 0x400999,
    }

    public enum MonitorItemType
    {
        Initial,
        Add,
        Remove,
    }
}
