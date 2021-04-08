using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LaborAndSocialSecurity.Uploaders
{
    public class ProjectWorkerUploader : Uploader<ProjectWorker>
    {
        private string teamSysNo;
        private readonly Team team;

        public ProjectWorkerUploader(string teamSysNo, Team team)
        {
            this.teamSysNo = teamSysNo;
            this.team = team;
        }

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ProjectWorker> GetData(object parm)
        {
            // 数量不能超过 5，此处固定 1条
            const int size = 1;
            
            DataSet set = DBHelperMySQL.TryQuery($"SELECT worker_id, company_id, project_id, cooperator_id, group_id, person_id, `name`, sex, birthday, id_card, profession_id, role_id, nation, address, mobile, entry_time FROM f_worker S WHERE status IN (1,2) AND group_id = { this.team.associated.group_id } AND cooperator_id = { this.team.associated.cooperator_id } AND project_id = { this.team.associated.project_id };");

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

        protected override bool HasSuccessfulUploaded(ProjectWorker data, out DateTime time, out OutputResult result)
        {
            var b = base.HasSuccessfulUploaded(data, out time, out result);
            if (b) return true;

            string code = result?.code;
            if (OutputCode.人员已存在.Equals(code)) return true;

            return false;
        }
    }
}
