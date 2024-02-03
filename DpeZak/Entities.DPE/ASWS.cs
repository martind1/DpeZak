using System;
using System.Collections.Generic;

namespace DpeZak.Entities.DPE
{
    public partial class ASWS
    {
        public string ASW_NAME { get; set; }
        public int ITEM_POS { get; set; }
        public string ITEM_VALUE { get; set; }
        public string ITEM_DISPLAY { get; set; }
        public string ERFASST_VON { get; set; }
        public DateTime? ERFASST_AM { get; set; }
        public string GEAENDERT_VON { get; set; }
        public DateTime? GEAENDERT_AM { get; set; }
        public int? ANZAHL_AENDERUNGEN { get; set; }
        public string BEMERKUNG { get; set; }
    }
}
