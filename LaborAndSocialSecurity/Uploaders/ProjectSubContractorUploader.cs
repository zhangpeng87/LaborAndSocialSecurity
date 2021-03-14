using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Uploaders
{
    /// <summary>
    /// 参建单位信息上传。
    /// </summary>
    public class ProjectSubContractorUploader
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
        /// 上传完成事件。
        /// </summary>
        public event UploadCompletedEventHandler UploadCompleted;

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
        private IEnumerable<ProjectSubContractor> GetData()
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

        /// <summary>
        /// 异步上传数据。
        /// </summary>
        /// <param name="data"></param>
        private void UploadData(ProjectSubContractor data)
        {
            try
            {
                DateTime start = DateTime.Now;
                OutputResult result = data.Upload();
                OutputContext context = new OutputContext(result);
                OutputResult final = context.NextCall();
                // 通知上传完成事件订阅者
                OnUploadCompleted(new UploadCompletedEventArgs(start, data, final));
            }
            catch (Exception e)
            {
                LogUtils4Error.Logger.Debug($"Exception: ProjectSubContractorUploader.UploadData: { e.Message }. data: { data.Serialize2JSON() }");
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
