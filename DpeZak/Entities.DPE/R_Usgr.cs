using System;
using System.Collections.Generic;

namespace DpeZak.Entities.DPE
{
    public partial class R_Usgr
    {
        public double? USGR_ID { get; set; }
        public double? USGR_USER_ID { get; set; }
        public double? USGR_GRUP_ID { get; set; }
        public string SO_USER_KENNUNG { get; set; }
        public string SO_GRUP_NAME { get; set; }
        public string ERFASST_VON { get; set; }
        public DateTime? ERFASST_AM { get; set; }
        public string GEAENDERT_VON { get; set; }
        public DateTime? GEAENDERT_AM { get; set; }
        public string BEMERKUNG { get; set; }
    }
}
