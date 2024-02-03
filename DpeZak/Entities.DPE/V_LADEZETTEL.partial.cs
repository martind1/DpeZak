using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DpeZak.Kmp;

namespace DpeZak.Entities.DPE
{
    public partial class V_LADEZETTEL
    {
        //Liste der FormatString pro Feld:
        //Groß/Klein egal (wird angepasst)
        //Format wird umgewandelt F -> {0:F}
        [NotMapped]
        public IDictionary<string, string> Formatlist { get; private set; } = new Dictionary<string, string>()
        {
            {"TAGEW", "r,0.00" },
            {"BRGEW", "r,0.00" },
            {"EDT", "d" }
        };

        //berechnete Eigenschaften
        [NotMapped]
        public bool? cfHOFL_OK
        {
            get => HOFL_OK == "J" ? true : HOFL_OK == "N" ? false : null;
            //set { HOFL_OK = value == null ? null : value == true ? "J" : "N"; } }
            set { }
        }

        [NotMapped]
        public string cfVFUE_NR
        {
            get => VFUE_NR?.ToString("D3") ?? "";  //Int mit führenden 0en 3stellig
            set { if (short.TryParse(value, out short I1)) VFUE_NR = I1; }
        }

        [NotMapped]
        public string cfLITYP
        {
            //Realsiert Auswahlfeld
            get => Auswahl.aswLityp.Where(a => a.Param == lityp).FirstOrDefault()?.Value;
            set { lityp = Auswahl.aswLityp.Where(a => a.Value == value).FirstOrDefault()?.Param; }
        }
    }
}
