using System;
using System.Collections.Generic;
using System.Text;

namespace GoNet_Comarch_SyncService.Services.Interfaces
{
    public interface IClientImportService
    {
        public Task ImportClients();

        public Task ImportClientBranches();

        public Task UpdateClients();

        public Task UpdateClientBranches();
    }
}