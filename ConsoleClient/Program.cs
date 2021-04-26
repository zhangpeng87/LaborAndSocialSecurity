using LaborAndSocialSecurity.Models;
using LaborAndSocialSecurity.Uploaders;
using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleClient
{
    class Program
    {
        private static readonly string alias = "smz";

        static void Main001(string[] args)
        {
            string sitype = "/person/team/list";

            Hashtable pars = new Hashtable
            {
                { "projectCode", "1001622" },
            };

            JObject resp = PmApiCaller.CallOpenApi(alias, sitype, pars);
        }

        static void Main002(string[] args)
        {
            var res = Math.Pow(10, 308);
            Console.WriteLine(res.ToString());
        }

        static void Main003(string[] args)
        {
            Console.WriteLine(RequestMethod.GET.Description());
        }

        static void Main007(string[] args)
        {
            string json = "{\"data\":{\"requestSerialCode\": null},\"code\": \"0\",\"message\": \"调用成功\"}";
            var result = JObject.Parse(json);
            //string s = result.SelectToken("data").SelectToken("requestSerialCode")?.ToString();
            string s = result["data"]["requestSerialCode"]?.ToString();
        }

        const string Private_key = "8673fdd3ea8654b5690c0a501effa328";              // 开发者秘钥
        
        static void Main008(string[] args)
        {
            string sData = "siwei";
            var result = EncryptUtils.EncryptPm(sData, Private_key);

            Console.WriteLine(result);
        }

        static void Main000(string[] args)
        {
            EnterpriseUploader enterprise = new EnterpriseUploader();
            ProjectInfoUploader project = new ProjectInfoUploader();
            CooperatorInfoUploader cooperator = new CooperatorInfoUploader();

            enterprise.BeginUpload(null);
            enterprise.UploadCompleted += (sender, e) => { project.BeginUpload(null); };

            project.UploadCompleted += (sender, e) => { cooperator.BeginUpload(null); };
        }

        static void Main011(string[] args)
        {
            var info = new
            {
                pageIndex = 1,
                pageSize = 10,
                projectCode = "ff8080817682f02601768933e2540009",
                buildCorpCode = "91420100177738297E"
            };

            JObject result = null;
            try
            {
                HjApi api = new HjApi() { Endpoint = "open/api/get", Method = "Project.Query", Version = "1.0" };
                result = HjApiCaller.CallOpenApi(api, info);
            }
            catch (Exception e)
            {
                throw;
            }

            Console.WriteLine(result?.ToString());
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Assembly.GetExecutingAssembly().Location);
        }
    }
}
