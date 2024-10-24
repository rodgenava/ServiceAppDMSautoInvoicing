using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppDMSautoInvoicing
{
    public interface IAutoInvoicingService
    {
        void createAutoInvoicing();
        Task<(bool, string)> writeInvoice(Invoicing invoicing);
    }
}
