using LaborAndSocialSecurity.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace LaborAndSocialSecurity.Uploaders
{
    /// <summary>
    /// 项目信息上传。
    /// </summary>
    public class ProjectInfoUploader
    {
        /// <summary>
        /// 上传完成事件。
        /// </summary>
        public event UploadCompletedEventHandler UploadCompleted;

        private readonly CooperatorInfoUploader NextUploader;

        public ProjectInfoUploader()
        {
            this.NextUploader = new CooperatorInfoUploader();
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
        private ProjectInfo GetData()
        {
            ProjectInfo result = null;

            using (StreamReader file = File.OpenText(@"JsonData\ProjectInfo.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                result = (ProjectInfo)serializer.Deserialize(file, typeof(ProjectInfo));
            }

            return result;
        }

        /// <summary>
        /// 异步上传数据。
        /// </summary>
        /// <param name="info"></param>
        private string UploadAsync(ProjectInfo info)
        {
            string requestSerialCode = null;

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
            #region 轮询上传结果

            var stateTimer = new Timer(CheckStatus, autoEvent, 1000, 1000);

            autoEvent.WaitOne();
            stateTimer.Dispose();

            #endregion

            //#region 最终上传结果

            //string result = "";
            //string status = "01";
            //string code = "0";
            //string message = "调用成功";

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

        private void CheckStatus(object stateInfo)
        {
            string requestSerialCode = stateInfo?.ToString();

            if (invokeCount == maxCount)
            {
                invokeCount = 0;
                autoEvent.Set();
            }
        }
    }
}