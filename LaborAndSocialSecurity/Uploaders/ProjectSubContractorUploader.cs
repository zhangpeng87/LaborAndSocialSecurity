using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LaborAndSocialSecurity.Uploaders
{
    /// <summary>
    /// 参建单位信息上传。
    /// </summary>
    public class ProjectSubContractorUploader : Uploader<ProjectSubContractor>
    {
        /// <summary>
        /// 本平台分配的项目ID。
        /// </summary>
        private string projectCode;
        /// <summary>
        /// 施工项目ID。（品茗）
        /// </summary>
        private int project_id;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="projectCode">本平台分配的项目ID。</param>
        /// <param name="project_id">施工项目ID。（品茗）</param>
        public ProjectSubContractorUploader(string projectCode, int project_id)
        {
            this.projectCode = projectCode;
            this.project_id = project_id;
        }

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ProjectSubContractor> GetData(object parm)
        {
            IEnumerable<ProjectSubContractor> result = null;
            // 只查询总包和劳务分包数据
            DataSet set = DBHelperMySQL.Query($"SELECT cooperator_id, company_id, project_id, unit_name, unit_type, credit_code, unit_person, contacts_mobile, remark FROM f_cooperator S WHERE S.project_id = { this.project_id } AND S.unit_type IN (1,2,3,4,5,6,9,12,13) AND S.`status` = 1;");

            result = from row in set.Tables[0].AsEnumerable()
                     where !string.IsNullOrEmpty(row["remark"]?.ToString()?.Trim())
                     select new ProjectSubContractor
                     {
                         projectCode = this.projectCode,
                         corpCode = row["remark"].ToString(),
                         corpName = row["unit_name"].ToString(),
                         corpType = ProjectSubContractor.ConvertCooperatorType(row["unit_type"].ToString()),
                         manager = string.IsNullOrEmpty(row["unit_person"].ToString()) ? "未知" : row["unit_person"].ToString(),
                         managerTel = string.IsNullOrEmpty(row["contacts_mobile"].ToString()) ? "15000000000" : row["contacts_mobile"].ToString(),
                         associated = new ProjectSubContractorAssociated
                         {
                             cooperator_id = Convert.ToInt32(row["cooperator_id"]),
                             company_id = Convert.ToInt32(row["company_id"]),
                             project_id = Convert.ToInt32(row["project_id"]),
                             unit_type = Convert.ToInt32(row["unit_type"])
                         }
                     };

            return result;
        }

        protected override bool HasSuccessfulUploaded(ProjectSubContractor data, out DateTime time, out OutputResult result)
        {
            var b = base.HasSuccessfulUploaded(data, out time, out result);
            if (b) return true;

            string code = result?.code;
            if (OutputCode.重复上传.Equals(code)) return true;

            return false;
        }
    }
}
