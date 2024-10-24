using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppDMSautoInvoicing
{
    public class Invoicing
    {
        public string Location { get; set; }
        public string PONumber { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public DateTime? RCRDate { get; set; }
        public string RCRNumber { get; set; }
        public string RCRAmount { get; set; }
        public string AdjustedRCRAmount { get; set; }
        public string SIAmount { get; set; }
        public DateTime? PORADate { get; set; }
        public string PORAAmount { get; set; }
        public string FinalAmount { get; set; }
        public string RCRStatus { get; set; }
        public DateTime? DateProcessed { get; set; }
        public string SINumber { get; set; }

        public DateTime? SIDate { get; set; }
        public string VatCode { get; set; }
        public string PaymentTerms { get; set; }
        public string AutoNumber { get; set; }
    }
}
