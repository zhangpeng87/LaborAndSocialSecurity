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
    public class WorkerAttendanceUploader
    {
        private readonly ProjectWorker workers;

        /// <summary>
        /// 上传完成事件。
        /// </summary>
        public event UploadCompletedEventHandler UploadCompleted;

        public WorkerAttendanceUploader(ProjectWorker workers)
        {
            this.workers = workers;
        }

        /// <summary>
        /// 开始上传相关数据。
        /// </summary>
        public void BeginUpload()
        {
            foreach (var worker in this.workers.workerList)
            {
                // 获取上传数据
                var list = this.GetData(worker);
                // 进行上传数据
                foreach (var item in list)
                    this.UploadData(item); 
            }
        }

        /// <summary>
        /// 获取上传数据。
        /// </summary>
        /// <returns></returns>
        private IEnumerable<WorkerAttendance> GetData(Worker worker)
        {
            int size = 20;

            DataSet set = DBHelperMySQL.Query($"SELECT project_id, worker_id, device_id, record_time, type FROM d_card_record S WHERE S.worker_id = { worker.associated.worker_id } AND record_time BETWEEN '2021-01-23 00:00:00' AND '2021-02-05 23:59:59' ORDER BY S.record_time DESC;", "zhgd_person");

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
                                   idcard = EncryptUtils.Encrypt(worker.associated.id_card, HjApiCaller.Appsecret),
                                   date = Convert.ToDateTime(row["record_time"]).ToString("yyyy-MM-dd HH:mm:ss"),
                                   direction = Attendance.ConvertDirection(Convert.ToInt32(row["type"]))
                               }
                };
            }
        }

        /// <summary>
        /// 同步上传数据。
        /// </summary>
        /// <param name="data"></param>
        private void UploadData(WorkerAttendance data)
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
                LogUtils4Error.Logger.Debug($"Exception: WorkerAttendanceUploader.UploadData: { e.Message }. data: { data.Serialize2JSON() }");
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
