using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public class Attendance
    {
        /// <summary>
        /// 身份证号 AES。
        /// </summary>
        public string idcard;
        /// <summary>
        /// 考勤时间，yyyy-MM-dd HH:mm:ss。
        /// </summary>
        public string date;
        /// <summary>
        /// 进出方向。参考工人考勤方向字典表。
        /// </summary>
        public string direction;

        /// <summary>
        /// 转换工人考勤方向。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ConvertDirection(int type)
        {
            if (type == 1) return "01";
            else return "02";
        }
    }
}
