using System;
using System.Collections.Generic;
using Photon.SocketServer.Rpc;

namespace GameOfRevenge.Helpers
{
    public abstract class DictDecode
    {
        private readonly Dictionary<byte, object> operations;
        public bool IsValid { get; private set; } = true;
        public string ErrorMessage { get; private set; }

        public DictDecode(Dictionary<byte, object> operation)
        {
            operations = operation;
        }

        public T As<T>(T instance)
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
                    if (operations.ContainsKey(paramCode))
                    {
                        if (property.CanWrite)
                        {
                            try
                            {
                                var dictPropType = operations[paramCode].GetType().Name;
                                var propType = property.PropertyType.Name;
                                if (dictPropType == propType)
                                {
                                    property.SetValue(instance, operations[paramCode]);
                                }
                                else
                                {
                                    SetInvalid("property is invalid.");
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                SetInvalid(ex.Message);
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
