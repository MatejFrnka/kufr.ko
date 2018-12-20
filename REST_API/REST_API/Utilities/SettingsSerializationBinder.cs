using Newtonsoft.Json.Serialization;
using REST_API.Models.Api.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Utilities
{
    public class SettingsSerializationBinder : ISerializationBinder
    {
        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            switch (typeName)
            {
                //MessageController
                case "EditMessage":
                    return typeof(EditMessage);
                case "GetMessage":
                    return typeof(GetMessage);
                case "SendMessage":
                    return typeof(SendMessage);
                case "SetMessageState":
                    return typeof(SetMessageState);
            }
            return null;
        }
    }
}