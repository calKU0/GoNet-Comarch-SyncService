using System;
using System.Collections.Generic;
using System.Text;

namespace GoNet_Comarch_SyncService.DTOs
{
    public class Attribute
    {
        public int AttributeId { get; set; }
        public string ClassName { get; set; } = default!;
        public string Value { get; set; } = default!;
    }
}