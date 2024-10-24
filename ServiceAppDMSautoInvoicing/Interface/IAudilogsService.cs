using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppDMSautoInvoicing
{
    public interface IAudilogsService
    {
        void writeLogs(string logsdata = "");
    }
}
