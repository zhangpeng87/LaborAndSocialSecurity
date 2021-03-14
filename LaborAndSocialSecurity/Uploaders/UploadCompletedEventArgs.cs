using LaborAndSocialSecurity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Uploaders
{
    /// <summary>
    /// 包含上传的时间、上传的数据、上传的结果。
    /// </summary>
    public class UploadCompletedEventArgs : EventArgs
    {
        public DateTime UploadTime { get; set; }
        /// <summary>
        /// 上传的数据。
        /// </summary>
        public object UploadedData { get; set; }
        /// <summary>
        /// 上传的结果。
        /// </summary>
        public OutputResult UploadedResult { get; set; }

        public UploadCompletedEventArgs()
        {

        }

        public UploadCompletedEventArgs(DateTime time, object uploadedData, OutputResult outputResult)
        {
            this.UploadTime = time;
            this.UploadedData = uploadedData;
            this.UploadedResult = outputResult;
        }
    }
}
