using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    /// <summary>
    /// 参建单位基本信息。
    /// </summary>
    public class CooperatorInfo
    {
        /// <summary>
        /// 本平台分配的项目ID
        /// </summary>
        public string projectCode;
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string corpCode;
        /// <summary>
        /// 企业名称
        /// </summary>
        public string corpName;
        /// <summary>
        /// 参建类型。参考参建单位类型字典表
        /// </summary>
        public string corpType;
        /// <summary>
        /// 退场时间，格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string entryTime;
        /// <summary>
        /// 退场时间，格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string exitTime;
        /// <summary>
        /// 企业负责人（新加字段）
        /// </summary>
        public string manager;
        /// <summary>
        /// 企业负责人电话（新加字段）
        /// </summary>
        public string managerTel;
    }
}
