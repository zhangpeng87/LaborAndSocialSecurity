using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public class OutputContext
    {
        private OutputSuper _outputSuper;
        protected OutputResult _result;

        public OutputContext(OutputResult result, object uploadedData)
        {
            this._result = result;

            switch (result.code)
            {
                case "0":
                    this._outputSuper = new OutputAsyncQuery(result.data["requestserialcode"].ToString());
                    break;

                case "2007":
                    this._outputSuper = new OutputExisted(uploadedData);
                    break;

                default:
                    this._outputSuper = new OutputDefault(result, uploadedData);
                    break;
            }
        }

        public OutputContext(OutputResult result)
            : this(result, null)
        {
        }

        public OutputResult NextCall()
        {
            return this._outputSuper?.NextCall() ?? this._result;
        }
    }
}
