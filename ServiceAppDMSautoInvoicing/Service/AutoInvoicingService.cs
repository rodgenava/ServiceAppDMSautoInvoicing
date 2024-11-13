using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppDMSautoInvoicing
{
    public class AutoInvoicingService : IAutoInvoicingService
    {
        private readonly IConfiguration _configuration;
        private readonly IDMSQueryService _dmsQueryService;
        private readonly IAudilogsService _audilogsService;
        public AutoInvoicingService(IConfiguration configuration, IDMSQueryService dmsQueryService, IAudilogsService audilogsService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dmsQueryService = dmsQueryService;
            _audilogsService = audilogsService;
        }

        public async void createAutoInvoicing()
        {
            Invoicing invoicing = await _dmsQueryService.checkDBforInvoicing();
            if (!string.IsNullOrEmpty(invoicing.RCRNumber))
            {
                invoicing.AutoNumber = await _dmsQueryService.InsertDBinvoicing(invoicing.RCRNumber);
                bool isdoneupdate = await _dmsQueryService.UpdateDBinvoicing(invoicing.RCRNumber);
                if (isdoneupdate)
                {
                    (bool isdone, string sourceFile) = await writeInvoice(invoicing);
                    if (isdone)
                    {
                        //sent Invoice to MMS
                        CopySendInvoiceFile(sourceFile, _configuration.GetSection("Directory:DistanationPath").Value);
                    }
                }
            }
        }
        public async Task<(bool,string)> writeInvoice(Invoicing invoicing)
        {
            try
            {
                DateTime localDate = DateTime.Now;
                string fileName = String.Format("SN{0}.501", localDate.ToString("MMddyy_hhmmss"));
                string path = _configuration.GetSection("Directory:Invoicepath").Value;  // UNC path to the CSV file 
                string fullpath = Path.Combine(path, fileName);
                string invoice = "";

                Directory.CreateDirectory(path);
                if (!File.Exists(fullpath))
                {
                    using (var sw = File.Create(fullpath))
                    {
                    }
                }

                using (var sw = new StreamWriter(fullpath, true))
                {
                    string[] invoiceArray = new string[]
                    {
                        autonumber(invoicing.AutoNumber),                           // 0            HDR_INVOICE_NUM
                        "STANDARD",                                                 // 1            HDR_INVOICE_TYPE_LOOKUP_CODE
                        invoicing.DateProcessed.Value.ToString("dd-MMM-yyyy"),      // 2            HDR_INVOICE_DATE
                        invoicing.VendorCode,                                       // 3            HDR_VENDOR_NUM
                        "KHO",                                                      // 4            HDR_VENDOR_SITE_CODE
                        invoicing.FinalAmount,                                      // 5            HDR_INVOICE_AMOUNT
                        invoicing.RCRNumber,                                        // 6            HDR_DESCRIPTION
                        invoicing.RCRDate.Value.ToString("dd-MMM-yyyy"),            // 7            HDR_GOODS_RECEIVED_DATE
                        invoicing.SIDate.Value.ToString("dd-MMM-yyyy"),             // 8            HDR_INVOICE_RECEIVED_DATE
                        invoicing.DateProcessed.Value.ToString("dd-MMM-yyyy"),      // 9            HDR_GL_DATE
                        "PO",                                                       // 10           HDR_SOURCE 
                        "1",                                                        // 11           DTL_LINE_NUMBER
                        invoicing.FinalAmount,                                      // 12           DTL_AMOUNT
                        "200",                                                      // 13           DTL_LINE_TYPE_LOOKUP_CODE
                        invoicing.Location,                                         // 14           DTL_DR_COMPANY
                        "",                                                         // 15           DTL_DR_BRANCH
                        "0",                                                        // 16           DTL_DR_BUSINESS_LINE
                        "0",                                                        // 17           DTL_DR_DEPARTMENT
                        "200335011",                                                // 18           DTL_DR_SECTION
                        "200335011",                                                // 19           DTL_DR_MAJOR_ACCOUNT
                        "",                                                         // 20           DTL_DR_MINOR_ACCOUNT
                        invoicing.FinalAmount,                                      // 21           DTL_RCR_AMOUNT
                        invoicing.RCRNumber,                                        // 22           DTL_REFERENCE_NUMBER
                        invoicing.RCRAmount,                                        // 23           DTL_RCR_AMOUNT
                        "",                                                         // 24           DTL_CR_COMPANY
                        "",                                                         // 25           DTL_CR_BRANCH
                        "",                                                         // 26           DTL_CR_BUSINESS_LINE
                        "",                                                         // 27           DTL_CR_DEPARTMENT
                        "",                                                         // 28           DTL_CR_SECTION
                        "",                                                         // 29           DTL_CR_MAJOR_ACCOUNT
                        "",                                                         // 30           DTL_CR_MINOR_ACCOUNT
                        "",                                                         // 31           DTL_CR_AMOUNT
                        invoicing.VatCode,                                          // 32           DTL_VAT_CODE
                        "",                                                         // 33           DTL_ITEM_CODE
                        "",                                                         // 34           DTL_QUANTITY
                        "",                                                         // 35           DTL_AMOUNT
                        "PHP",                                                      // 36           HDR_CURR_CODE
                        invoicing.PaymentTerms,                                     // 37           DUE_DATE/Payment Terms
                        "",                                                         // 38           MISC_SUP_ACTUAL_NAME
                        fileName                                                    // 39           FILENAME
                    };

                    // Join array elements into a single string with "|" as delimiter
                    invoice = string.Join("|", invoiceArray);

                    sw.WriteLine(invoice + "|");
                }
                return (true,fullpath);
            }
            catch (Exception ex)
            {
                _audilogsService.writeLogs(ex.ToString());
                return (false,"");
            }

        }

        public bool CopySendInvoiceFile(string sourceFile, string destinationPath)
        {
            try
            {
                string filename = Path.GetFileName(sourceFile);
                string destinationFullPath = Path.Combine(destinationPath, filename);
                // Copy the file to the destination path
                File.Copy(sourceFile, destinationFullPath, true); // Overwrites if the file exists
                return true;
            }
            catch (Exception ex)
            {
                _audilogsService.writeLogs(ex.ToString());
                return false;
            }
        }
        public string autonumber(string number)
        {
            string formatstring = string.Concat("000000000", number);
            string numberformat = number.Count() < 10 ?  formatstring.Substring(number.Count()) : number;
            string autonumber = string.Format("DMS{0}", numberformat);
            return autonumber;        
        }

    }
}
