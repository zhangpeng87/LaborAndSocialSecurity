using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    /// <summary>
    /// 企业基本信息。
    /// </summary>
    public class EnterpriseInfo
    {
        /// <summary>
        /// 企业统一社会信用代码
        /// </summary>
        public string corpCode;
        /// <summary>
        /// 企业名称
        /// </summary>
        public string corpName;
        /// <summary>
        /// 区域编码
        /// </summary>
        public string areaCode;
        /// <summary>
        /// 公司类型。参考参建单位类型字典表
        /// </summary>
        public string type;
        /// <summary>
        /// 注册地
        /// </summary>
        public string registerAddress;
        /// <summary>
        /// 单位地址
        /// </summary>
        public string address;
        /// <summary>
        /// Email
        /// </summary>
        public string email;
        /// <summary>
        /// 法人姓名
        /// </summary>
        public string legalMan;
        /// <summary>
        /// 法人联系方式
        /// </summary>
        public string legalManTel;
        /// <summary>
        /// 联系人
        /// </summary>
        public string linkman;
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string linkTel;
        /// <summary>
        /// 备注
        /// </summary>
        public string remark;
    }
}
