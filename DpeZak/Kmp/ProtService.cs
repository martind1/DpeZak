using DpeZak.Shared;
using Microsoft.AspNetCore.Components;
using Serilog;
using System.Text.Json;

namespace DpeZak.Kmp
{
    /// <summary>
    /// Service für Protokollierung und Statusmeldung
    /// </summary>
    public class ProtService : ComponentBase
    {
        #region Statustext, Eventconsole
        public string StatusText { get; set; } = "Statustext";
        public event Action OnStatusTextChange;
        private void StatusTextChanged() => OnStatusTextChange?.Invoke();
        //todo: C# Event Vorgaben
        public EventConsole console;

        public void ProtX(ProtFlags flags, String Text)
        {
            if ((flags & ProtFlags.SMess) != 0)
            {
                StatusText = Text;
                //Ereignis zum Anzeigen woanders auslösen:
                StatusTextChanged();
            }
            if ((flags & ProtFlags.List) != 0)
            {
                if (console != null)
                    console.Log(Text);
                else
                    Debug0();
            }
            if ((flags & ProtFlags.File) != 0)
            {
                if ((flags & ProtFlags.Debug) != 0)
                    Log.Debug(Text);
                else
                    Log.Information(Text);
            }
        }


        public void Prot0(String Text)
        {
            ProtX(ProtFlags.File, Text);
        }

        public void Prot0SL(String Text)
        {
            ProtX(ProtFlags.File | ProtFlags.SMess | ProtFlags.List, Text);
        }

        public void SMess(String Text)
        {
            ProtX(ProtFlags.SMess, Text);
        }

        //Statuszeile mit Object->JSON
        public void SMess(object value)
        {
            SMess(JsonSerializer.Serialize(value));
        }

        //Statuszeile und Protlist
        public void SMessL(String Text)
        {
            ProtX(ProtFlags.SMess | ProtFlags.List, Text);
        }

        //Logfile und Statuszeile

        public void Debug0()
        {
            //macht nix. Nur für Brakpoint
        }

        #endregion

    }

    [Flags]
    public enum ProtFlags
    {
        SMess = 1,
        List = 2,
        File = 4,
        Database = 8,
        WarnDlg = 16,
        ErrDlg = 32,
        TimeStamp = 64,
        Debug = 128
    }
    /*
       TProtModus = (prTrm,    {Ausgabe in ListBox}
                prFile,        {Ausgabe in Protokolldatei}
                prRemain,      {Zeilenwechsel in Listbox unterdrücken}
                prTimeStamp,   {mit Protokolierung von Timestamp}
                prMsg,         {Ausgabe als Dialogbox}
                prList,        {Ausgabe in Listbox}
                prDatabase,    {Ausgabe in DB Tabelle}
                prWarn,        {i.V.m. prMsg: 'Warnung'   (WMess: 'Information'}
                prErr,         {i.V.m. WMessErr: 'Error'}
                prSMess,       {Ausgabe in Statuszeile}
                prNoLbStamp,   {In Listbox kein Timestamp, PC Nr}
                prNoLbFocus);  {In Listbox auch bei Focus auffüllen}

     */
}