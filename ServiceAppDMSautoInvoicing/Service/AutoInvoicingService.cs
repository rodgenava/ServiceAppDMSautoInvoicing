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
                    invoice = String.Format("{0}|STANDARD|{1}|{2}|KHO|{3}|{4}|{5}|{6}|{7}|PO|{8}|{9}|ITEM|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|||||||||{20}||||PHP|{21}||{22}|",

                        autonumber(invoicing.AutoNumber),   //0
                        invoicing.DateProcessed,            //1
                        invoicing.VendorCode,               //2
                        invoicing.FinalAmount,              //3
                        invoicing.RCRNumber,                //4
                        invoicing.RCRDate,                  //5
                        invoicing.SIDate,                   //6
                        invoicing.DateProcessed,            //7
                        "",                                 //8
                        invoicing.FinalAmount,              //9
                        "200",                              //10
                        invoicing.Location,                 //11
                        "",                                 //12
                        "0",                                //13
                        "0",                                //14
                        "200335011",                        //15
                        "200335011",                        //16
                        invoicing.FinalAmount,              //17
                        invoicing.RCRNumber,                //18
                        invoicing.RCRAmount,                //19
                        invoicing.VatCode,                  //20
                        invoicing.PaymentTerms,             //21
                        fileName);                          //22

                    sw.WriteLine(invoice);
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
