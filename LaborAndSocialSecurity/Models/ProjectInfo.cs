using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    /// <summary>
    /// 项目基本信息。
    /// </summary>
    public class ProjectInfo
    {
        /// <summary>
        /// 施工方统一社会信用代码
        /// </summary>
        public string contractorCorpCode;
        /// <summary>
        /// 施工方名称
        /// </summary>
        public string contractorCorpName;
        /// <summary>
        /// 项目名称
        /// </summary>
        public string name;
        /// <summary>
        /// 建设方名称
        /// </summary>
        public string buildCorpName;
        /// <summary>
        /// 建设方统一社会信用代码
        /// </summary>
        public string buildCorpCode;
        /// <summary>
        /// 所属区域。参考行政区划字典表
        /// </summary>
        public string areaCode;
        /// <summary>
        /// 工程造价，单位：（元）
        /// </summary>
        public double invest;
        /// <summary>
        /// 开工日期，精确到天，格式：yyyy-MM-dd
        /// </summary>
        public string startDate;
        /// <summary>
        /// 建设周期，单位：（天）
        /// </summary>
        public int timeLimit;
        /// <summary>
        /// 项目办理人姓名
        /// </summary>
        public string linkMan;
        /// <summary>
        /// 联系方式
        /// </summary>
        public string linkTel;
        /// <summary>
        /// 项目状态。参考项目状态字典表
        /// </summary>
        public string prjStatus;
        /// <summary>
        /// WGS84经度
        /// </summary>
        public string lat;
        /// <summary>
        /// WGS84纬度
        /// </summary>
        public string lng;
        /// <summary>
        /// 项目地址
        /// </summary>
        public string address;
        /// <summary>
        /// 工资发放日 ，可为空值(1-31)
        /// </summary>
        public int salaryDay;
        /// <summary>
        /// 项目类型。（新加字段）参考项目类型字典表
        /// </summary>
        public string type;
        /// <summary>
        /// 投资类别。（新加字段）参考项目投资类别字典表
        /// </summary>
        public string investment;
        /// <summary>
        /// 行业。（新加字段）参考项目行业字典表
        /// </summary>
        public string industry;
    }
}
