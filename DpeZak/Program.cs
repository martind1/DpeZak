using Radzen;
using Serilog;
using DpeZak.Connection.DPE;
using DpeZak.Data;
using DpeZak.Kmp;

//md Serilog noch ohne Builder:
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();
Log.Information("Programmstart");

try
{
    var builder = WebApplication.CreateBuilder(args);

    //md Serilog anhand appsettings.json:
    builder.Host.UseSerilog((ctx, lc) => lc
            //.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration));

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddSingleton<WeatherForecastService>();

    //Problem IIS 21.10.22:
    builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });

    //md radzen:
    builder.Services.AddScoped<DialogService>();
    builder.Services.AddScoped<NotificationService>();
    builder.Services.AddScoped<TooltipService>();
    builder.Services.AddScoped<ContextMenuService>();
    
    //md Dpe:
    builder.Services.AddSqlServer<DpeContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
    builder.Services.AddDbContext<DpeContext>();
    builder.Services.AddScoped<DpeData>();
    //md Serilog:
    builder.Services.AddLogging();
    builder.Logging.AddSerilog();
    //md Utils:
    builder.Services.AddScoped<GlobalService>();
    builder.Services.AddScoped<ProtService>();

    //md ab .NET 6 kein Startup.cs mehr notwendig!

    var app = builder.Build();

    //md Serilog:
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
    //md Fehler erzwingen:
    //var a = 0; var b = 5; var c = b / a;

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Schwerer Fehler");
}
finally
{
    Log.Information("Programmende");
    Log.CloseAndFlush();
}

