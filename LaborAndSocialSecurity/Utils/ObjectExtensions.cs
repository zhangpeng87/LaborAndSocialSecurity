using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Utils
{
    public static class ObjectExtensions
    {
        public static string Serialize2JSON(this object source)
        {
            if (source == null) return string.Empty;

            StringBuilder builder = new StringBuilder();
            using (StringWriter sw = new StringWriter(builder))
            using (JsonTextWriter writer = new JsonTextWriter(sw))
            {
                writer.QuoteChar = '\"';

                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(writer, source);
            }

            return builder.ToString();
        }
    }
}
