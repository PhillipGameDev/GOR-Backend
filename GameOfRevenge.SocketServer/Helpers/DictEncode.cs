using System;
using System.Collections.Generic;
using Photon.SocketServer.Rpc;
using GameOfRevenge.Model;

namespace GameOfRevenge.Helpers
{
    [Obsolete]
    public abstract class DictEncode
    {
        private readonly Dictionary<byte, object> operations;

        public bool IsValid { get; private set; } = true;
        public string ErrorMessage { get; private set; }

        [DataMember(Code = (byte)RoomParameterKey.CommonMessage, IsOptional = false)]
        public string Message { get; set; } = "OK";

        [DataMember(Code = (byte)RoomParameterKey.IsSuccess, IsOptional = false)]
        public bool IsSuccess { get; set; } = true;

        public DictEncode(ref Dictionary<byte, object> operation)
        {
            operations = operation;
        }

        public T SetDictionary<T>(T instance)
        {
            var type = typeof(T);
            var properties = type.GetProperties();
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
                                var value = property.GetValue(instance);
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

            return instance;
        }

        private void SetInvalid(string message)
        {
            IsValid = false;
            ErrorMessage = message;
        }
    }
}
