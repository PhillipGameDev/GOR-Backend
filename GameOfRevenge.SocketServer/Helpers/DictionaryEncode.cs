using Photon.SocketServer.Rpc;
using GameOfRevenge.Model;
using System.Collections.Generic;
using System;

namespace GameOfRevenge.Helpers
{
    public abstract class DictionaryEncode
    {
        public bool IsValid { get; private set; } = true;
        public string ErrorMessage { get; private set; }

        [DataMember(Code = (byte)RoomParameterKey.CommonMessage, IsOptional = false)]
        public string Message { get; set; } = "OK";

        [DataMember(Code = (byte)RoomParameterKey.IsSuccess, IsOptional = false)]
        public bool IsSuccess { get; set; } = true;

        public void SetInvalid(string message)
        {
            IsValid = false;
            ErrorMessage = message;
        }

        public Dictionary<byte,object> GetDictionary()
        {
            var type = GetType();
            var properties = type.GetProperties();
            var operations = new Dictionary<byte, object>();

            foreach (var property in properties)
            {
                var attributes = (DataMemberAttribute[])property.GetCustomAttributes(typeof(DataMemberAttribute), true);
                if (attributes.Length > 0)
                {
                    var paramCode = attributes[0].Code;
                    var isOptional = attributes[0].IsOptional;
                    if (!operations.ContainsKey(paramCode))
                    {
                        if (property.CanRead)
                        {
                            try
                            {
                                var value = property.GetValue(this);
                                if (value != null)
                                {
                                    operations.Add(paramCode, value);
                                }
                                else if (isOptional == false)
                                {
                                    SetInvalid("model instance is not valid.");
                                }
                            }
                            catch (Exception ex)
                            {
                                SetInvalid(ex.Message + ex.StackTrace);
                                break;
                            }
                        }
                    }
                    else if (isOptional == false)
                    {
                        SetInvalid("model is not valid.");
                        break;
                    }
                }
            }

            if (operations != null && operations.Count <= 0) operations = null;
            return operations;
        }
    }
}
