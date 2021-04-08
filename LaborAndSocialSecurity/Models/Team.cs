using LaborAndSocialSecurity.Uploaders;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public class TeamAssociated
    {
        /// <summary>
        /// 班组id。
        /// </summary>
        public int group_id;
        /// <summary>
        /// 参建单位id。
        /// </summary>
        public int cooperator_id;
        /// <summary>
        /// 项目id。
        /// </summary>
        public int project_id;
    }

    public class Team : IUploadable
    {
        /// <summary>
        /// 本平台分配的项目ID
        /// </summary>
        public string projectCode;
        /// <summary>
        /// 班组所在企业统一社会信用代码
        /// </summary>
        public string corpCode;
        /// <summary>
        /// 班组所在企业名称
        /// </summary>
        public string corpName;
        /// <summary>
        /// 班组名称，同一个项目下面不能重复
        /// </summary>
        public string teamName;
        /// <summary>
        /// 班组类型,参考班组类型字典表
        /// </summary>
        public string type = "1000";
        /// <summary>
        /// 备注
        /// </summary>
        public string remark;

        /// <summary>
        /// 关联的对象，存放额外信息。
        /// </summary>
        [JsonIgnore]
        public TeamAssociated associated;

        [JsonIgnore]
        public int DataId => this.associated.group_id;

        public OutputResult Upload()
        {
            HjApi api = new HjApi
            {
                Endpoint = "open/api/get",
                Method = "Team.Upload",
                Version = "2.1",
                Format = "json"
            };
            
            return JsonConvert.DeserializeObject<OutputResult>(api.ReadyToCall(this).ToString());
        }

        public static OutputResult Query(int pageIndex, int pageSize, string projectCode)
        {
            HjApi api = new HjApi
            {
                Endpoint = "open/api/get",
                Method = "Team.Query",
                Version = "2.1",
                Format = "json"
            };

            var input = new
            {
                pageIndex,
                pageSize,
                projectCode
            };

            return JsonConvert.DeserializeObject<OutputResult>(api.ReadyToCall(input).ToString());
        }

        public static OutputResult SysNo(string projectCode, string teamName)
        {
            HjApi api = new HjApi
            {
                Endpoint = "open/api/get",
                Method = "Team.SysNo",
                Version = "1.0",
                Format = "json"
            };

            var input = new
            {
                projectCode,
                teamName
            };

            return JsonConvert.DeserializeObject<OutputResult>(api.ReadyToCall(input).ToString());
        }

        /// <summary>
        /// 记录生成的班组编号。
        /// </summary>
        /// <param name="teamSysNo"></param>
        public void RecordTeamSysNo2File(string teamSysNo)
        {
            LogUtils4Team.Logger.Info($"{ this.associated.group_id },{ this.associated.cooperator_id },{ this.associated.project_id },{ this.teamName },{ this.corpName },{ this.corpCode },{ this.projectCode },{ teamSysNo }");
        }
    }
}
