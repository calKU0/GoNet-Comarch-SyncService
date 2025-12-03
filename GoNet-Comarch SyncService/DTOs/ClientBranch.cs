using System;
using System.Collections.Generic;
using System.Text;

namespace GoNet_Comarch_SyncService.DTOs
{
    public class ClientBranch
    {
        public int BranchCrmId { get; set; }
        public int BranchErpId { get; set; }
        public int BranchErpType { get; set; }
        public int BranchClientErpId { get; set; }
        public string Acronym { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string NIP { get; set; } = default!;
        public string Regon { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone1 { get; set; } = string.Empty;
        public string Phone2 { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Price { get; set; } = default!;
        public Address Address { get; set; } = default!;
        public List<Attribute> Attributes { get; set; } = default!;
    }
}