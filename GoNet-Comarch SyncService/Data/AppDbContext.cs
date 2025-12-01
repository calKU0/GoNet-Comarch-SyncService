using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace GoNet_Comarch_SyncService.Data
{
    public class AppDbContext
    {
        private readonly string _connectionString;
        private readonly ILogger<AppDbContext> _logger;

        public AppDbContext(IConfiguration configuration, ILogger<AppDbContext> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnectionString")
                ?? throw new InvalidOperationException("Missing DefaultConnectionString in configuration.");
            _logger = logger;
        }

        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
}