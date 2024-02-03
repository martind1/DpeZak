using NuGet.Protocol;
using Radzen;
using Serilog;
using System.Security.Policy;

namespace DpeZak.Kmp
{
    // Datenstrukturen für LNav, GNav:
    // Columnlist: Metadaten für Grid Column
    // FltrList: Metadaten für Suchkriterien / .where
    // SqlFieldList: Metadaten für Feldliste / .insert

    #region ColumnList
    //ColumnList: Verbindung zur KMP Welt. Für Grid Columns
    //15.09.22 md  Columns als IList ausgeprägt um nach ColIndex zu sortieren
    //20.10.22 md  Array-Property ergibt ColumnListItem
    public class ColumnList
    {
        public IList<ColumnListItem> Columns { get; set; }

        // Zugriff per lnav.Columnlist[<Feldname]
        public ColumnListItem this[string index]
        {
            get { return Columns.Where(c => c.Fieldname == index).FirstOrDefault(); }
        }

        public IList<ColumnListItem> SortedColumns
        {
            get
            {
                var sc = from col in Columns orderby col.ColIndex select col;
                return sc.ToList();  //ergibt eine sortierte Kopie von Columns
            }
        }

        private int ColCounter = 0;
       
        public ColumnList()
        {
            Columns = new List<ColumnListItem>();
        }

        public ColumnList(string columnlist) : this()
        {
            //fltrlist Zeile idF <ColDesc>=<FieldName>
            List<string> list = new(
                columnlist.Split(new string[] { "\r\n", "\n", "\r" },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            foreach (var s in list)
            {
                if (s.StartsWith(";"))
                    continue;
                AddColumn(s);
            }
        }

        public void AddColumn(string desc)
        {
            ColCounter++;
            var col = new ColumnListItem(desc)
            {
                ColIndex = ColCounter
            };
            Columns.Add(col);
        }
    }


    [Flags]
    public enum ColumnListItemFlags
    {
        Speicherwert = 1, //S=Auswahlfeld: Speicherwert (statt Anzeigewert) anzeigen
        Summe = 2,        //M=Spalte summieren
        OptiBreite = 4,   //O=Optimale Breite 
        MaxBreite = 8,    //X=Maximale Breite (in letzter Spalte)
        Hilfetext = 16,   //H=vollständigen Text in Statuszeile anzeigen

    }

    // Beschreibung einer Spalte
    // 15.09.22 md  Fieldname, ColIndex ergänzt
    public class ColumnListItem
    {
        public int ColIndex { get; set; }  //Spaltenreihenfolge
        public string Fieldname { get; set; }
        public string DisplayLabel { get; set; }
        public int DisplayWidth { get; set; } = 0;
        public string WidthPx { get { int dw8 = DisplayWidth * 8 + 16; return $"{dw8}px"; } }
        public bool IsVisible
        {
            get => DisplayWidth > 0;
            set
            {
                if (value)
                {
                    if (DisplayWidth == 0)
                        DisplayWidth = 8;
                }
                else
                {
                    if (DisplayWidth > 0)
                        DisplayWidth = 0;
                }
            }
        }
        public ColumnListItemFlags Flags { get; set; }

        //wird nachträglich ausgefüllt: (asc, desc, ""/null) für razor
        public Radzen.SortOrder? SortOrder { get; set; }
        //wird anhand FieldType und Formatlist ausgefüllt
        public string FormatString;  //zB "{0:d}"
        public TextAlign RzTextAlign;
        public string SingleStyle;  //für Textbox, Numeric


        //Spalte anhand KMP Beschreibung anlegen idF <Display>:<Width>,<[Option]*>=<Fieldname>
        public ColumnListItem(string ColDesc)
        {
            var SL0 = ColDesc.Split('=', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (SL0.Length >= 2)
            {
                Fieldname = SL0[1];
                DisplayLabel = Fieldname;
                DisplayWidth = 0;
                var SL1 = SL0[0].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (SL1.Length >= 1)
                {
                    var SL2 = SL1[0].Split(':');
                    DisplayLabel = SL2[0];
                    if (SL2.Length >= 2)
                        DisplayWidth = int.Parse(SL2[1]);
                    foreach (var option in SL1[1..])
                    {
                        ColumnListItemFlags flags = option switch
                        {
                            "S" => ColumnListItemFlags.Speicherwert,
                            "M" => ColumnListItemFlags.Summe,
                            "O" => ColumnListItemFlags.OptiBreite,
                            "X" => ColumnListItemFlags.MaxBreite,
                            "H" => ColumnListItemFlags.Hilfetext,
                            _ => throw new NotImplementedException()
                        };
                        Flags |= flags;
                    }
                }
            }
        }
    }
    #endregion

#region FltrList und SQL generieren (Where Clause)
    //FltrList: Verbindung zur KMP Welt. Für SQL Where Clause
    public partial class FltrList
    {
        public IList<FltrListItem> Fltrs { get; set; }

        public FltrList()
        {
            Fltrs = new List<FltrListItem>();
            SqlParams = new Dictionary<int, object>();
        }

        public FltrList(string fltrlist) : this()
        {
            //fltrlist Zeile idF <FieldName>=<Suchkriterien>
            if (fltrlist != null)
            {
                List<string> list = new(
                    fltrlist.Split(new string[] { "\r\n", "\n", "\r" },
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
                foreach (var s in list)
                {
                    if (!s.StartsWith(";"))
                    {
                        var SL = s.Split2("=");  //beware ANL_NA1=AGH%;=
                        if (SL.Length >= 2 && !String.IsNullOrEmpty(SL[1]))
                        {
                            var fli = new FltrListItem(SL[0], SL[1]);
                            Fltrs.Add(fli);
                        }
                    }
                }
            }
        }
    }


    // Beschreibung Suchkriterien eines Felds
    // KmpStr (von Kmp.Fltrlist.Zeile): a >a <a == >= a;b;c [raw Feld-Argument] {raw Zeile ohne Feld}
    // Where (für Linq): <FldName> = "a" oder <FldName> > b
    // OrFlag (Kmp beginnt mit ';'): true = Verknüpfung mit 'or' mit anderen Feldern (und entspr Klammerung)
    public partial class FltrListItem
    {
        public string KmpStr { get; set; }
        public string Fieldname { get; set; }

        public string SqlWhere { get; set; }

        //Item anhand KMP Beschreibung anlegen. Noch kein Where/SQL
        public FltrListItem(string fieldname, string kmpstr)
        {
            Fieldname = fieldname;
            KmpStr = kmpstr;
        }
    }
    #endregion

    #region Formatlist

    // Aufbau: <Fieldname>=<Format> # Format = [r,][R,]
    // todo:machen

    #endregion


}
