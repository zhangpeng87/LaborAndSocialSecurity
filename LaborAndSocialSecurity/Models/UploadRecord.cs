using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public class UploadRecord
    {
        public string DataType { get; set; }
        public int DataId { get; set; }
        public string UploadedData { get; set; }
        public string UploadResult { get; set; }
        public DateTime UploadTime { get; set; }
    }
}
