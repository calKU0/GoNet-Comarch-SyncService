using cdn_api;
using Dapper;
using GoNet_Comarch_SyncService.Data;
using GoNet_Comarch_SyncService.DTOs;
using GoNet_Comarch_SyncService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Attribute = GoNet_Comarch_SyncService.DTOs.Attribute;

namespace GoNet_Comarch_SyncService.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _db;

        public ClientRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Client>> GetClientsForImport()
        {
            const string proc = "dbo.GaskaGetGoNetClientsForExport";

            using var conn = await _db.GetOpenConnectionAsync();

            var clientDict = new Dictionary<int, Client>();

            var result = await conn.QueryAsync<Client, Address, Attribute, Client>(
                proc,
                (client, address, attribute) =>
                {
                    if (!clientDict.TryGetValue(client.ClientCrmId, out var existingClient))
                    {
                        existingClient = client;
                        existingClient.Address = address;
                        existingClient.Attributes = new List<Attribute>();
                        clientDict.Add(existingClient.ClientCrmId, existingClient);
                    }

                    if (attribute != null)
                        existingClient.Attributes.Add(attribute);

                    return existingClient;
                },
                splitOn: "City,AttributeId",
                commandType: System.Data.CommandType.StoredProcedure
            );

            return clientDict.Values;
        }

        public Task<IEnumerable<ClientBranch>> GetClientBranchesForImport()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Client>> GetClientsForUpdate()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ClientBranch>> GetClientBranchesForUpdate()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateClient(Client client)
        {
            const string proc = "dbo.GaskaUpdateClientFromGoNet";

            using var conn = await _db.GetOpenConnectionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", client.ClientErpId);
            parameters.Add("@Acronym", client.Acronym);
            parameters.Add("@Name", client.Name);
            parameters.Add("@NIP", client.NIP);
            parameters.Add("@Regon", client.Regon);
            parameters.Add("@Description", client.Description);
            parameters.Add("@Email", client.Email);
            parameters.Add("@Phone", client.Phone);
            parameters.Add("@Website", client.Website);
            parameters.Add("@Price", client.Price);

            // Address (nested object)
            parameters.Add("@Street", client.Address.Street);
            parameters.Add("@City", client.Address.City);
            parameters.Add("@PostalCode", client.Address.PostalCode);
            parameters.Add("@Country", client.Address.Country);

            await conn.ExecuteAsync(proc, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateClientBranch(ClientBranch branch)
        {
            const string proc = "dbo.GaskaUpdateClientBranchFromGoNet";

            using var conn = await _db.GetOpenConnectionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", branch.BranchErpId);
            parameters.Add("@ClientId", branch.BranchClientErpId);
            parameters.Add("@Acronym", branch.Acronym);
            parameters.Add("@Name", branch.Name);
            parameters.Add("@NIP", branch.NIP);
            parameters.Add("@Regon", branch.Regon);
            parameters.Add("@Description", branch.Description);
            parameters.Add("@Email", branch.Email);
            parameters.Add("@Phone", branch.Phone);
            parameters.Add("@Website", branch.Website);
            parameters.Add("@Price", branch.Price);

            // Address (nested object)
            parameters.Add("@Street", branch.Address.Street);
            parameters.Add("@City", branch.Address.City);
            parameters.Add("@PostalCode", branch.Address.PostalCode);
            parameters.Add("@Country", branch.Address.Country);

            await conn.ExecuteAsync(proc, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAttributes(int objectId, int objectType, List<Attribute> attributes)
        {
            const string proc = "dbo.GaskaUpdateClientAttributesFromGoNet";

            using var conn = await _db.GetOpenConnectionAsync();

            foreach (var attr in attributes)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ObjectId", objectId);
                parameters.Add("@ObjectType", objectType);
                parameters.Add("@AttributeKey", attr.ClassName);
                parameters.Add("@AttributeValue", attr.Value);

                await conn.ExecuteAsync(proc, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}