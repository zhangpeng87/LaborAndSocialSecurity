using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private int invokeCount = 0;
        private readonly int maxCount = 3;
        private AutoResetEvent autoEvent = new AutoResetEvent(false);

        public OutputAsyncQuery(string requestSerialCode)
        {
            this.requestSerialCode = requestSerialCode;
        }

        public override OutputResult NextCall()
        {
            if (string.IsNullOrEmpty(this.requestSerialCode)) return null;

            OutputResult result = null;
            
            TimerCallback callback = (state) =>
            {
                HjApi api = new HjApi() { Endpoint = "open/api/async", Method = "Async", Version = "2.1" };
                JObject res = api.Call(new { requestSerialCode });
                invokeCount++;

                if (res["message"]?.ToString().IndexOf("频繁") > -1) return;

                string sts = res["data"]?["status"]?.ToString();
                if (invokeCount == maxCount || AsyncProcessStatus.处理成功.Description().Equals(sts))
                { // 查询完成
                    result = JsonConvert.DeserializeObject<OutputResult>(res?.ToString());

                    invokeCount = 0;
                    autoEvent.Set();
                }
            };

            Timer stateTimer = new Timer(callback, this.requestSerialCode, 1000, 1300);

            autoEvent.WaitOne();
            stateTimer.Dispose();

            return result;
        }
    }
}
