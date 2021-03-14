using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public class WorkerAssociated
    {
        public int worker_id;
        public string id_card;
    }

    public class Worker
    {
        /// <summary>
        /// 工人姓名。
        /// </summary>
        public string workerName;
        /// <summary>
        /// 身份证号AES。
        /// </summary>
        public string idcard;
        /// <summary>
        /// 性别传字典表数字值，不要传中文。参考性别字典表。
        /// </summary>
        public string sex;
        /// <summary>
        /// 生日。
        /// </summary>
        public string birthday;
        /// <summary>
        /// 当前工种。传字典表数字值，不要传中文。参考工人工种字典表。
        /// </summary>
        public string workType;
        /// <summary>
        /// 工人类型。传字典表数字值，不要传中文。参考工人类型字典表。
        /// </summary>
        public string workRole;
        /// <summary>
        /// 人脸采集相片。不超过50KB的Base64字符串不要带任何图片格式标识符（image/png;base64 等），只传图片内容字符串。
        /// </summary>
        public string facePic;
        /// <summary>
        /// 务工卡号。
        /// </summary>
        public string cardNumber;
        /// <summary>
        /// 工资卡号。
        /// </summary>
        public string payRollBankCardNumber;
        /// <summary>
        /// 工资卡银行。传字典表数字值，不要传中文。参考银行代码字典表。
        /// </summary>
        public string payRollTopBankCode;
        /// <summary>
        /// 民族。传字典表数字值，不要传中文。参考民族字典表。
        /// </summary>
        public string nation;
        /// <summary>
        /// 住址。
        /// </summary>
        public string address;
        /// <summary>
        /// 头像。不超过50KB的Base64字符串。
        /// </summary>
        public string headImage;
        /// <summary>
        /// 政治面貌。传字典表数字值，不要传中文。参考政治面貌字典表。
        /// </summary>
        public string politicsType;
        /// <summary>
        /// 手机号码，接受空值。。
        /// </summary>
        public string tel;
        /// <summary>
        /// 文化程度。传字典表数字值，不要传中文。参考文化程度字典表。。
        /// </summary>
        public string cultureLevelType;
        /// <summary>
        /// 婚姻状况。传字典表数字值，不要传中文。参考婚姻状况字典表。。
        /// </summary>
        public string maritalStatus;
        /// <summary>
        /// 雇佣开始日期，格式yyyy-mm-dd。
        /// </summary>
        public string startDate;
        /// <summary>
        /// 雇佣结束日期，格式yyyy-mm-dd。
        /// </summary>
        public string endDate;

        /// <summary>
        /// 关联对象。
        /// </summary>
        [JsonIgnore]
        public WorkerAssociated associated;

        /// <summary>
        /// 品茗职位/工种/岗位 -> 会基
        /// </summary>
        /// <param name="role_code_pm"></param>
        /// <param name="profession_code_pm"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetHjWorkType(int role_code_pm, int profession_code_pm)
        {
            var first = (from row in DataDictionary.RoleRelationship.AsEnumerable()
                         where role_code_pm.ToString().Equals(row["role_code_pm"].ToString()) &&
                               profession_code_pm.ToString().Equals(row["profession_code_pm"].ToString())
                         select new
                         {
                             role_code_hj = row["role_code_hj"].ToString(),
                             profession_code_hj = row["profession_code_hj"].ToString()
                         }).FirstOrDefault();

            return new Tuple<string, string>(first?.role_code_hj ?? "14", first?.profession_code_hj ?? "1000");
        }

        /// <summary>
        /// 查找对应会基的民族编码。
        /// </summary>
        /// <param name="nation_type_pm"></param>
        /// <returns></returns>
        public static string GetHjNation(string nation_type_pm)
        {
            var first = (from row in DataDictionary.Nations.AsEnumerable()
                         where row["nation_type_hj"].ToString().Equals(nation_type_pm?.Trim() + "族") ||
                               row["nation_type_hj"].ToString().Equals(nation_type_pm?.Trim())
                         select new
                         {
                             nation_code_hj = row["nation_code_hj"].ToString()
                         }).FirstOrDefault();

            return first?.nation_code_hj ?? "01";           // 默认汉族
        }

        /// <summary>
        /// 获取对应会基的性别编码。
        /// </summary>
        /// <param name="sex_code_pm"></param>
        /// <returns></returns>
        public static string GetSex(int sex_code_pm)
        {
            var first = (from row in DataDictionary.SexDic.AsEnumerable()
                         where sex_code_pm.ToString().Equals(row["sex_code_pm"].ToString())
                         select new
                         {
                             sex_code_hj = row["sex_code_hj"].ToString()
                         }).FirstOrDefault();

            return string.IsNullOrEmpty(first?.sex_code_hj) ? "0" : first.sex_code_hj;      // 默认男
        }

        /// <summary>
        /// 默认头像（base64格式）。
        /// </summary>
        public static string DefaultHeadImage
        {
            get
            {
                return @"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";
            }
        }
    }
}
