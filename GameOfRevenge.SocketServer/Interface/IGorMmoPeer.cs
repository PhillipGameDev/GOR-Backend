using System;
using System.Collections.Generic;
using Photon.SocketServer;
using GameOfRevenge.Model;
using GameOfRevenge.Common.Net;
using GameOfRevenge.GameHandlers;

namespace GameOfRevenge.Interface
{
    public interface IGorMmoPeer : IDisposable
    {
        IRpcProtocol Protocol { get; }
        PlayerInstance PlayerInstance { get; set; }


/*        SendResult SendEventTo(List<int> players, IEventData opCode, ReturnCode returnCode, string debuMsg);
        SendResult SendEventTo(List<int> players, IEventData opCode, ReturnCode returnCode, Dictionary<byte, object> data = null, string debuMsg = null);

        SendResult SendEventTo(List<int> players, byte opCode, ReturnCode returnCode, string debuMsg);
        SendResult SendEventTo(List<int> players, byte opCode, ReturnCode returnCode, Dictionary<byte, object> data = null, string debuMsg = null);
*/
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

        SendResult SendEvent(IEventData eventData);
    }
}
