using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public static class OutputCode
    {
        public static readonly string 成功 = "0";
        public static readonly string 非法凭证 = "1001";
        public static readonly string 过期凭证 = "1002";
        public static readonly string 验签失败 = "1003";
        public static readonly string 参数残缺 = "2001";
        public static readonly string 系统内部错误 = "2002";
        public static readonly string 参数校验失败 = "2003";
        public static readonly string 访问超过限制 = "2005";
        public static readonly string 请求序列码不存在 = "2006";
        public static readonly string 重复上传 = "2007";

        // 补充
        public static readonly string 人员已存在 = "1086";
    }

    public class OutputResult
    {
        public string code;
        public string message;
        public JObject data;
    }

    public static class AsyncStatus
    {
        public static readonly string 待处理 = "01";
        public static readonly string 处理失败 = "02";
        public static readonly string 处理成功 = "03";
    }

    /// <summary>
    /// 异步处理状态枚举。
    /// </summary>
    public enum AsyncProcessStatus
    {
        [Description("01")]
        待处理,
        [Description("02")]
        处理失败,
        [Description("03")]
        处理成功
    }
}
