using System;
using System.Collections.Generic;

namespace DpeZak.Entities.DPE
{
    public partial class R_Grup
    {
        public double? GRUP_ID { get; set; }
        public string GRUP_NAME { get; set; }
        public string GRUP_INFORMATION { get; set; }
        public string ERFASST_VON { get; set; }
        public DateTime? ERFASST_AM { get; set; }
        public string GEAENDERT_VON { get; set; }
        public DateTime? GEAENDERT_AM { get; set; }
        public string BEMERKUNG { get; set; }
    }
}
