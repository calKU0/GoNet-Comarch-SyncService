using GoNet_Comarch_SyncService.DTOs;
using GoNet_Comarch_SyncService.Repositories.Interfaces;
using GoNet_Comarch_SyncService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoNet_Comarch_SyncService.Services
{
    public class ClientImportService : IClientImportService
    {
        private readonly ILogger<ClientImportService> _logger;
        private readonly IClientRepository _clientRepo;
        private readonly IErpApiClient _erpApiClient;

        public ClientImportService(ILogger<ClientImportService> logger, IClientRepository clientRepo, IErpApiClient erpApiClient)
        {
            _logger = logger;
            _clientRepo = clientRepo;
            _erpApiClient = erpApiClient;
        }

        public async Task ImportClients()
        {
            try
            {
                int apiSessionId = _erpApiClient.Login();
                var clients = await _clientRepo.GetClientsForImport();

                foreach (var client in clients)
                {
                    try
                    {
                        _erpApiClient.CreateClient(apiSessionId, client);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error creating client {client.Acronym} with CRM ID {client.ClientCrmId}");
                    }
                }

                _erpApiClient.Logout(apiSessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during client import");
            }
        }

        public async Task ImportClientBranches()
        {
            try
            {
                int apiSessionId = _erpApiClient.Login();
                var branches = await _clientRepo.GetClientBranchesForImport();

                foreach (var branch in branches)
                {
                    try
                    {
                        _erpApiClient.CreateClientBranch(apiSessionId, branch);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error creating client branch {branch.Acronym} with CRM ID {branch.BranchCrmId}");
                    }
                }

                _erpApiClient.Logout(apiSessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during client branches import");
            }
        }

        public async Task UpdateClients()
        {
            try
            {
                var clients = await _clientRepo.GetClientsForUpdate();

                foreach (var client in clients)
                {
                    try
                    {
                        await _clientRepo.UpdateClient(client);
                        await _clientRepo.UpdateAttributes(client.ClientErpId, client.ClientErpType, client.Attributes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error updating client {client.Acronym} with ERP ID {client.ClientErpId} with CRM ID {client.ClientCrmId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during client update");
            }
        }

        public async Task UpdateClientBranches()
        {
            try
            {
                var branches = await _clientRepo.GetClientBranchesForUpdate();

                foreach (var branch in branches)
                {
                    try
                    {
                        await _clientRepo.UpdateClientBranch(branch);
                        await _clientRepo.UpdateAttributes(branch.BranchErpId, branch.BranchErpType, branch.Attributes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error updating client branch {branch.Acronym} with ERP ID {branch.BranchErpId} with CRM ID {branch.BranchCrmId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during client branches import");
            }
        }
    }
}