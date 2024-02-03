using System;
using System.Collections.Generic;

namespace DpeZak.Entities.DPE
{
    public partial class R_Usrs
    {
        public double? USER_ID { get; set; }
        public string USER_KENNUNG { get; set; }
        public string USER_LANGNAME { get; set; }
        public string USER_TELEFON_NR { get; set; }
        public string USER_INFORMATION { get; set; }
        public string USER_PASSWORT { get; set; }
        public string ERFASST_VON { get; set; }
        public string USER_LEVEL { get; set; }
        public DateTime? ERFASST_AM { get; set; }
        public string GEAENDERT_VON { get; set; }
        public DateTime? GEAENDERT_AM { get; set; }
        public string BEMERKUNG { get; set; }
        public string FLAG_PASSWORT { get; set; }
    }
}
