using System;
using System.Collections.Generic;
using System.Text;

namespace GoNet_Comarch_SyncService.Settings
{
    public class ErpApiSettings
    {
        public int ApiVersion { get; set; } = default!;
        public string ProgramName { get; set; } = default!;
        public string Database { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}