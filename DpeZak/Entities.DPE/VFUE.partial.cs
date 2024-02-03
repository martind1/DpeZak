using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DpeZak.Kmp;

namespace DpeZak.Entities.DPE
{
    public partial class VFUE
    {
        //Liste der FormatString pro Feld:
        //Format wird später umgewandelt F -> {0:F}
        //d -> kurzes Datumformat ohne Zeit
        [NotMapped]
        public IDictionary<string, string> Formatlist { get; private set; } = new Dictionary<string, string>()
        {
            {"GUELTIG_BIS", "d" },
            {"GUELTIG_AB", "d" }
        };

        //berechnete Eigenschaften
        [NotMapped]
        public string cfVFUE_NR
        {
            get => VFUE_NR.ToString("D3") ?? "";
            set { if (short.TryParse(value, out short I1)) VFUE_NR = I1; }
        }
    }
}
