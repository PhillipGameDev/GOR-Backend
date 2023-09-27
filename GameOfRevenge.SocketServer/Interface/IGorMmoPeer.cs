using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Models.Quest;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace GameOfRevenge.Interface
{
    public interface IGorMmoPeer : IDisposable
    {
        IRpcProtocol Protocol { get; }
        PlayerInstance PlayerInstance { get; set; }


        SendResult SendOperation(OperationCode opCode, ReturnCode returnCode);
        SendResult SendOperation(OperationCode opCode, ReturnCode returnCode, string debuMsg);
        SendResult SendOperation(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data);
        SendResult SendOperation(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data, string debuMsg);

        SendResult SendOperation(byte opCode, ReturnCode returnCode);
        SendResult SendOperation(byte opCode, ReturnCode returnCode, string debuMsg);
        SendResult SendOperation(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data);
        SendResult SendOperation(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data, string debuMsg);

        SendResult Broadcast(OperationCode opCode, ReturnCode returnCode);
        SendResult Broadcast(OperationCode opCode, ReturnCode returnCode, string debuMsg);
        SendResult Broadcast(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data);
        SendResult Broadcast(OperationCode opCode, ReturnCode returnCode, Dictionary<byte, object> data, string debuMsg);

        SendResult Broadcast(byte opCode, ReturnCode returnCode);
        SendResult Broadcast(byte opCode, ReturnCode returnCode, string debuMsg);
        SendResult Broadcast(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data);
        SendResult Broadcast(byte opCode, ReturnCode returnCode, Dictionary<byte, object> data, string debuMsg);

        SendResult SendEvent(IEventData eventData, SendParameters sendParameters);
    }
}
