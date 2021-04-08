using LaborAndSocialSecurity.Uploaders;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public class ProjectSubContractorAssociated
    {
        /// <summary>
        /// ID。
        /// </summary>
        public int cooperator_id;
        /// <summary>
        /// 所属企业ID。
        /// </summary>
        public int company_id;
        /// <summary>
        /// 施工项目ID。
        /// </summary>
        public int project_id;
        /// <summary>
        /// 企业类型。
        /// </summary>
        public int unit_type;
    }

    /// <summary>
    /// 参建单位上传信息。
    /// </summary>
    public class ProjectSubContractor : IUploadable
    {
        /// <summary>
        /// 本平台分配的项目ID。
        /// </summary>
        public string projectCode;
        /// <summary>
        /// 统一社会信用代码。
        /// </summary>
        public string corpCode;
        /// <summary>
        /// 企业名称。
        /// </summary>
        public string corpName;
        /// <summary>
        /// 参建类型。参考参建单位类型字典表。
        /// </summary>
        public string corpType;
        /// <summary>
        /// 退场时间，格式 yyyy-MM-dd HH:mm:ss。
        /// </summary>
        public string entryTime;
        /// <summary>
        /// 退场时间，格式 yyyy-MM-dd HH:mm:ss。
        /// </summary>
        public string exitTime;
        /// <summary>
        /// 企业负责人（新加字段）。
        /// </summary>
        public string manager;
        /// <summary>
        /// 企业负责人电话（新加字段）。
        /// </summary>
        public string managerTel;

        /// <summary>
        /// 关联的对象，存放额外信息。
        /// </summary>
        [JsonIgnore]
        public ProjectSubContractorAssociated associated;

        [JsonIgnore]
        public int DataId => this.associated.cooperator_id;

        /// <summary>
        /// 上传方法。
        /// </summary>
        /// <returns></returns>
        public OutputResult Upload()
        {
            HjApi api = new HjApi
            {
                Endpoint = "open/api/get",
                Method = "ProjectSubContractor.Upload",
                Version = "2.1",
                Format = "json"
            };

            return JsonConvert.DeserializeObject<OutputResult>(api.ReadyToCall(this).ToString());
        }

        /// <summary>
        /// 转换参建单位类型。
        /// </summary>
        /// <param name="cooperator_code_pm"></param>
        /// <returns></returns>
        public static string ConvertCooperatorType(string cooperator_code_pm)
        {
            var first = (from row in DataDictionary.CooperatorTypes.AsEnumerable()
                         where row["cooperator_code_pm"].ToString().Equals(cooperator_code_pm?.Trim())
                         select new
                         {
                             cooperator_code_hj = row["cooperator_code_hj"].ToString()
                         }).FirstOrDefault();

            return first?.cooperator_code_hj ?? "03";           // 默认分包
        }
    }
}
