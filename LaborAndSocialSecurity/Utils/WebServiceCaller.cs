using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace LaborAndSocialSecurity.Utils
{
    public delegate string UrlEncodeHandler(string str);

    /// <summary>
    /// 旧的类文件。
    /// </summary>
    public class WebServiceCaller
    {
        /// <summary>
        /// 通用WebService调用(Soap),参数Pars为String类型的参数名、参数值
        /// </summary>
        public static XmlDocument QuerySoapWebService(string URL, string MethodName, Hashtable Pars)
        {
            if (_xmlNamespaces.ContainsKey(URL))
            {
                return QuerySoapWebService(URL, MethodName, Pars, _xmlNamespaces[URL].ToString());
            }
            else
            {
                return QuerySoapWebService(URL, MethodName, Pars, GetNamespace(URL));
            }

            //return QuerySoapWebService(URL, MethodName, Pars, GetNamespace(URL));
        }

        private static XmlDocument QuerySoapWebService(string URL, string MethodName, Hashtable Pars, string XmlNs)
        {
            _xmlNamespaces[URL] = XmlNs;//加入缓存，提高效率
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "\"" + XmlNs + (XmlNs.EndsWith("/") ? "" : "/") + MethodName + "\"");
            //SetWebRequest(request);
            request.ReadWriteTimeout = 600000;
            request.Timeout = 600000;
            request.KeepAlive = false;
            request.Proxy = null;
            byte[] data = EncodeParsToSoap(Pars, XmlNs, MethodName);
            WriteRequestData(request, data);
            XmlDocument doc = new XmlDocument();
            WebResponse presp = request.GetResponse();
            doc = ReadXmlResponse(presp);
            if (presp != null)
            {
                presp.Close();
            }
            if (request != null)
            {
                request.Abort();
            }
            GC.Collect();
            XmlDocument doc2 = new XmlDocument();
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            string RetXml = doc.SelectSingleNode("//soap:Body/*/*", mgr).InnerXml;
            doc2.LoadXml("<root>" + RetXml + "</root>");
            AddDelaration(doc2);
            return doc2;
        }
        private static string GetNamespace(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + "?WSDL");
            SetWebRequest(request);
            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            response.Close();
            sr.Close();
            return doc.SelectSingleNode("//@targetNamespace").Value;
        }

        private static byte[] EncodeParsToSoap(Hashtable Pars, string XmlNs, string MethodName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"></soap:Envelope>");
            AddDelaration(doc);
            //XmlElement soapBody = doc.createElement_x_x("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlElement soapBody = doc.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            //XmlElement soapMethod = doc.createElement_x_x(MethodName);
            XmlElement soapMethod = doc.CreateElement(MethodName);
            soapMethod.SetAttribute("xmlns", XmlNs);
            foreach (string k in Pars.Keys)
            {
                //XmlElement soapPar = doc.createElement_x_x(k);
                XmlElement soapPar = doc.CreateElement(k);
                soapPar.InnerXml = ObjectToSoapXml(Pars[k]);
                soapMethod.AppendChild(soapPar);
            }
            soapBody.AppendChild(soapMethod);
            doc.DocumentElement.AppendChild(soapBody);
            return Encoding.UTF8.GetBytes(doc.OuterXml);
        }
        private static string ObjectToSoapXml(object o)
        {
            XmlSerializer mySerializer = new XmlSerializer(o.GetType());
            MemoryStream ms = new MemoryStream();
            mySerializer.Serialize(ms, o);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Encoding.UTF8.GetString(ms.ToArray()));
            if (doc.DocumentElement != null)
            {
                return doc.DocumentElement.InnerXml;
            }
            else
            {
                return o.ToString();
            }
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

        private static byte[] EncodePars(Hashtable Pars, Func<Hashtable, string> ParsConverter = null)
        {
            if (ParsConverter == null)
            {
                ParsConverter = ParsToString;
            }
            return Encoding.UTF8.GetBytes(ParsConverter(Pars));
        }

        private static string ParsToJSONString(Hashtable pars)
        {
            return pars.Serialize2JSON();
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

        private static XmlDocument ReadXmlResponse(WebResponse response)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retXml = sr.ReadToEnd();
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);
            return doc;
        }

        private static void AddDelaration(XmlDocument doc)
        {
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(decl, doc.DocumentElement);
        }

        private static Hashtable _xmlNamespaces = new Hashtable();//缓存xmlNamespace，避免重复调用GetNamespace

        #region 新增方法 2020-3-19
        /// <summary>
        /// 需要WebService支持Post调用，返回Json格式数据。
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="MethodName"></param>
        /// <param name="Pars"></param>
        /// <param name="urlEncodeHandler">调用.NET接口：HttpUtility.UrlEncode；调用Java接口：HttpUtilityEx.UrlEncode4Java。</param>
        /// <returns></returns>
        public static JObject QueryPostWebService(string URL, string MethodName, Hashtable Pars, UrlEncodeHandler encodeHandler)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL + "/" + MethodName);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.PreAuthenticate
            //request.Headers[HttpRequestHeader.Authorization] = "";
            SetWebRequest(request);
            request.ReadWriteTimeout = 600000;
            request.Timeout = 600000;
            //request.ContinueTimeout
            byte[] data = Encoding.UTF8.GetBytes(ParsToString(Pars, encodeHandler));
            WriteRequestData(request, data);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var result = ReadResponse(response, JObject.Parse);
            if (response != null) response.Close();

            return result;
        }

        /// <summary>
        /// 需要WebService支持Post调用，返回Json格式数据。
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="MethodName"></param>
        /// <param name="Pars"></param>
        /// <param name="contentType">application/x-www-form-urlencoded; application/json;</param>
        /// <param name="authorization">Bearer+空格+token</param>
        /// <returns></returns>
        public static JObject QueryPostWebService(string URL, string MethodName, Hashtable Pars, string contentType, string authorization)
        {
            return QueryPostWebService(URL, MethodName, Pars, contentType, authorization, s => JObject.Parse(s));
        }

        /// <summary>
        /// 需要WebService支持Post调用，返回Json格式数据。
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="MethodName"></param>
        /// <param name="Pars"></param>
        /// <param name="contentType">application/x-www-form-urlencoded; application/json;</param>
        /// <param name="authorization">Bearer+空格+token</param>
        /// <returns></returns>
        public static T QueryPostWebService<T>(string URL, string MethodName, Hashtable Pars, string contentType, string authorization, Func<string, T> func)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL + "/" + MethodName);
            request.Method = "POST";
            request.ContentType = contentType;
            request.Headers[HttpRequestHeader.Authorization] = authorization;
            SetWebRequest(request);
            request.ReadWriteTimeout = 600000;
            request.Timeout = 600000;
            //request.ContinueTimeout
            byte[] data = EncodePars(Pars);
            if ("application/json".Equals(contentType)) data = EncodePars(Pars, p => p.Serialize2JSON());
            WriteRequestData(request, data);
            //return ReadJsonResponse(request.GetResponse());
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retXml = sr.ReadToEnd();
            if (response != null) response.Close();
            sr.Close();

            return func(retXml);
        }

        private static string ParsToString(Hashtable Pars, UrlEncodeHandler encodeHandler)
        {
            if (Pars == null) return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(encodeHandler(k) + "=" + encodeHandler(Pars[k]?.ToString()));
            }
            return sb.ToString();
        }

        private static T ReadResponse<T>(WebResponse response, Func<string, T> responseConverter)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retXml = sr.ReadToEnd();
            //response.Close();
            sr.Close();
            return responseConverter(retXml);
        }

        /// <summary>
        /// 需要WebService支持Get调用，返回Jsonp格式数据。
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Pars"></param>
        /// <param name="encodeHandler"></param>
        /// <returns></returns>
        public static T QueryGetWebService<T>(string URL, Hashtable Pars, UrlEncodeHandler encodeHandler, Func<string, T> responseConverter)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL + "?" + ParsToString(Pars, encodeHandler));
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            SetWebRequest(request);
            request.ReadWriteTimeout = 600000;
            request.Timeout = 600000;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var result = ReadResponse(response, responseConverter);
            if (response != null) response.Close();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="URL"></param>
        /// <param name="MethodName"></param>
        /// <param name="urlParams">url后接参数</param>
        /// <param name="bodyParams">body中的参数</param>
        /// <param name="contentType"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static T QueryPostWebService<T>(string URL, string MethodName, Hashtable urlParams, object bodyParams, string contentType, Func<string, T> callback)
        {
            string urlParamsString = ParsToString(urlParams);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL + "/" + MethodName + "?" + urlParamsString);
            request.Method = "POST";
            request.ContentType = contentType;
            SetWebRequest(request);

            byte[] data = null;
            switch (contentType)
            {
                case "application/x-www-form-urlencoded":
                    data = Encoding.UTF8.GetBytes(ParsToString(bodyParams as Hashtable));
                    break;
                case "application/json":
                    data = Encoding.UTF8.GetBytes(bodyParams.Serialize2JSON());
                    break;
                default:
                    break;
            }

            WriteRequestData(request, data);
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException)
            {
                return default(T);
            }
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retXml = sr.ReadToEnd();
            if (response != null) response.Close();
            sr.Close();

            return callback(retXml);
        }

        public class Jsonp
        {
            public string RawData { get; private set; }
            public string Callback { get; private set; }
            public JObject JsonData { get; private set; }

            private Jsonp()
            {

            }

            //public Jsonp(string rawData)
            //{
            //    this.RawData = rawData;
            //    string trimed = rawData.Trim();

            //    string pattern = @"^(?<callback>[\w]+)\((?<jsondata>[\s|\S]*)\)[;]?$";
            //    Match matc = Regex.Match(trimed, pattern);

            //    // 解析回调函数名
            //    this.Callback = matc.Groups["callback"].ToString();
            //    // 解析返回的数据
            //    string json = matc.Groups["jsondata"].ToString();
            //    this.JsonData = JObject.Parse(json);
            //}

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
        #endregion
    }
}
