using Microsoft.AspNetCore.Components;
using DpeZak.Kmp;

namespace Utils
{

    // von HSchwichtenberg_Blazor60-Buch_Beispiele_v6.12.0
    // Polling Komponente
    // Anwendung: <BlazorTimer Seconds="1" Callback="Aktion"></BlazorTimer> # void Aktion()
    public class BlazorTimer : ComponentBase, IDisposable
    {
        [Inject]
        private ProtService Prot { get; set; }

        [Parameter]
        public double Seconds { get; set; }

        [Parameter]
        public EventCallback Callback { get; set; }

        System.Threading.Timer timer; // Außerhalb wegen GC!
        protected override void OnInitialized()
        {
            Prot.SMessL("Starte Timer...(" + Seconds + " seconds)");
            timer = new System.Threading.Timer(
              callback: (_) => InvokeAsync(() => Callback.InvokeAsync(null)),
              state: null,
              dueTime: TimeSpan.FromSeconds(Seconds),
              period: TimeSpan.FromSeconds(Seconds));
        }

        public void Dispose()
        {
            // wichtig, damit Timer nicht weiterläuft, wenn die Komponente schon nicht mehr lebt
            if (timer != null)
            {
                timer.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}
