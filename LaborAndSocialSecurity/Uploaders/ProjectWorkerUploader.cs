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
    public class ProjectWorkerUploader
    {
        private string teamSysNo;
        private readonly Team team;

        /// <summary>
        /// 上传完成事件。
        /// </summary>
        public event UploadCompletedEventHandler UploadCompleted;

        public ProjectWorkerUploader(string teamSysNo, Team team)
        {
            this.teamSysNo = teamSysNo;
            this.team = team;
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
        private IEnumerable<ProjectWorker> GetData()
        {
            // 数量不能超过 5
            int size = 1;
            
            DataSet set = DBHelperMySQL.Query($"SELECT worker_id, company_id, project_id, cooperator_id, group_id, person_id, `name`, sex, birthday, id_card, profession_id, role_id, nation, address, mobile, entry_time FROM f_worker S WHERE status IN (1,2) AND group_id = { this.team.associated.group_id } AND cooperator_id = { this.team.associated.cooperator_id } AND project_id = { this.team.associated.project_id };");

            double d = set.Tables[0].Rows.Count * 1d / size;
            int total = (int)Math.Ceiling(d);

            for (int i = 0; i < total; i++)
            {
                yield return new ProjectWorker
                {
                    projectCode = this.team.projectCode,
                    corpCode = this.team.corpCode,
                    teamName = this.team.teamName,
                    teamSysNo = teamSysNo,
                    corpName = this.team.corpName,
                    workerList = from row in set.Tables[0].AsEnumerable().Skip(i * size).Take(size)
                                 let tmp = Worker.GetHjWorkType(Convert.ToInt32(row["role_id"]), Convert.ToInt32(row["profession_id"]))
                                 let _idcard = string.IsNullOrEmpty(row["id_card"].ToString()) ? "未知" : row["id_card"].ToString().ToUpper()
                                 select new Worker
                                 {
                                     workerName = string.IsNullOrEmpty(row["name"].ToString()) ? "未知" : row["name"].ToString(),
                                     idcard = EncryptUtils.Encrypt(_idcard, HjApiCaller.Appsecret),
                                     sex = DBNull.Value.Equals(row["sex"]) ? "0" : Worker.GetSex(Convert.ToInt32(row["sex"])),
                                     birthday = string.IsNullOrEmpty(row["birthday"].ToString()) ? "1990-01-01" : Convert.ToDateTime(row["birthday"].ToString()).ToString("yyyy-MM-dd"),
                                     workRole = tmp.Item1,                                      // 工人类型
                                     workType = tmp.Item2,                                      // 工人工种
                                     nation = Worker.GetHjNation(row["nation"].ToString()),     // 民族
                                     address = string.IsNullOrEmpty(row["address"].ToString()) ? "未知" : row["address"].ToString(),
                                     headImage = Worker.DefaultHeadImage,
                                     tel = string.IsNullOrEmpty(row["mobile"].ToString()) ? "13888888888" : row["mobile"].ToString(),
                                     startDate = DBNull.Value.Equals(row["entry_time"]) ? "1900-01-01" : Convert.ToDateTime(row["entry_time"]).ToString("yyyy-MM-dd"),
                                     endDate = new DateTime(2021, 6, 30).ToString("yyyy-MM-dd"),
                                     associated = new WorkerAssociated
                                     {
                                         worker_id = Convert.ToInt32(row["worker_id"]),
                                         id_card = _idcard
                                     }
                                 }
                };
            }
        }

        /// <summary>
        /// 异步上传数据。
        /// </summary>
        /// <param name="data"></param>
        private void UploadData(ProjectWorker data)
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
                LogUtils4Error.Logger.Debug($"Exception: ProjectWorkerUploader.UploadData: { e.Message }. data: { data.Serialize2JSON() }");
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
