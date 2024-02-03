using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using NuGet.Protocol;
using Radzen;
using Radzen.Blazor;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Security.Policy;
using System.Text.Json;
using DpeZak.Data;
using DpeZak.Entities.DPE;
using static DpeZak.Kmp.DposUtils;

namespace DpeZak.Kmp
{

    //Lookup Definition
    public class LookupDef<TItem>: NavigatorLink<TItem>
        where TItem : class
    {
        public ColumnList Columnlist { get; set; }
        public string KeyFields { get; set; }
        public string OrderBy { get; set; }  //von Radzen.LoadDataArgs
        public FltrList Fltrlist { get; set; }
        public FltrList References { get; set; }  //nicht durch User änderbar

        //Liste mit Original Feldnamen (Groß/Kleinschreibung). Lazy Loading.
        private IDictionary<string, FieldInfo> _entityFieldlist;
        public IDictionary<string, FieldInfo> EntityFieldlist
        {
            get => _entityFieldlist ??= GetFieldlist(typeof(TItem));
            set => _entityFieldlist = value;
        }

        protected DpeData data;
        private readonly GlobalService gnav;
        private readonly ProtService prot;

        public FLTR FltrRec { get; set; }
        public string Abfrage { get; set; }

        //LuDef:
        public INavigatorLink lnav;  //Mastersource
        public string FormKurz { get; set; }  // Lu:Fremder Pagename


        public LookupDef()
        {
            //siehe dort - EntityFieldlist = DposUtils.GetFieldlist(typeof(TItem));
            owner = nlOwner.ownLuDef;
        }

        public LookupDef(GlobalService gnav, DpeData data, ProtService prot, string formKurz, string abfrage): this()
        {
            this.gnav = gnav;  //muss sein wg Session
            this.data = data;  //muss sein wg Session
            this.prot = prot;  //muss sein wg Session

            FormKurz = formKurz;
            Abfrage = abfrage;
            LoadAbfrage();  //Columnlist usw
        }


        #region ColumnList, KeyFields, Fltrlist, References  

        // Record mit Columnlist, Keyfields, Fltrlist von Table FLTR[FormKurz, Abfrage] laden
        public void LoadAbfrage()
        {
            FltrRec = data.GetFltr(FormKurz, Abfrage);  //null wenn nicht vorhanden ist auch gut
            Columnlist = LoadColumnlist();
            KeyFields = LoadKeyFields();  //erst hier wg ergänzt Columnlist
            Fltrlist = LoadFltrlist();
        }

        public ColumnList LoadColumnlist()
        {
            //Test: statische Liste
            //todo: von Abfrage laden
            //string cl = 
            //@"Quittung:5=HOFL_KTRL
            //sta:0=sta
            //edt:0=edt
            //Lieferart:10=lityp
            //Ein:5=ETm
            //Fahrzeug:11=fahr_knz
            //Beförderer:23=anl_na1
            //Sorte Bez.:21=srte_bez
            //Tara:8=tagew
            //erz_na1:13=erz_na1
            //erz_na2:14=erz_na2
            //erz_str:15=erz_str";
            //return new ColumnList(cl);

            ColumnList columnlist = (FltrRec == null) ? new ColumnList() : FltrRec.cfColumnlist;  //von DB
            int width = (FltrRec == null) ? 8 : 0;

            //Groß/Klein korrigieren:
            foreach (var col in columnlist.Columns)
            {
                col.Fieldname = DposUtils.AdjustFieldname(col.Fieldname, EntityFieldlist.Keys.ToList<string>());
            }

            //fehlende Entity Felder als invisible ergänzen:
            //Wenn keine Abfrage/FltrRec dann Standardbereite (width)
            foreach (var field in EntityFieldlist.Keys.ToList<string>())
            {
                var col = columnlist.Columns.Where(x => x.Fieldname == field).FirstOrDefault();
                if (col == null)
                {
                    columnlist.AddColumn($"{field}:{width}={field}");
                }
            }

            //Format bestimmen:
            foreach (var col in columnlist.Columns)
            {
                if (EntityFieldlist.TryGetValue(col.Fieldname, out var fieldInfo))
                {
                    col.FormatString = fieldInfo.Formatstring;
                    col.RzTextAlign = fieldInfo.Options switch
                    {
                        FormatOptions.alRight => TextAlign.Right,
                        FormatOptions.alCenter => TextAlign.Center,
                        _ => TextAlign.Left,
                    };
                    col.SingleStyle = fieldInfo.Options switch
                    {
                        FormatOptions.alRight => "text-align: right",
                        FormatOptions.alCenter => "text-align: center",
                        _ => "text-align: left",
                    };
                }
            }
            return columnlist;
        }


        //KeyFields von Abfrage laden und nach Columnlist.Sortorder übertragenb
        //OrderBy setzen
        public string LoadKeyFields()
        {
            //von Abfrage laden
            //Groß/Kleinschreibung prüfen, anpassen oder Fehler wenn nicht gefunden
            //Bsp  "edt;ETm desc"
            string kf = (FltrRec == null) ? "" : FltrRec.KEYFIELDS;

            string[] keyfields = kf.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            IDictionary<string, string> fields = new Dictionary<string, string>();

            foreach (var keyfield in keyfields)
            {
                var key = keyfield.Split(' ');
                key[0] = DposUtils.AdjustFieldname(key[0], EntityFieldlist.Keys.ToList<string>());  //Groß/Klein korrigieren
                fields.Add(key[0], key.Length >= 2 ? key[1] : "asc");
            }

            //nach Columnlist.Sortorder übertragen: 
            foreach (var col in Columnlist.Columns)
            {
                if (fields.TryGetValue(col.Fieldname, out string keyvalue))
                    col.SortOrder = keyvalue.ToLower() == "asc" ? Radzen.SortOrder.Ascending : Radzen.SortOrder.Descending;
                else
                    col.SortOrder = null;
            }

            //OrderBy setzen (Keyfields in Linq Syntax):
            var order = new List<string>();
            foreach (var col in Columnlist.Columns.Where(c => c.SortOrder != null))
            {
                if (col.SortOrder == SortOrder.Descending)
                    order.Add($"{col.Fieldname} {SortOrder.Descending}");
                else
                    order.Add($"{col.Fieldname}");
            }
            OrderBy = string.Join(",", order);

            return kf; //Original KMP Keyfields
        }


        public FltrList LoadFltrlist()
        {
            //von Abfrage laden: LookUp FLTR[formKurz, Abfrage].FltrList
            //Bsp  "lityp=B;A\r\nlort_nr=57";
            FltrList fltrlist = (FltrRec == null) ? new FltrList() : FltrRec.cfFltrlist;
            //Groß/Klein korrigieren:
            foreach (var fltr in fltrlist.Fltrs)
            {
                fltr.Fieldname = DposUtils.AdjustFieldname(fltr.Fieldname, EntityFieldlist.Keys.ToList<string>());
            }
            return fltrlist;
        }

        public FltrList LoadReferences()
        {
            //wird nicht aufgerufen
            //In Razor Seite: lnav.References = new FltrList("lityp=<>X");
            string re = "lityp=<>X";
            return new FltrList(re);
        }

        //SQL Where Caluse generieren: Für Radzen Query
        public string Filter { get; set; }
        public object[] FilterParameters { get; set; }

        #endregion

        #region LoadData und GenFilter

        [Flags]
        public enum UseFltrs
        {
            UseAll = 0,
            UseFltrlist = 1,
            UseReferences = 2
        }

        public void GenFilter()
        {
            GenFilter(UseFltrs.UseAll);
        }

        public void GenFilter(UseFltrs useFltrs)
        {
                var allFltrlist = new FltrList();
            if (useFltrs == UseFltrs.UseFltrlist || useFltrs == UseFltrs.UseAll)
                allFltrlist.Fltrs.AddRange(Fltrlist.Fltrs);
            if (useFltrs == UseFltrs.UseReferences || useFltrs == UseFltrs.UseAll)
                allFltrlist.Fltrs.AddRange(References.Fltrs);
            //todo: FltrList und References zusammenführen (evtl über die SqlTokens, oder später getrennt zusammenführen:Parameter? )

            //Fltrlist.GenSqlWhere(_filter, _filterparameter);  //schreibt nach _filter und Parameter
            //erstmal ohne Parameter:
            allFltrlist.GenSqlWhere(EntityFieldlist);  //schreibt nach SqlWhere und SqlParams
            Filter = allFltrlist.SqlWhere;
            FilterParameters = allFltrlist.SqlParams.Values.ToArray<object>();

            //Generate SQL: anhand FltrList und References:
            //works _filter = "(lityp=\"B\" or lityp=\"A\") and (lort_nr=\"57\") and anl_na1 .contains(\"AGH\") and (sta=\"H\")";
            //DynamicFunctions.Like(Brand, \"%a%\")
            //_filter = "(lityp=\"B\" or lityp=\"A\") and (lort_nr=\"57\") and Like(anl_na1, \"%A_H%\" and (sta=\"H\")";
        }

        public async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;
            await Task.Yield();

            LoadAbfrage();  //von FLTR neu einlesen (Columnlist, Fltrlist,..)

            //pagesize anpassen falls in GNav/LNav geändert
            if (Paging || Virtualization)
            {
                //verwaltung extra
            }
            else
            {
                pagesize = gnav.MaxRecordCount;
                if (args.Skip == 0 && args.Top != pagesize)
                {
                    prot?.SMessL($"Top: {args.Top} => {pagesize}");
                    args.Top = pagesize;
                }
            }
            //merken für später
            if (string.IsNullOrEmpty(args.OrderBy))
                args.OrderBy = OrderBy;  //von Vorgabe
            else
                OrderBy = args.OrderBy;
            prot?.SMessL($"Skip: {args.Skip}, Top: {args.Top}, pagesize={pagesize}");

            GenFilter(UseFltrs.UseFltrlist);  //SQL Filter und Parameters generieren. LuDef:nur Fltrlist!
            var query = data.QueryFromLoadDataArgs(args);
            query.Filter = Filter;
            query.FilterParameters = FilterParameters;
            prot?.Prot0SL($"Filter:{Filter}");
            prot?.Prot0SL($"Filterparameter:{JsonSerializer.Serialize(query.FilterParameters)}");
            tbl = data.EntityQuery<TItem>(query).ToList();
            //Idee ohne Data Service: tbl = lnav.queryList();

            Recordcount = data.EntityQueryCount<TItem>(query);
            if (gnav != null) 
                gnav.StatusChanged();
            prot?.SMessL($"Loaded. Count: {Recordcount}");

            isLoading = false;
        }


        #endregion

    }
}
