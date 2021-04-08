using LaborAndSocialSecurity.Uploaders;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public class ProjectWorker : IUploadable
    {
        /// <summary>
        /// 本平台分配的项目ID。
        /// </summary>
        public string projectCode;
        /// <summary>
        /// 所在企业统一社会信用代码。
        /// </summary>
        public string corpCode;
        /// <summary>
        /// 所在企业名称。
        /// </summary>
        public string corpName;
        /// <summary>
        /// 班组编号。
        /// </summary>
        public string teamSysNo;
        /// <summary>
        /// 班组名称。
        /// </summary>
        public string teamName;
        /// <summary>
        /// 人员列表数据,JSON 数组，数量不能超过 5。
        /// </summary>
        public IEnumerable<Worker> workerList;

        [JsonIgnore]
        public int DataId => workerList.FirstOrDefault()?.associated.worker_id ?? default(int);

        public OutputResult Upload()
        {
            HjApi api = new HjApi
            {
                Endpoint = "open/api/get",
                Method = "ProjectWorker.Upload",
                Version = "2.1",
                Format = "json"
            };

            return JsonConvert.DeserializeObject<OutputResult>(api.ReadyToCall(this).ToString());
        }
    }
}
