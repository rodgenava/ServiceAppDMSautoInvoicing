namespace ServiceAppDMSautoInvoicing
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAutoInvoicingService _autoInvoicingService;
        

        public Worker(ILogger<Worker> logger, IAutoInvoicingService autoInvoicingService)
        {
            _logger = logger;
            _autoInvoicingService = autoInvoicingService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _autoInvoicingService.createAutoInvoicing();
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
