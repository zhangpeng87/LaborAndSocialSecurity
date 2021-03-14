using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Models
{
    public abstract class OutputSuper
    {
        public OutputSuper()
        {

        }

        public abstract OutputResult NextCall();
    }
}
