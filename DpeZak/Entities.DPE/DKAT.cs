using System;
using System.Collections.Generic;

namespace DpeZak.Entities.DPE
{
    public partial class DKAT
    {
        public string DKAT_NR { get; set; }
        public string DKAT_BEZ { get; set; }
        public DateTime? DKAT_DTMVON { get; set; }
        public DateTime? DKAT_DTMBIS { get; set; }
        public string ERFASST_VON { get; set; }
        public DateTime? ERFASST_AM { get; set; }
        public string GEAENDERT_VON { get; set; }
        public DateTime? GEAENDERT_AM { get; set; }
        public int? ANZAHL_AENDERUNGEN { get; set; }
        public string BEMERKUNG { get; set; }
    }
}
