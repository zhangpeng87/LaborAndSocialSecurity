using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Utils
{
    public static class PmApiCaller
    {
        const string ServiceUrl = "http://122.189.155.124:9014/api/openapi";        // 接口地址
        const string Cono = "1145422";                                              // 企业ID
        const string Private_key = "8673fdd3ea8654b5690c0a501effa328";              // 开发者秘钥
        const byte DType = 2;                                                       // 开发者类型: 1企业开发者 2平台开发者，默认为空代表企业开发者
        const string DKey = "2c91808d70b2e6610170b2e6619c0000";                     // 开发者key

        /// <summary>
        /// 调用品茗OpenAPI接口。
        /// </summary>
        /// <param name="serverAlias">接口别名。</param>
        /// <param name="sItype">接口编号。</param>
        /// <param name="objectData">业务数据。</param>
        /// <returns></returns>
        public static JObject CallOpenApi(string serverAlias, string sItype, object objectData)
        {
            string sTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            // 转换为JSON
            string sData = JsonConvert.SerializeObject(objectData);
            // UTF-8编码
            sData = HttpUtilityEx.UrlEncode4Java(sData, Encoding.UTF8);
            // 使用AES加密（AES加密是对照品茗给的Demo修改的）
            string sEncryptData = EncryptUtils.EncryptPm(sData, Private_key);
            // MD5签名
            string sSign = EncryptUtils.Md5Encrypt(sItype + sTime + Cono + sEncryptData + Private_key);

            Hashtable pars = new Hashtable
            {
                { "itype", sItype },
                { "cono", Cono },
                { "time", sTime },
                { "data", sEncryptData },
                { "sign", sSign },
                { "etype", "1" },
                { "dtype", DType.ToString() },
                { "dkey", DKey }
            };

            return WebServiceCaller.QueryPostWebService(ServiceUrl, serverAlias, pars, HttpUtilityEx.UrlEncode4Java);
        }
    }
}
