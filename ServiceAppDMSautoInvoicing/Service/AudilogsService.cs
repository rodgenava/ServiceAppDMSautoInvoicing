using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppDMSautoInvoicing
{
    public class AudilogsService : IAudilogsService
    {
        private readonly IConfiguration _configuration;
        public AudilogsService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async void writeLogs(string logsdata = "")
        {
            string fileName = "Auditlogs.txt";
            string path = _configuration.GetSection("Directory:logs").Value;  // UNC path to the CSV file 
            string fullpath = Path.Combine(path, fileName);

            Directory.CreateDirectory(path);

            if (!File.Exists(fullpath))
            {
                using (var sw = File.Create(fullpath))
                {
                }
            }

            using (var sw = new StreamWriter(fullpath, true))
            {
                DateTime localDate = DateTime.Now;
                sw.WriteLine(String.Format("{0} {1}", logsdata, localDate));
                sw.WriteLine("______________________________________________________________________________________________________");
            }
        }
    }
}
