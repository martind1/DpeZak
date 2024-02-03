using System;
using System.Collections.Generic;

namespace DpeZak.Entities.DPE
{
    public partial class FLTR
    {
        public string FORM { get; set; }
        public string NAME { get; set; }
        public string FLTRLIST { get; set; }
        public string KEYFIELDS { get; set; }
        public string ISPUBLIC { get; set; }
        public int FLTR_ID { get; set; }
        public string ERFASST_VON { get; set; }
        public DateTime? ERFASST_AM { get; set; }
        public string ERFASST_DATENBANK { get; set; }
        public string GEAENDERT_VON { get; set; }
        public DateTime? GEAENDERT_AM { get; set; }
        public string GEAENDERT_DATENBANK { get; set; }
        public int? ANZAHL_AENDERUNGEN { get; set; }
        public string BEMERKUNG { get; set; }
        public string COLUMNLIST { get; set; }
    }
}
