using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppDMSautoInvoicing
{
    public class DMSQueryService : IDMSQueryService
    {
        private readonly IConfiguration _configuration;
        private readonly IAudilogsService _audilogsService;
        public DMSQueryService(IConfiguration configuration, IAudilogsService audilogsService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _audilogsService = audilogsService;
        }

        public async Task<Invoicing> checkDBforInvoicing()
        {
            try
            {
                Invoicing invoicing = new Invoicing();
                string connectionString = _configuration.GetSection("ConnectionStrings:DMS").Value;

                string selectquery = "select * " +
                                          //",PONumber" +
                                          //",VendorCode" +
                                          //",VendorName" +
                                          //",RCRDate" +
                                          //",RCRNumber" +
                                          //",RCRAmount" +
                                          //",AdjustedRCRAmount" +
                                          //",SIAmount" +
                                          //",PORADate" +
                                          //",PORAAmount" +
                                          //",FinalAmount" +
                                          //",RCRStatus" +
                                          //",DateProcessed" +
                                          //",SINumber " +
                                      //"from QF_RCR where LOWER(RCRStatus) = 'on process' and isnull(IsInvoiced, 0) = 0";
                                      "from QF_RCR where LOWER(RCRStatus) = 'processed' and isnull(IsInvoiced, 0) = 0";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Create a SqlCommand
                    using (SqlCommand command = new SqlCommand(selectquery, connection))
                    {
                        // Execute the query and get a SqlDataReader
                        using (SqlDataReader sreader = command.ExecuteReader())
                        {
                            // Loop through the rows
                            while (sreader.Read())
                            {
                                invoicing.Location = sreader["Location"].ToString();
                                invoicing.PONumber = sreader["PONumber"].ToString();
                                invoicing.VendorCode = sreader["VendorCode"].ToString();
                                invoicing.VendorName = sreader["VendorName"].ToString();
                                invoicing.RCRDate = DateTime.Parse(sreader["RCRDate"].ToString());
                                invoicing.RCRNumber = sreader["RCRNumber"].ToString();
                                invoicing.RCRAmount = sreader["RCRAmount"].ToString();
                                invoicing.AdjustedRCRAmount = sreader["AdjustedRCRAmount"].ToString();
                                invoicing.SIAmount = sreader["SIAmount"].ToString();
                                invoicing.PORADate = DateTime.Parse(sreader["PORADate"].ToString());
                                invoicing.PORAAmount = sreader["PORAAmount"].ToString();
                                invoicing.FinalAmount = sreader["FinalAmount"].ToString();
                                invoicing.RCRStatus = sreader["RCRStatus"].ToString();
                                invoicing.DateProcessed = sreader["DateProcessed"].ToString() == "" ? DateTime.Now : DateTime.Parse(sreader["DateProcessed"].ToString());
                                invoicing.SINumber = sreader["SINumber"].ToString();

                                invoicing.SIDate = DateTime.Parse(sreader["SIDate"].ToString());
                                invoicing.VatCode = sreader["VatCode"].ToString();
                                invoicing.PaymentTerms = sreader["PaymentTerms"].ToString();
                                invoicing.AutoNumber = sreader["AutoNumber"].ToString();
                            }
                        }
                    }

                }
                return invoicing;
            }
            catch (Exception ex)
            {
                _audilogsService.writeLogs(ex.ToString());
                return null;
            }
        }

        public async Task<bool> UpdateDBinvoicing(string RCRNumber = "")
        {
            try
            {
                string connectionString = _configuration.GetSection("ConnectionStrings:DMS").Value;

                string queryUpdate = "UPDATE QF_RCR SET [IsInvoiced] = @IsInvoiced WHERE RCRNumber = @RCRNumber";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Create a SqlCommand
                    using (SqlCommand cmd = new SqlCommand(queryUpdate, connection))
                    {
                        cmd.Parameters.AddWithValue("@IsInvoiced", true);
                        cmd.Parameters.AddWithValue("@RCRNumber", RCRNumber);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //update is successfull
                            return true;
                        }
                        else
                        {
                            //update is failed
                            return false;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _audilogsService.writeLogs(ex.ToString());
                return false;
            }
        }
    }
}
