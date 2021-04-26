using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LaborAndSocialSecurity.Uploaders
{
    public class WorkerAttendanceUploader : Uploader<WorkerAttendance>
    {
        private readonly ProjectWorker workers;

        public WorkerAttendanceUploader(ProjectWorker workers)
        {
            this.workers = workers;
            this.parm4GetData = workers.workerList.FirstOrDefault();
        }

        #region 静态成员
        private static readonly string QueryString;
        // 缓存查询结果
        private static readonly DataTable cache = new DataTable();
        static WorkerAttendanceUploader()
        {
            QueryString = $"SELECT S.record_id, S.project_id, S.worker_id, S.device_id, S.record_time, S.type FROM zhgd_person.d_card_record S INNER JOIN zhgd_lw.f_worker W ON S.worker_id = W.worker_id INNER JOIN zhgd_lw.f_group G ON W.group_id = G.group_id INNER JOIN zhgd_lw.f_cooperator C ON G.cooperator_id = C.cooperator_id WHERE S.record_time BETWEEN '2021-04-17 00:00:00' AND '2021-04-17 23:59:59' AND C.project_id = { HjApiCaller.Project_id };";
            var resultSet = DBHelperMySQL.TryQuery(QueryString);
            cache = resultSet.Tables[0];
        }
        #endregion

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<WorkerAttendance> GetData(object parm)
        {
            Worker worker = parm as Worker;
            // 此处必须固定 1条
            const int size = 1;
            var filtered = cache.Select($"worker_id = { worker.associated.worker_id }");

            double d = filtered.Length * 1d / size;
            int total = (int)Math.Ceiling(d);

            for (int i = 0; i < total; i++)
            {
                yield return new WorkerAttendance
                {
                    projectCode = this.workers.projectCode,
                    teamSysNo = this.workers.teamSysNo,
                    dataList = from row in filtered.Skip(i * size).Take(size)
                               select new Attendance
                               {
                                   id = Convert.ToInt32(row["record_id"]),
                                   idcard = EncryptUtils.Encrypt(worker.associated.id_card, HjApiCaller.Appsecret),
                                   date = Convert.ToDateTime(row["record_time"]).ToString("yyyy-MM-dd HH:mm:ss"),
                                   direction = Attendance.ConvertDirection(Convert.ToInt32(row["type"]))
                               }
                };
            }
        }

        protected override bool HasSuccessfulUploaded(WorkerAttendance data, out DateTime time, out OutputResult result)
        {
            var b = base.HasSuccessfulUploaded(data, out time, out result);
            if (b) return true;

            string r = result?.data?["result"]?.ToString() ?? string.Empty;
            if (r.IndexOf("班组或者合同不存在") > -1) return true;

            return false;
        }
    }
}
