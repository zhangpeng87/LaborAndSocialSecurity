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

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<WorkerAttendance> GetData(object parm)
        {
            Worker worker = parm as Worker;
            // 此处必须固定 1条
            const int size = 1;

            DataSet set = DBHelperMySQL.Query($"SELECT record_id, project_id, worker_id, device_id, record_time, type FROM d_card_record S WHERE S.worker_id = { worker.associated.worker_id } AND record_time BETWEEN '2021-04-01 00:00:00' AND '2021-04-01 23:59:59' ORDER BY S.record_time DESC;", "zhgd_person");

            double d = set.Tables[0].Rows.Count * 1d / size;
            int total = (int)Math.Ceiling(d);

            for (int i = 0; i < total; i++)
            {
                yield return new WorkerAttendance
                {
                    projectCode = this.workers.projectCode,
                    teamSysNo = this.workers.teamSysNo,
                    dataList = from row in set.Tables[0].AsEnumerable().Skip(i * size).Take(size)
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
