using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        #region 静态成员
        private static readonly string QueryString;
        // 缓存查询结果
        private static readonly DataTable cache = new DataTable();

        static TeamInfoUploader()
        {
            QueryString = $"SELECT S.group_id, S.cooperator_id, P.project_id, P.credit_code, P.unit_name, S.`name`, P.remark FROM f_group S INNER JOIN f_cooperator P ON S.cooperator_id = P.cooperator_id WHERE P.project_id = { HjApiCaller.Project_id };";
            var resultSet = DBHelperMySQL.TryQuery(QueryString);
            cache = resultSet.Tables[0];
        }
        #endregion

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Team> GetData(object parm)
        {
            IEnumerable<Team> result = null;
            var filtered = cache.Select($"cooperator_id = { this.cooperator_id }");

            result = from row in filtered
                     where !string.IsNullOrEmpty(row["remark"]?.ToString()?.Trim()) ||
                           !string.IsNullOrEmpty(row["credit_code"]?.ToString()?.Trim())
                     select new Team
                     {
                         projectCode = HjApiCaller.ProjectCode,
                         corpCode = "".Equals(row["remark"].ToString().Trim()) ? row["credit_code"].ToString() : row["remark"].ToString().Trim(),
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
