using LaborAndSocialSecurity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    /// <summary>
    /// 默认处理策略。
    /// </summary>
    public class OutputDefault : OutputSuper
    {
        private OutputResult _result;
        private object _data;

        public OutputDefault(OutputResult result, object uploadedData)
        {
            this._result = result;
            this._data = uploadedData;
        }

        public override OutputResult NextCall()
        {
            var message = new
            {
                OutputResult = this._result,
                UploadedData = this._data
            };

            LogUtils4Error.Logger.Debug(message.Serialize2JSON());

            return null;
        }
    }
}
