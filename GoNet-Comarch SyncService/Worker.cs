using GoNet_Comarch_SyncService.Services.Interfaces;
using GoNet_Comarch_SyncService.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Runtime;

namespace GoNet_Comarch_SyncService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppSettings _appSettings;
        private readonly IClientImportService _clientService;

        public Worker(ILogger<Worker> logger, IOptions<AppSettings> options, IClientImportService clientService)
        {
            _logger = logger;
            _appSettings = options.Value;
            _clientService = clientService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service started");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Starting client import...");
                    await _clientService.ImportClients();
                    await _clientService.ImportClientBranches();
                    await _clientService.UpdateClients();
                    await _clientService.UpdateClientBranches();
                    _logger.LogInformation("Client import completed. Waiting for next cycle...");
                    await Task.Delay(TimeSpan.FromMinutes(_appSettings.SynchroInterval), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Service stopping...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during service execution.");
            }
            finally
            {
                _logger.LogInformation("Service stopped.");
            }
        }
    }
}