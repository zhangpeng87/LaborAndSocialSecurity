using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    /// <summary>
    /// 异步查询。
    /// </summary>
    class OutputAsyncQuery : OutputSuper
    {
        private string requestSerialCode;

        public OutputAsyncQuery(string requestSerialCode)
        {
            this.requestSerialCode = requestSerialCode;
        }

        public override OutputResult NextCall()
        {
            if (string.IsNullOrEmpty(this.requestSerialCode)) return null;

            var json = Policy
                        .HandleResult<JObject>(r =>
                        {
                            return string.Compare(AsyncProcessStatus.待处理.Description(), r.SelectToken("data")?.SelectToken("status")?.ToString()) == 0;
                        })
                        .WaitAndRetry(30, retryCount =>
                        {
                            return TimeSpan.FromSeconds(1);
                        })
                        .Execute(() =>
                        {
                            HjApi api = new HjApi() { Endpoint = "open/api/async", Method = "Async", Version = "2.1" };
                            return api.Invoke(new { requestSerialCode });
                        });

            return JsonConvert.DeserializeObject<OutputResult>(json?.ToString());
        }
    }
}
