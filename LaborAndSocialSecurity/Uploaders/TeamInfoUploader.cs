using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Uploaders
{
    public class TeamInfoUploader
    {
        private string projectCode;
        private int cooperator_id;

        /// <summary>
        /// 上传完成事件。
        /// </summary>
        public event UploadCompletedEventHandler UploadCompleted;

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
        /// 开始上传相关数据。
        /// </summary>
        public void BeginUpload()
        {
            // 获取上传数据
            var list = this.GetData();
            // 进行上传数据
            foreach (var item in list)
                this.UploadData(item);
        }

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Team> GetData()
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

        /// <summary>
        /// 异步上传数据。
        /// </summary>
        /// <param name="data"></param>
        private void UploadData(Team data)
        {
            try
            {
                DateTime start = DateTime.Now;
                OutputResult result = data.Upload();
                OutputContext context = new OutputContext(result, data);
                OutputResult final = context.NextCall();
                // 通知上传完成事件订阅者
                OnUploadCompleted(new UploadCompletedEventArgs(start, data, final));
            }
            catch (Exception e)
            {
                LogUtils4Error.Logger.Debug($"Exception: TeamInfoUploader.UploadData: { e.Message }. data: { data.Serialize2JSON() }");
                //throw;
            }
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
    }
}
