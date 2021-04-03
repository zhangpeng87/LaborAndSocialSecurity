using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Uploaders;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity
{
    public class Program
    {
        private static bool anyExceptions = false;
        #region 上传客户端

        static void Main(string[] args)
        {
            // 初始化上传标段参数
            if (!HjApiCaller.TryInit())
                Environment.Exit(0);
            
            try
            {
                Console.WriteLine($"当前正在上传标段：{ HjApiCaller.ProjectName }.");
                StartUpload();
            }
            catch (Exception e)
            {
                LogUtils4Error.Logger.Error(e.Message);
                anyExceptions = true;
            }
            finally
            {
                // 1、日志记录完成
                LogUtils4Debug.Logger.Debug($"================={ HjApiCaller.ProjectName }，已上传完成！=================");
                LogUtils.Logger.Info($"{ HjApiCaller.ProjectName },{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },上传完成并{ (anyExceptions ? "有" : "无")}异常发生");

                // 2、打开新的程序
                Process.Start("LaborAndSocialSecurity.exe");

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
            string status = e.UploadedResult.data?["status"]?.ToString();

            if (OutputCode.成功.Equals(code) && AsyncStatus.处理成功.Equals(status)) isSuccess = true;

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
            //workerUploader.UploadCompleted += SaveWorkerInfo;
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
            LogUtils4Debug.Logger.Debug($"The args of { sender.GetType().Name }.UploadCompleted: { e.Serialize2JSON() }");
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

        static void Main100(string[] args)
        {
            HjApi api = new HjApi
            {
                Endpoint = "open/api/get",
                Method = "Project.Info",
                Version = "2.1"
            };

            HjApiCaller.AppId = "db4cad1e537443478718d518f1943c2a";
            HjApiCaller.Appsecret = "af25f376ca4240a4927c487bf965de72";
            HjApiCaller.Host = @"http://219.138.224.85:7004";

            var input = new
            {
                projectName = "新建湖北鄂州民用机场工程信息弱电工程（一标段）",        // 项目名称
                contractorCorpCode = "91420100744771385L"                               //  施工方统一社会信用代码
            };

            var output = api.Call(input);

            Console.WriteLine(output.Serialize2JSON());
        }
        
        static void Main200(string[] args)
        {
            string s = "{\"code\":\"1\",\"data\":null}";
            JObject j = JObject.Parse(s);

            // j["message"]?.ToString().IndexOf("频繁") > -1
            
        }

        static void Main300(string[] args)
        {
            int index = 1;
            int size = 10;
            string code = "ff80808176c65dd00176d66faff20015";

            var result = Team.Query(index, size, code);

        }

        static void Main400(string[] args)
        {
            string code = "ff80808176c65dd00176d66faff20015";

            var result = Team.SysNo(code, "土方四队");
        }

        static void Main500(string[] args)
        {
            string json = "{\"name\": \"hello\",\"age123\": \"success\",\"hobby\": [{\"obj1\": \"6\",\"obj2\": \"7\",\"obj3\": \"10\"}, {\"obj1\": \"6\",\"obj2\": \"7\",\"obj3\": \"10\"}]}";
            var q = JObject.Parse(json);

            bool r = q.ContainsKey("age13");
        }

        static void Main600(string[] args)
        {
            string j = @"
                        {
                            ""code"": 0,
                            ""msg"": ""SUCCESS"",
                            ""data"": {
                                        ""name"": ""顾晓雪"",
                                ""nickName"": ""顾晓雪"",
                                ""phoneNumber"": ""15317870854"",
                                ""countryCode"": ""86"",
                                ""email"": ""505588907@qq.com"",
                                ""sex"": 1,
                                ""city"": ""上海市/市辖区/杨浦区"",
                                ""work"": ""施工"",
                                ""company"": ""译筑科技"",
                                ""position"": ""BIM工程师"",
                                ""project"": ""建筑"",
                                ""isInvited"": false,
                                ""protected"": false,
                                ""createdAt"": ""2020-10-14T01:29:21.530Z"",
                                ""updatedAt"": ""2020-10-14T01:30:53.694Z"",
                                ""_id"": ""5f865471e3c1f9000fd61b88""
                            }
                        }";

            var result = JObject.Parse(j);
            var b = result.ContainsKey("data");
            var v = result.SelectToken("$.data.phoneNumber")?.ToString();
        }

        static void Main700(string[] args)
        {
            List<int> list = new List<int> { 2, 7, 4, 2 };
            string result = string.Join<int>(",", list);
            Console.WriteLine(result);
        }
    }
}
