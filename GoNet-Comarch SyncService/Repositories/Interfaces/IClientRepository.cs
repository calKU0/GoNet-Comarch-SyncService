using GoNet_Comarch_SyncService.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using Attribute = GoNet_Comarch_SyncService.DTOs.Attribute;

namespace GoNet_Comarch_SyncService.Repositories.Interfaces
{
    public interface IClientRepository
    {
        public Task<IEnumerable<Client>> GetClientsForImport();

        public Task<IEnumerable<ClientBranch>> GetClientBranchesForImport();

        public Task<IEnumerable<Client>> GetClientsForUpdate();

        public Task<IEnumerable<ClientBranch>> GetClientBranchesForUpdate();

        public Task UpdateClient(Client client);

        public Task UpdateClientBranch(ClientBranch client);

        public Task UpdateAttributes(int objectId, int objectType, List<Attribute> attributes);
    }
}