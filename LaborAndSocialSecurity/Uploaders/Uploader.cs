﻿using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Uploaders
{
    public delegate void UploadCompletedEventHandler(object sender, UploadCompletedEventArgs e);

    public abstract class Uploader<T> where T : IUploadable
    {
        protected object parm4GetData;

        /// <summary>
        /// 上传完成事件。
        /// </summary>
        public event UploadCompletedEventHandler UploadCompleted;

        /// <summary>
        /// 开始上传相关数据。
        /// </summary>
        public virtual void BeginUpload()
        {
            // 获取上传数据
            var list = this.GetData(parm4GetData);

            //DateTime time;
            //OutputResult result;

            //// 进行上传数据
            //foreach (T item in list)
            //    if (HasSuccessfulUploaded(item, out time, out result))
            //        this.Notify(time, item, result);
            //    else
            //        this.UploadData(item);

            list.AsParallel()
                .WithDegreeOfParallelism(10)
                .ForAll(item =>
                {
                    DateTime time;
                    OutputResult result;

                    if (HasSuccessfulUploaded(item, out time, out result))
                        this.Notify(time, item, result);
                    else
                        this.UploadData(item);
                });
        }
        
        protected abstract IEnumerable<T> GetData(object parm = null);

        /// <summary>
        /// 检查是否以前上传过并成功
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasSuccessfulUploaded(T data, out DateTime time, out OutputResult result)
        {
            time = default(DateTime);
            result = default(OutputResult);

            var record = this.GetUploadRecord(data);
            if (record == null) return false;       // 未上传过

            time = record.UploadTime;
            data = JsonConvert.DeserializeObject<T>(record.UploadedData);
            result = JsonConvert.DeserializeObject<OutputResult>(record.UploadResult);

            string code = result.code;
            string status = result.data?["status"]?.ToString();

            // 成功/失败地上传过
            return OutputCode.成功.Equals(code) && AsyncStatus.处理成功.Equals(status);
        }

        /// <summary>
        /// 同步上传数据。
        /// </summary>
        /// <param name="data"></param>
        protected void UploadData(T data)
        {
            try
            {
                LogUtils4Debug.Logger.Debug($"开始上传数据：{ data.Serialize2JSON() }");
                DateTime start = DateTime.Now;
                OutputResult result = data.Upload();
                OutputContext context = new OutputContext(result, data);
                OutputResult final = context.NextCall();
                // 记录上传情况：上传时间、上传数据、返回结果
                this.SaveUpload(start, data, final);
                // 通知上传完成事件订阅者
                OnUploadCompleted(new UploadCompletedEventArgs(start, data, final, false));
            }
            catch (Exception e)
            {
                LogUtils4Error.Logger.Debug($"Exception: { this.GetType().ToString() }.UploadData: { e.Message }. data: { data.Serialize2JSON() }");
                //throw;
            }
        }

        /// <summary>
        /// 以前上传过并成功了的情况
        /// </summary>
        protected void Notify(DateTime time, T data, OutputResult result)
        {
            LogUtils4Debug.Logger.Debug($"数据已成功地上传过：{ data.Serialize2JSON() }");
            OnUploadCompleted(new UploadCompletedEventArgs(time, data, result, true));
        }

        #region 读写上传记录

        /// <summary>
        /// 记录上传情况：上传时间、上传数据、返回结果
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        /// <param name="result"></param>
        private void SaveUpload(DateTime time, T data, OutputResult result)
        {
            string sql = $"INSERT INTO upload_record ( data_type, data_id, upload_time, uploaded_data, upload_result, project_name ) VALUES  ( '{ data.GetType().Name }', { data.DataId }, '{ time.ToString("yyyy-MM-dd HH:mm:ss") }', '{ data.Serialize2JSON() }', '{ result.Serialize2JSON() }', '{ HjApiCaller.ProjectName }' );";
            ArDBConnection.ExceuteSQLNoReturn(sql);
        }

        /// <summary>
        /// 获取上传记录。
        /// </summary>
        /// <returns></returns>
        private UploadRecord GetUploadRecord(T data)
        {
            string sql = $"SELECT TOP 1 S.data_type, S.data_id, S.uploaded_data, S.upload_result, S.upload_time FROM upload_record S WHERE S.data_type = '{ data.GetType().Name }' AND S.data_id = { data.DataId } ORDER BY S.upload_time DESC;";
            var result = ArDBConnection.ExceuteSQLDataTable(sql);

            if (result == null || result.Rows.Count == 0) return null;
            DataRow first = result.AsEnumerable().First();
            UploadRecord record = new UploadRecord
            {
                DataType = Convert.ToString(first["data_type"]),
                DataId = Convert.ToInt32(first["data_id"]),
                UploadTime = Convert.ToDateTime(first["upload_time"]),
                UploadedData = Convert.ToString(first["uploaded_data"]),
                UploadResult = Convert.ToString(first["upload_result"]),
            };

            return record;
        }

        #endregion

        /// <summary>
        /// 可以供继承类的重写。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUploadCompleted(UploadCompletedEventArgs e)
        {
            this.UploadCompleted?.Invoke(this, e);
        }
    }
}
