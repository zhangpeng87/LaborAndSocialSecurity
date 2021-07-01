using LaborAndSocialSecurity.Uploaders;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace LaborAndSocialSecurity.Utils
{    
    /// <summary>
    /// 会基接口类。
    /// </summary>
    public class HjApi
    {
        /// <summary>
        /// 开头不要带斜杠。
        /// </summary>
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public string Version { get; set; }                     // 接口版本
        public string Format { get; set; } = "json";            // 返回类型

        public JObject Invoke(object input)
        {
            var command = new Dispatcher.ApiInvokeCommand(this, input);
            var result = Policy
                            .HandleResult<JObject>(r =>
                            {
                                return r.SelectToken("message")?.ToString().IndexOf("频繁") > -1;
                            })
                            .Retry(3)
                            .Execute(() =>
                            {
                                var autoEvent = Dispatcher.Instance.EnqueueCommand(command);
                                autoEvent.WaitOne();
                                autoEvent.Dispose();

                                return command.Output;
                            });

            return result;
        }
    }

    /// <summary>
    /// 会基WebAPI调用方法类。
    /// </summary>
    public class HjApiCaller
    {
        public static string Appsecret = "1b8b6f5fcb4e4e059fc51713d1a2947b";        // secret：  签名密钥
        public static string AppId = "1921af09a95b45f280c3ff4d1a007e0e";            // appid：   身份标识符（一个appId对应一个项目）
        public static string Host = "http://devel.sunhj.cn:9998";                   // 地址：      资源入口

        public static string ProjectName = "测试标段";
        public static string ProjectCode = "ff8080817682f02601768933e2540009";      // 分配的项目ID（会基）
        public static int Project_id = 21;                                          // 施工项目ID（品茗）

        public static string Host1 => Host;

        /// <summary>
        /// 初始化待上传标段的参数信息。
        /// </summary>
        /// <returns>初始化是否成功。</returns>
        public static bool TryInit()
        {
            // 1、读取今日上传日志
            List<string> uploadedProjects = new List<string>();
            using (TextFieldParser parser = new TextFieldParser($@"D:\LogFile\LaborAndSocialSecurity\UploadLog\Upload{ DateTime.Now.ToString("yyyyMMdd") }.log"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                    uploadedProjects.Add(parser.ReadFields()[0]);
            }
            // 2、读取标段配置文件
            using (TextFieldParser parser = new TextFieldParser($"{ Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) }\\Project.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    if (!uploadedProjects.Contains(fields[0]))
                    {
                        // 4、配置当前标段参数
                        AppId = fields[6];
                        Appsecret = fields[7];
                        Host = fields[8];

                        ProjectName = fields[0];
                        Project_id = Convert.ToInt32(fields[11]);
                        ProjectCode = fields[12];

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取时间戳。
        /// </summary>
        /// <returns></returns>
        private static string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// 生成6位的随机数。
        /// </summary>
        /// <returns></returns>
        private static string Get6RandomNumber()
        {
            return DateTime.Now.ToString("mmssff");
        }

        /// <summary>
        /// 调用接口。
        /// </summary>
        /// <param name="api">接口信息。</param>
        /// <param name="input">输入参数。</param>
        /// <returns></returns>
        public static JObject CallOpenApi(HjApi api, object input)
        {
            string timestamp = GetTimeStamp();
            string sData = input.Serialize2JSON();
            // UTF-8编码
            //sData = HttpUtility.UrlEncode(sData, Encoding.UTF8);
            string nonce = Get6RandomNumber();
            string sign = $"appId={ AppId }&data={ sData }&format={ api.Format }&method={ api.Method }&nonce={ nonce }&timestamp={ timestamp }&version={ api.Version }&appsecret={ Appsecret }".ToLower();
            sign = EncryptUtils.SHA256Encrypt(sign);

            Hashtable pars = new Hashtable
            {
                { "appId", AppId },
                { "data", sData },
                { "format", api.Format },
                { "method", api.Method },
                { "nonce", nonce },
                { "sign", sign },
                { "timestamp", timestamp },
                { "version", api.Version },
            };

            WebApiCaller<JObject> caller = new WebApiCaller<JObject>
            {
                Method = RequestMethod.POST,
                ContentType = RequestContentType.URLENCODEED,
                Callback = JObject.Parse
            };

            var result = caller.Call($"{ Host1 }/{ api.Endpoint }", null, pars);
            LogUtils4Debug.Logger.Debug(new { Api = api, Params = input, Result = result }.Serialize2JSON());

            return result;
        }
    }
}