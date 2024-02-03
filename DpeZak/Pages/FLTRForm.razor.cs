using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Radzen.Blazor;
using System.Text.Json;
using DpeZak.Data;
using DpeZak.Entities.DPE;
using DpeZak.Kmp;

namespace DpeZak.Pages
{
    public partial class FLTRForm
    {
        RadzenDataGrid<FLTR> grid;
        IList<FLTR> tbl;
        IList<FLTR> selectedList;
        LocalService<FLTR> lnav;

        [Parameter]
        public string Abfrage { get; set; } = "Standard";
        public string formKurz = "FLTR";

        [Inject]
        private GlobalService GNav { get; set; }
        [Inject]
        private ProtService Prot { get; set; }

        public FLTRForm()
        {
            //GNav ist hier noch null!
            //grid ist hier noch null!
        }

        [Inject]
        DpeData Data { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            GNav.OnDoKommando += RunKommando;
            pagesize = GNav.MaxRecordCount;
            //Prot.SMessL($"Init. Abfrage={Abfrage}");  //hier noch keine console!
        }

        protected override void OnParametersSet()
        {
            Prot.SMessL($"Init. Abfrage={Abfrage}");  //hier console!

            lnav = new LocalService<FLTR>(GNav, Data, formKurz, Abfrage)
            {
                Pagetitle = "Abfragen",
                References = new FltrList("")
            };
            GNav.SetLNav(lnav);
        }

        #region Konfiguration

        public int PollSeconds { get; set; } = 120;

        #endregion

        #region Demo Inline Insert, Edit, Delete

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //Prot.SMessL($"Init. Abfrage={Abfrage}");  //hier console
            }
            return base.OnAfterRenderAsync(firstRender);
        }

        async Task EditRow(FLTR fltr)
        {
            await grid.EditRow(fltr);
        }

        void OnUpdateRow(FLTR fltr)
        {
            if (fltr == entityToInsert)
            {
                entityToInsert = null;
            }

            Data.EntityUpdate<FLTR>(fltr);
            // For demo purposes only
            //fltr.Customer = dbContext.Customers.Find(fltr.CustomerID);
            //fltr.Employee = dbContext.Employees.Find(fltr.EmployeeID);
            // For production
            //dbContext.SaveChanges();
        }

        async Task SaveRow(FLTR fltr)
        {
            if (fltr == entityToInsert)
            {
                entityToInsert = null;
            }

            await grid.UpdateRow(fltr);
        }

        void CancelEdit(FLTR fltr)
        {
            if (fltr == entityToInsert)
            {
                entityToInsert = null;
            }

            grid.CancelEditRow(fltr);
            // For production
            var vorfEntry = Data.EntityEntry<FLTR>(fltr);
            if (vorfEntry.State == EntityState.Modified)
            {
                vorfEntry.CurrentValues.SetValues(vorfEntry.OriginalValues);
                vorfEntry.State = EntityState.Unchanged;
            }
        }

        async Task DeleteRow(FLTR fltr)
        {
            if (fltr == entityToInsert)
            {
                entityToInsert = null;
            }

            if (tbl.Contains(fltr))
            {
                Data.EntityRemove<FLTR>(fltr);
                // For demo purposes only
                tbl.Remove(fltr);
                // For production
                //dbContext.SaveChanges();
                await grid.Reload();
            }
            else
            {
                grid.CancelEditRow(fltr);
            }
        }

        FLTR entityToInsert;
        async Task InsertRow()
        {
            entityToInsert = new FLTR();
            await grid.InsertRow(entityToInsert);
        }

        void OnCreateRow(FLTR fltr)
        {
            Data.EntityAdd<FLTR>(fltr);
            // For demo purposes only
            //fltr.Customer = dbContext.Customers.Find(fltr.CustomerID);
            //fltr.Employee = dbContext.Employees.Find(fltr.EmployeeID);
            // For production
            //dbContext.SaveChanges();
        }
        #endregion

        #region von Sped:
        protected bool isLoading = false;
        protected int pagesize;  //LNav bzw GNax MaxRecordCount

        protected async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;
            await Task.Yield();

            //pagesize anpassen falls in GNav/LNav geändert
            pagesize = GNav.MaxRecordCount;
            if (args.Skip == 0 && args.Top != pagesize)
            {
                Prot.SMessL($"Top: {args.Top} => {pagesize}");
                args.Top = pagesize;
            }
            //merken für später
            lnav.OrderBy = args.OrderBy;
            Prot.SMessL($"Skip: {args.Skip}, Top: {args.Top}, pagesize={pagesize}");

            lnav.GenFilter();  //SQL Filter und Parameters generieren
            var query = Data.QueryFromLoadDataArgs(args);
            query.Filter = lnav.Filter;
            query.FilterParameters = lnav.FilterParameters;
            Prot.Prot0SL($"Filter:{query.Filter}");
            Prot.Prot0SL($"Filterparameter:{JsonSerializer.Serialize(query.FilterParameters)}");
            tbl = Data.EntityQuery<FLTR>(query).ToList<FLTR>();
            //Idee ohne Data Service: tbl = lnav.queryList(args);

            lnav.Recordcount = Data.EntityQueryCount<FLTR>(query);
            Prot.SMessL($"Loaded. Count: {lnav.Recordcount}");

            isLoading = false;
        }

        #endregion

        #region Kommando von GlobalNavigator über GlobalService nach hier

        public void RunKommando(int KNr)
        {
            switch ((GlobalService.KommandoTyp)KNr)
            {
                case GlobalService.KommandoTyp.Suchen:
                    {
                        //SuchenDialog
                        //if (allowFiltering)
                        //    _ = InsertRow();
                        StateHasChanged();
                        break;
                    }
                case GlobalService.KommandoTyp.Refresh:
                    {
                        _ = Reset();
                        break;
                    }
                case GlobalService.KommandoTyp.Pagesize:
                    {
                        pagesize = GNav.MaxRecordCount;
                        _ = Reset();
                        break;
                    }
                case GlobalService.KommandoTyp.Erfass:
                    {
                        if (entityToInsert == null)
                            _ = InsertRow();
                        break;
                    }
            }
        }

        async Task Reset()
        {
            await grid.Reload();
        }

        #endregion

        void OnSort(DataGridColumnSortEventArgs<FLTR> args)
        {
            //
            //bringt EAccess string json = JsonConvert.SerializeObject(args);
            //bringt auch EAccess string json = System.Text.Json.JsonSerializer.Serialize(args);
            Prot.SMessL($"OnSort:{args.Column.Property} {args.SortOrder}");
            var colList = grid.ColumnsCollection.Where(c => c.SortOrder != null);
            foreach (var col in colList)
            {
                Prot.SMessL($"SortColumn Fld:{col.Property} SortProp:{col.SortProperty} a/d:{col.SortOrder}");

            }
        }


        /// <summary>
        /// Daten alle 120s refreshen (Tablet Hofliste)
        /// </summary>
        void Poll()
        {
            Prot.SMessL("Poll Timer: Reset");
            _ = Reset();
        }

    }
}