using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameOfRevenge.Helpers
{
    public static class GlobalHelper
    {
        public static string DicToString(Dictionary<byte, object> d)
        {
            string message = string.Empty;
            
            if (d != null)
            {
                foreach (var item in d)
                {
                    message += ("  KEY: " + item.Key.ToString()) + "  " + (" VALUE: " + JsonConvert.SerializeObject(item.Value));
                }
            }

            return message;
        }
    }
}
