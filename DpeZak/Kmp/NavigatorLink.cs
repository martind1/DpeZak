namespace DpeZak.Kmp
{
    public interface INavigatorLink
    {
        public int Recordcount { get; set; }
        public string Pagetitle { get; set; }
    }

    public class DummyEntity
    {
        public Guid Id { get; set; }
    }

    public enum nlOwner
    {
        ownLNav,
        ownLuDef
    }


    /// <summary>
    /// Basisklasse für LocalService und LookupDef: gemeinsame Sachen
    /// </summary>
    public class NavigatorLink<TItem>
    {
        public nlOwner owner;
        public bool isLoading = false;
        public int pagesize;  //LNav bzw GNax MaxRecordCount

        public bool Paging { get; set; } = false;
        public bool Virtualization { get; set; } = false;


        public int Recordcount { get; set; }  //Istwert von DB


        public IList<TItem> tbl;  //oder Navlink


    }


}
