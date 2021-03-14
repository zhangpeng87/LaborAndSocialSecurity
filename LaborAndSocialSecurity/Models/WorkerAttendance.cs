using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    /// <summary>
    /// 考勤基本信息。
    /// </summary>
    public class WorkerAttendance
    {
        /// <summary>
        /// 平台为项目分配的接入编号。
        /// </summary>
        public string projectCode;
        /// <summary>
        /// 平台生成的班组编号。
        /// </summary>
        public string teamSysNo;
        /// <summary>
        /// 考勤列表。JSON数组，数组长度不超过20。
        /// </summary>
        public IEnumerable<Attendance> dataList;

        /// <summary>
        /// 上传方法。
        /// </summary>
        /// <returns></returns>
        public OutputResult Upload()
        {
            HjApi api = new HjApi
            {
                Endpoint = "open/api/get",
                Method = "WorkerAttendance.Upload",
                Version = "2.1",
                Format = "json"
            };

            return JsonConvert.DeserializeObject<OutputResult>(api.Call(this).ToString());
        }
    }
}
