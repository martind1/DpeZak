using System;
using System.Collections.Generic;

namespace DpeZak.Entities.DPE
{
    public partial class R_INIT
    {
        public string ANWENDUNG { get; set; }
        public string TYP { get; set; }
        public string NAME { get; set; }
        public string SECTION { get; set; }
        public string PARAM { get; set; }
        public string WERT { get; set; }
        public int INIT_ID { get; set; }
        public string ERFASST_VON { get; set; }
        public DateTime? ERFASST_AM { get; set; }
        public string ERFASST_DATENBANK { get; set; }
        public int? ANZAHL_AENDERUNGEN { get; set; }
        public DateTime? GEAENDERT_AM { get; set; }
        public string GEAENDERT_VON { get; set; }
        public string GEAENDERT_DATENBANK { get; set; }
        public string BEMERKUNG { get; set; }
    }
}
