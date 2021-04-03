using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace LaborAndSocialSecurity.Uploaders
{
    public class TeamInfoUploader : Uploader<Team>
    {
        private string projectCode;
        private int cooperator_id;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="projectCode">本平台分配的项目ID。</param>
        /// <param name="cooperator_id">参建单位ID（品茗的）。</param>
        public TeamInfoUploader(string projectCode, int cooperator_id)
        {
            this.projectCode = projectCode;
            this.cooperator_id = cooperator_id;
        }

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Team> GetData(object parm)
        {
            IEnumerable<Team> result = null;

            DataSet set = DBHelperMySQL.Query($"SELECT S.group_id, S.cooperator_id, P.project_id, P.credit_code, P.unit_name, S.`name`, P.remark FROM f_group S INNER JOIN f_cooperator P ON S.cooperator_id = P.cooperator_id WHERE P.cooperator_id = { this.cooperator_id };");

            result = from row in set.Tables[0].AsEnumerable()
                     select new Team
                     {
                         projectCode = this.projectCode,
                         //corpCode = "91420100177738297E",
                         corpCode = row["remark"].ToString(),
                         //corpCode = row["credit_code"].ToString(),
                         corpName = row["unit_name"].ToString(),
                         teamName = row["name"].ToString(),
                         remark = "",
                         associated = new TeamAssociated
                         {
                             group_id = Convert.ToInt32(row["group_id"]),
                             cooperator_id = Convert.ToInt32(row["cooperator_id"]),
                             project_id = Convert.ToInt32(row["project_id"]),
                         }
                     };

            return result;
        }

        protected override bool HasSuccessfulUploaded(Team data, out DateTime time, out OutputResult result)
        {
            var b = base.HasSuccessfulUploaded(data, out time, out result);
            if (b) return true;

            string code = result?.code;
            if (OutputCode.重复上传.Equals(code)) return true;

            return false;
        }
    }
}
