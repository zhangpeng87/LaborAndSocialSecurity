using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Uploaders;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Timer = System.Threading.Timer;
using System.Reflection;

namespace LaborAndSocialSecurity
{
    public class Program
    {
        #region extern functions

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int ProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        // Also consider whether you're being lazy or not.
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion

        private static bool anyExceptions = false;
        private static NotifyIcon notifyIcon = new NotifyIcon();
        private static bool Visible = false;

        #region 上传客户端

        private static readonly Timer timer = new Timer(TimerCallback, null, 1000, 3000);

        private static readonly object obj = new object();
        private static void TimerCallback(object s)
        {
            lock (obj)
            {
                Console.Clear();
                Console.WriteLine($"当前正在上传标段：{ HjApiCaller.ProjectName }, 活动中的线程数量：{ Process.GetCurrentProcess().Threads.Count }, 命令队列中的数量：{ Dispatcher.Instance.Count }."); 
            }
        }

        static void Main(string[] args)
        {
            SetConsoleWindowVisibility(Visible);
            notifyIcon.DoubleClick += (s, e) =>
            {
                Visible = !Visible;
                SetConsoleWindowVisibility(Visible);
            };
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Visible = true;
            notifyIcon.Text = "实名制信息推送程序【鄂州市劳保】";

            // 初始化上传标段参数
            if (!HjApiCaller.TryInit())
                Environment.Exit(0);
            
            try
            {
                Task.Factory
                    .StartNew(StartUpload)
                    .ContinueWith(t => 
                    {
                        anyExceptions = (t.Exception != null);
                        Application.Exit();
                    });
            }
            catch (Exception e)
            {
                LogUtils4Error.Logger.Error(e.Message);
            }
            finally
            {
                Application.Run();
                notifyIcon.Visible = false;

                // 1、日志记录完成
                LogUtils4Debug.Logger.Debug($"================={ HjApiCaller.ProjectName }，已上传完成！=================");
                LogUtils.Logger.Info($"{ HjApiCaller.ProjectName },{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },上传完成并{ (anyExceptions ? "有" : "无")}异常发生");

                DBHelperMySQL.Dispose();
                // 2、打开新的程序
                var p = Process.Start(Assembly.GetExecutingAssembly().Location);

                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 流程步骤如下：
        ///  参建单位 -> 班组信息 -> 人员信息 -> 考勤信息
        ///  查询 -> 上传 -> 询问 -> 告知
        /// </summary>
        static void StartUpload()
        {
            // 分配的项目ID（会基）
            string projectCode = HjApiCaller.ProjectCode;
            // 施工项目ID（品茗）
            int project_id = HjApiCaller.Project_id;

            if (string.IsNullOrEmpty(projectCode))
            {
                Console.WriteLine("分配的项目ID（会基）为空！");
                return;
            }

            // 上传参建单位
            ProjectSubContractorUploader uploader = new ProjectSubContractorUploader(projectCode, project_id);
            uploader.UploadCompleted += UploadCompletedEventDebugLogHandler;
            uploader.UploadCompleted += CooperatorInfoUploadCompletedEventHandler;

            uploader.BeginUpload();
        }

        /// <summary>
        /// 参建单位上传完成事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CooperatorInfoUploadCompletedEventHandler(object sender, UploadCompletedEventArgs e)
        {
            bool isSuccess = false;
            string code = e.UploadedResult.code;
            string status = e.UploadedResult.data?["status"]?.ToString();

            if (OutputCode.成功.Equals(code) && AsyncStatus.处理成功.Equals(status)) isSuccess = true;
            else if (OutputCode.重复上传.Equals(code)) isSuccess = true;

            // 只有单位上传成功，才进行班组信息上传
            if (!isSuccess) return;

            var data = (ProjectSubContractor)e.UploadedData;
            // 上传班组信息
            TeamInfoUploader uploader = new TeamInfoUploader(data.projectCode, data.associated.cooperator_id);
            uploader.UploadCompleted += UploadCompletedEventDebugLogHandler;        // 打印调试日志
            uploader.UploadCompleted += TeamInfoUploadSucceedEventHandler;          // 记录生成编号
            uploader.UploadCompleted += TeamInfoUploadCompletedEventHandler;        // 上传人员信息

            uploader.BeginUpload();
        }

        /// <summary>
        /// 班组信息上传完成（成功）事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void TeamInfoUploadSucceedEventHandler(object sender, UploadCompletedEventArgs e)
        {
            bool isSuccess = false;
            string code = e.UploadedResult.code;
            string status = e.UploadedResult.data.SelectToken("status")?.ToString();

            if (OutputCode.成功.Equals(code) && 
                AsyncStatus.处理成功.Equals(status) &&
                !e.HasSuccessfulUploaded) isSuccess = true;

            if (!isSuccess) return;

            var team = (Team)e.UploadedData;
            var teamSysNo = e.UploadedResult?.data["result"]["teamSysNo"]?.ToString();

            team.RecordTeamSysNo2File(teamSysNo);
        }

        /// <summary>
        /// 班组信息上传完成事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void TeamInfoUploadCompletedEventHandler(object sender, UploadCompletedEventArgs e)
        {
            bool isSuccess = false;
            string code = e.UploadedResult.code;
            string status = e.UploadedResult.data?["status"]?.ToString();
            
            if (OutputCode.成功.Equals(code) && AsyncStatus.处理成功.Equals(status)) isSuccess = true;
            else if (OutputCode.重复上传.Equals(code)) isSuccess = true;

            // 只有班组上传成功，才进行人员信息上传
            if (!isSuccess) return;

            var data = (Team)e.UploadedData;
            var teamSysNo = e.UploadedResult?.data["result"]["teamSysNo"]?.ToString();

            ProjectWorkerUploader workerUploader = new ProjectWorkerUploader(teamSysNo, data);
            workerUploader.UploadCompleted += UploadCompletedEventDebugLogHandler;
            // 人员上传成功后，上传考勤
            workerUploader.UploadCompleted += WorkerInfoUploadCompletedEventHandler;

            workerUploader.BeginUpload();
        }

        /// <summary>
        /// 人员信息上传完成事件处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void WorkerInfoUploadCompletedEventHandler(object sender, UploadCompletedEventArgs e)
        {
            bool isSuccess = false;
            string code = e.UploadedResult.code;
            string status = e.UploadedResult.data?["status"]?.ToString();

            if (OutputCode.成功.Equals(code) && AsyncStatus.处理成功.Equals(status)) isSuccess = true;
            else if (OutputCode.人员已存在.Equals(code)) isSuccess = true;

            // 只有人员上传成功，才进行考勤信息上传
            if (!isSuccess) return;

            var workers = (ProjectWorker)e.UploadedData;
            WorkerAttendanceUploader attendanceUploader = new WorkerAttendanceUploader(workers);
            attendanceUploader.UploadCompleted += UploadCompletedEventDebugLogHandler;

            attendanceUploader.BeginUpload();
        }

        /// <summary>
        /// 上传完成事件日志处理程序。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void UploadCompletedEventDebugLogHandler(object sender, UploadCompletedEventArgs e)
        {
            //LogUtils4Debug.Logger.Debug($"The args of { sender.GetType().Name }.UploadCompleted: { e.Serialize2JSON() }");
        }
        
        /// <summary>
        /// 人员上传成功后（或已上传过），记录下来。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void SaveWorkerInfo(object sender, UploadCompletedEventArgs e)
        {
            bool isSuccess = false;
            string code = e.UploadedResult.code;
            string status = e.UploadedResult.data?["status"]?.ToString();

            if (OutputCode.成功.Equals(code) && AsyncStatus.处理成功.Equals(status)) isSuccess = true;
            else if (OutputCode.人员已存在.Equals(code)) isSuccess = true;

            if (!isSuccess) return;

            // 只有人员上传成功
            var w = (ProjectWorker)e.UploadedData;
            LogUtils4Worker.Logger.Info($"{ HjApiCaller.ProjectName },{ w.corpName },{ w.teamName },'{ w.workerList.FirstOrDefault()?.associated.id_card },{ w.workerList.FirstOrDefault()?.workerName }");
        }

        #endregion

        public static void SetConsoleWindowVisibility(bool visible)
        {
            IntPtr hWnd = FindWindow(null, Console.Title);
            if (hWnd != IntPtr.Zero)
            {
                if (visible) ShowWindow(hWnd, 1); //1 = SW_SHOWNORMAL           
                else ShowWindow(hWnd, 0); //0 = SW_HIDE               
            }
        }


    }
}
