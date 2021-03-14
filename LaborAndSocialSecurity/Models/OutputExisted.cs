using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;

namespace LaborAndSocialSecurity.Models
{
    /// <summary>
    /// 重复上传。
    /// </summary>
    public class OutputExisted : OutputSuper
    {
        private object _uploadedData;

        public OutputExisted(object uploadedData)
        {
            this._uploadedData = uploadedData;
        }

        public override OutputResult NextCall()
        {
            if (this._uploadedData is Team)
            {
                // 上传的班组已存在的情况
                string teamSysNo = null;
                var d = this._uploadedData as Team;
                
                using (TextFieldParser parser = new TextFieldParser(@"D:\LogFile\LaborAndSocialSecurity\UploadedInfo\Team.csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        string[] fields = parser.ReadFields();
                        if (fields[0] == d.associated.group_id.ToString())
                        {
                            teamSysNo = fields[7];
                            break;
                        }
                    }
                }

                return new OutputResult
                {
                    code = OutputCode.重复上传,
                    message = "重复上传！",
                    data = new JObject
                    {
                        new JProperty("result", new JObject { new JProperty("teamSysNo", teamSysNo) })
                    }
                };
            }

            return null;
        }
    }
}
