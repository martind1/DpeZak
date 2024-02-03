using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using NuGet.Protocol;
using Radzen;
using Radzen.Blazor;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using DpeZak.Data;
using DpeZak.Entities.DPE;
using static DpeZak.Kmp.DposUtils;

namespace DpeZak.Kmp
{

    //Der lokale Navigator
    public class LocalService<TItem>: INavigatorLink
        where TItem : class
    {
        //hier kein Inject !! (GNav, DpeData, Prots usw) -->von Page übergeben ->später:GNav als Singleton anlegen!!!

        //todo: nach NLnk - NavigatorLink - LinkService
        //Idee: hier auch individueller MaxRecordCount
        private ColumnList _columnlist;
        public ColumnList Columnlist {
            get { 
                _columnlist ??= LoadColumnlist();
                LoadKeyFields();  //hier wg überschreibt Columnlist
                return _columnlist;
            }
            set => _columnlist = value; 
        }
        private string _keyfields;
        public string KeyFields { get => _keyfields ??= LoadKeyFields(); set => _keyfields = value; }
        public string OrderBy { get; set; }  //von Radzen.LoadDataArgs
        private FltrList _fltrlist;
        public FltrList Fltrlist { get => _fltrlist ??= LoadFltrlist(); set => _fltrlist = value; }
        public FltrList References { get; set; }  //nicht durch User änderbar


        //Liste mit Original Feldnamen (Groß/Kleinschreibung). Lazy Loading.
        private IDictionary<string, FieldInfo> _entityFieldlist;
        public IDictionary<string, FieldInfo> EntityFieldlist { 
            get => _entityFieldlist ??= GetFieldlist(typeof(TItem));
            set => _entityFieldlist = value; }

        protected DpeData data;
        private FLTR _fltrRec;  //Datensatz aus Tabelle 'FLTR'
        public FLTR FltrRec { get => _fltrRec ??= data.GetFltr(FormKurz, Abfrage); }


        private string _abfrage;
        public string Abfrage { get => _abfrage; set => SetAbfrage(value); }

        //Bei neuer Abfrage Metadaten neu einlesen
        public void SetAbfrage(string value)
        {
            var oldAbfrage = _abfrage;
            _abfrage = value;
            //wenn Abfrage entfernt wird dann Rest nicht entfernen.
            //Abfrage wird entfernt bei User-Änderungen bzgl Filter,Sort,Columns
            if (!String.IsNullOrEmpty(value) && (oldAbfrage ?? string.Empty) != value) {
                //s.o. _abfrage = value;
                RefreshAbfrage();
            }
        }

        //erzwingt Neueinlesen von FLTR Tabelle
        public void RefreshAbfrage()
        {
            _fltrRec = null; //neu Laden von Abfrage
            _columnlist = null; //neu Laden von Abfrage
            _keyfields = null; //neu Laden von Abfrage
            _fltrlist = null; //neu Laden von AbfrageLoadFltrlist();
        }
        private string _formKurz;
        public string FormKurz { get => _formKurz; set => SetFormKurz(value); }
        public void SetFormKurz(string value)
        {
            if ((_formKurz ?? string.Empty) != (value ?? string.Empty))
            {
                _formKurz = value;
                _fltrRec = null; //neu Laden von Abfrage
                _columnlist = null; //neu Laden von Abfrage
                _keyfields = null; //neu Laden von Abfrage
                _fltrlist = null; //neu Laden von AbfrageLoadFltrlist();
            }
        }

        private readonly GlobalService gnav;

        public LocalService()
        {
            //einer ohne Parameter muss sein
        }

        public LocalService(GlobalService gnav)
        {
            //siehe dort - EntityFieldlist = DposUtils.GetFieldlist(typeof(TItem));
            this.gnav = gnav;  //Minimalfeld für Benachrichtigungen
        }

        public LocalService(GlobalService gnav, DpeData data, string formKurz, string abfrage): this()
        {
            //todo: an GNav weiterleiten
            this.gnav = gnav;  //muss sein wg Session
            this.data = data;  //muss sein wg Session

            _formKurz = formKurz;
            _abfrage = abfrage;  //ohne SetAbfrage

            //References = LoadReferences(); * auf Seite
        }

        #region Live (recordcount, ..)

        private int _recordcount;
        public int Recordcount { 
            get { return _recordcount; }
            set { _recordcount = value;
                if (gnav != null) gnav.StatusChanged();
            }
        }

        private string _pagetitle;
        public string Pagetitle
        {
            get { return _pagetitle; }
            set
            {
                _pagetitle = value;
                if (gnav != null) gnav.StatusChanged();
            }
        }

        private GlobalService.PageState? _pageState;
        public GlobalService.PageState PageState {
            get { _pageState ??= GlobalService.PageState.Multi; 
                  return (GlobalService.PageState)_pageState; }
            set => _pageState = value; 
        }

        #endregion



        #region ColumnList  

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

        #endregion

        #region KeyFields

        //KeyFields von Abfrage laden und nach Columnlist.Sortorder übertragenb
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
            // beware Columnlist sonst deadlock!
            foreach (var col in _columnlist.Columns)
            {
                if (fields.TryGetValue(col.Fieldname, out string keyvalue))
                    col.SortOrder = keyvalue.ToLower() == "asc" ? Radzen.SortOrder.Ascending : Radzen.SortOrder.Descending;
                else
                    col.SortOrder = null;
            }

            return kf;
        }

        #endregion

        #region FltrList und References

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
        private string _filter;
        public string Filter { get => _filter; set => _filter = value; }
        private object[] _filterParameters;
        public object[] FilterParameters { get => _filterParameters; set => _filterParameters = value; }

        public void GenFilter()
        {
            var allFltrlist = new FltrList();
            allFltrlist.Fltrs.AddRange(Fltrlist.Fltrs);
            allFltrlist.Fltrs.AddRange(References.Fltrs);
            //todo: FltrList und References zusammenführen (evtl über die SqlTokens, oder später getrennt zusammenführen:Parameter? )

            //Fltrlist.GenSqlWhere(_filter, _filterparameter);  //schreibt nach _filter und Parameter
            //erstmal ohne Parameter:
            allFltrlist.GenSqlWhere(EntityFieldlist);  //schreibt nach SqlWhere und SqlParams
            _filter = allFltrlist.SqlWhere;
            _filterParameters = allFltrlist.SqlParams.Values.ToArray<object>();

            //Generate SQL: anhand FltrList und References:
            //works _filter = "(lityp=\"B\" or lityp=\"A\") and (lort_nr=\"57\") and anl_na1 .contains(\"AGH\") and (sta=\"H\")";
            //DynamicFunctions.Like(Brand, \"%a%\")
            //_filter = "(lityp=\"B\" or lityp=\"A\") and (lort_nr=\"57\") and Like(anl_na1, \"%A_H%\" and (sta=\"H\")";
        }

        #endregion

    }
}
