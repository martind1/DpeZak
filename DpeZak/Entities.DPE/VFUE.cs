using System;
using System.Collections.Generic;

namespace DpeZak.Entities.DPE
{
    public partial class VFUE
    {
        public short VFUE_NR { get; set; }
        public string VFUE_BEZ { get; set; }
        public DateTime? GUELTIG_AB { get; set; }
        public DateTime? GUELTIG_BIS { get; set; }
        public string ERFASST_VON { get; set; }
        public DateTime? ERFASST_AM { get; set; }
        public string GEAENDERT_VON { get; set; }
        public DateTime? GEAENDERT_AM { get; set; }
        public int? ANZAHL_AENDERUNGEN { get; set; }
        public string BEMERKUNG { get; set; }
    }
}
