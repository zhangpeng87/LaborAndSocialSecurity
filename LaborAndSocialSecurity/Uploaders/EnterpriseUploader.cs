using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Uploaders
{
    /// <summary>
    /// 企业信息上传。
    /// </summary>
    public class EnterpriseUploader
    {
        /// <summary>
        /// 上传完成事件。
        /// </summary>
        public event UploadCompletedEventHandler UploadCompleted;

        private readonly ProjectInfoUploader NextUploader;

        public EnterpriseUploader()
        {
            this.NextUploader = new ProjectInfoUploader();
        }

        /// <summary>
        /// 开始上传相关数据。
        /// </summary>
        public void BeginUpload(string[] args)
        {
            // 获取上传数据
            var data = this.GetData();
            // 异步上传数据
            var code = this.UploadAsync(data);
            // 查询上传结果
            this.QueryResult(code);
        }

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        private EnterpriseInfo GetData()
        {
            EnterpriseInfo result = null;

            using (StreamReader file = File.OpenText(@"JsonData\EnterpriseInfo.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                result = (EnterpriseInfo)serializer.Deserialize(file, typeof(EnterpriseInfo));
            }

            return result;
        }

        /// <summary>
        /// 异步上传数据。
        /// </summary>
        /// <param name="info"></param>
        private string UploadAsync(EnterpriseInfo info)
        {
            string requestSerialCode = null;

            try
            {
                HjApi api = new HjApi() { Endpoint = "open/api/get", Method = "Corp.Upload", Version = "2.1" };
                var result = HjApiCaller.CallOpenApi(api, info);
                requestSerialCode = result["data"]["requestSerialCode"]?.ToString();
            }
            catch (Exception e)
            {

                throw;
            }

            return requestSerialCode;
        }

        /// <summary>
        /// 可以供继承类的重写。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUploadCompleted(UploadCompletedEventArgs e)
        {
            if (this.UploadCompleted != null)
            {
                this.UploadCompleted(this, e);
            }
        }
        
        private AutoResetEvent autoEvent = new AutoResetEvent(false);
        /// <summary>
        /// 查询上传结果。
        /// </summary>
        /// <param name="requestSerialCode"></param>
        private void QueryResult(string requestSerialCode)
        {
            string result = "";
            string status = "01";
            string code = "0";
            string message = "调用成功";

            #region 轮询上传结果

            TimerCallback callback = (state) =>
            {
                #region 查询上传处理结果

                HjApi api = new HjApi() { Endpoint = "open/api/async", Method = "Async", Version = "2.1" };
                var res = HjApiCaller.CallOpenApi(api, new { requestSerialCode });

                #endregion

                string sts = res["data"]["status"]?.ToString();
                if (invokeCount == maxCount || AsyncProcessStatus.处理成功.Description().Equals(sts))
                { // 查询完成
                    result = res["data"]["result"]?.ToString();
                    status = res["data"]["status"]?.ToString();
                    code = res["code"]?.ToString();
                    message = res["message"]?.ToString();

                    invokeCount = 0;
                    autoEvent.Set();
                }
            };

            var stateTimer = new Timer(callback, requestSerialCode, 1000, 1000);

            autoEvent.WaitOne();
            stateTimer.Dispose();

            #endregion

            //#region 封装上传结果

            //UploadCompletedEventArgs e = new UploadCompletedEventArgs
            //{
            //    Result = result,
            //    Status = status,
            //    Code = code,
            //    Message = message
            //};

            //OnUploadCompleted(e);

            //#endregion
        }

        private int invokeCount = 0;
        private readonly int maxCount = 30;

        //private void CheckStatus(object stateInfo)
        //{
        //    string requestSerialCode = stateInfo?.ToString();

        //    #region 查询上传处理结果

        //    HjApi api = new HjApi() { Endpoint = "open/api/async", Method = "Async", Version = "2.1" };
        //    var result = HjApiCaller.CallOpenApi(api, new { requestSerialCode });

        //    #endregion

        //    string status = result["data"]["status"]?.ToString();
        //    if (invokeCount == maxCount || AsyncProcessStatus.处理成功.Description().Equals(status))
        //    {
        //        invokeCount = 0;
        //        autoEvent.Set();
        //    }
        //}
    }
}
