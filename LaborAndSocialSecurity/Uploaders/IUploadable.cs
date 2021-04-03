using LaborAndSocialSecurity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Uploaders
{
    public interface IUploadable
    {
        int DataId { get; }
        OutputResult Upload();
    }
}
