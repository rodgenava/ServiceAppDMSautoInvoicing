using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppDMSautoInvoicing
{
    public interface IDMSQueryService
    {
        Task<Invoicing> checkDBforInvoicing();
        Task<bool> UpdateDBinvoicing(string RCRNumber = "");
        Task<string> InsertDBinvoicing(string RCRNumber = "");
    }
}
