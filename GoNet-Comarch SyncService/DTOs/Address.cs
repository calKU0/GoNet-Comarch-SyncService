using System;
using System.Collections.Generic;
using System.Text;

namespace GoNet_Comarch_SyncService.DTOs
{
    public class Address
    {
        public string City { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string HomeNo { get; set; } = string.Empty;
        public string PostalCode { get; set; } = default!;
        public string Country { get; set; } = default!;
    }
}