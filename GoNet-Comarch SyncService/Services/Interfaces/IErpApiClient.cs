using GoNet_Comarch_SyncService.DTOs;

namespace GoNet_Comarch_SyncService.Services.Interfaces
{
    public interface IErpApiClient
    {
        public int Login();

        public int Logout(int sessionId);

        public int CreateClient(int sessionId, Client client);

        public int CreateClientBranch(int sessionId, ClientBranch branch);
    }
}