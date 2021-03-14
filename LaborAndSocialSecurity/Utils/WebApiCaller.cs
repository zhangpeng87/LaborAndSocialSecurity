using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace LaborAndSocialSecurity.Utils
{
    public enum RequestMethod
    {
        GET,
        POST,
        DELETE,
        PUT
    }

    public enum RequestContentType
    {
        [Description("application/x-www-form-urlencoded")]
        URLENCODEED,
        [Description("application/json")]
        JSON,
        [Description("text/plain")]
        TEXT
    }

    public class WebApiCaller<T>
    {
        public RequestMethod Method { get; set; }
        public RequestContentType ContentType { get; set; }
        public Func<string, T> Callback { get; set; }
        public Hashtable Headers { get; set; }

        public WebApiCaller()
        {
            this.Method = RequestMethod.POST;
            this.ContentType = RequestContentType.JSON;
            this.Headers = new Hashtable();
        }

        private static string ParsToString(Hashtable Pars)
        {
            if (Pars == null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(Pars[k]?.ToString()));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 设置凭证与超时时间
        /// </summary>
        /// <param name="request"></param>
        private static void SetWebRequest(HttpWebRequest request)
        {
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ReadWriteTimeout = 600000;
            request.Timeout = 600000;

        }

        private static void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
        }

        public T Call(string url, Hashtable queryParams, object bodyParams)
        {
            string urlParamsString = ParsToString(queryParams);
            string requestUriString;
            requestUriString = url;
            if (!string.IsNullOrEmpty(urlParamsString))
                requestUriString += "?" + urlParamsString;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUriString);
            request.Method = this.Method.ToString();
            request.ContentType = this.ContentType.Description();
            foreach (var key in this.Headers?.Keys)
            {
                if (key is string)
                    request.Headers.Add(key.ToString(), this.Headers[key].ToString());
                else if (key is HttpRequestHeader)
                    request.Headers.Add((HttpRequestHeader)Enum.Parse(typeof(HttpRequestHeader), key.ToString(), true), this.Headers[key].ToString());
            }
            SetWebRequest(request);

            byte[] data = null;
            switch (this.ContentType)
            {
                case RequestContentType.URLENCODEED:
                    data = Encoding.UTF8.GetBytes(ParsToString(bodyParams as Hashtable));
                    break;

                case RequestContentType.JSON:
                    data = Encoding.UTF8.GetBytes(bodyParams?.Serialize2JSON());
                    break;

                case RequestContentType.TEXT:
                    data = Encoding.UTF8.GetBytes(bodyParams?.ToString());
                    break;

                default:
                    data = Encoding.UTF8.GetBytes(bodyParams?.ToString());
                    break;
            }

            WriteRequestData(request, data);
            WebResponse response = null;
            StreamReader sr = null;
            try
            {
                response = request.GetResponse();
                sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string retXml = sr.ReadToEnd();

                return this.Callback(retXml);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                response?.Close();
                sr?.Close();
            }
        }
    }

    public class Jsonp
    {
        public string RawData { get; private set; }
        public string Callback { get; private set; }
        public JObject JsonData { get; private set; }

        private Jsonp()
        {

        }

        public static Jsonp Parse(string str)
        {
            Jsonp jsonp = new Jsonp();

            string trimed = str.Trim();
            string pattern = @"^(?<callback>[\w]+)\((?<jsondata>[\s|\S]*)\)[;]?$";
            Match matc = Regex.Match(trimed, pattern);

            jsonp.RawData = str;
            // 解析回调函数名
            jsonp.Callback = matc.Groups["callback"].ToString();
            // 解析返回的数据
            string json = matc.Groups["jsondata"].ToString();
            jsonp.JsonData = JObject.Parse(json);

            return jsonp;
        }
    }
}
