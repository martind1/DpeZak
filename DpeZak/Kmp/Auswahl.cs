namespace DpeZak.Kmp
{
    public class Asws
    {
        public string Param { get; set; }
        public string Value { get; set; }
        //für Ceckbox:
        //public bool? this[string index]
        //{
        //    get { return index == "J" ? true : index == "N" ? false : null; }
        //    set { }
        //}
    }


    /// <summary>
    /// Feste Auswahlen Param=Gespeicherter Wert, Value=angezeigter Wert
    /// </summary>
    public static class Auswahl
    {

        public static readonly IEnumerable<Asws> aswOK = new Asws[] {
            new Asws(){ Param = "J", Value = "OK" },
            new Asws(){ Param = "N", Value = "Nicht OK" } };

        public static readonly IEnumerable<Asws> aswOKStrich = new Asws[] {
            new Asws(){ Param = "J", Value = "OK" },
            new Asws(){ Param = "N", Value = "-" } };

        public static readonly IEnumerable<Asws> aswLityp = new Asws[] {
            new Asws(){ Param = "A", Value = "Entsorgung" },
            new Asws(){ Param = "B", Value = "Abgänge" },
            new Asws(){ Param = "W", Value = "Intern" } };

        public static readonly IEnumerable<Asws> aswHtmlSingle = new Asws[] {
            new Asws(){ Param = "VFUE", Value = "Verfüllabschnitt" },
            new Asws(){ Param = "DKAT", Value = "Deponiekataster" },
            new Asws(){ Param = "PROB", Value = "Probenahme" },
            new Asws(){ Param = "FAHR", Value = "Fahrzeug" },
            new Asws(){ Param = "EDTM", Value = "Eingang Zeit" },
            new Asws(){ Param = "VONR", Value = "Beleg Nr." },
            new Asws(){ Param = "LORT", Value = "Lager" },
            new Asws(){ Param = "KATA", Value = "Kataster" },
            new Asws(){ Param = "CHAR", Value = "Kompost Chargennr." },
            new Asws(){ Param = "MKEN", Value = "Materialkennung" },
            new Asws(){ Param = "SRTE", Value = "Sorte" },
            new Asws(){ Param = "AVV", Value = "AVV" },
            new Asws(){ Param = "ENTS", Value = "Nachweis" },
            new Asws(){ Param = "BEF", Value = "Beförderer" },
            new Asws(){ Param = "ANF", Value = "Anfallstelle" },
        };
    }
}
