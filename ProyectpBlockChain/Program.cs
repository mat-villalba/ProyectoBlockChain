using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica;
using ProyectoBlockChain.Logica.Interfaces;
using ProyectoBlockChain.Web;

var builder = WebApplication.CreateBuilder(args);

// Registrar el DbContext (Conexión a PostgreSQL en Render)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AventuraBlockchainDbContext>(options =>
    options.UseNpgsql(connectionString)
);


// Cargar la sección BlockchainSettings
var blockchainSettingsSection = builder.Configuration.GetSection("BlockchainSettings");
var blockchainSettings = blockchainSettingsSection.Get<BlockchainSettings>() ?? new BlockchainSettings();

// Leer el archivo ABI (si existe)
if (!string.IsNullOrEmpty(blockchainSettings.ContractAbi))
{
    var abiPath = Path.Combine(AppContext.BaseDirectory, blockchainSettings.ContractAbi);
    if (File.Exists(abiPath))
    {
        var abi = File.ReadAllText(blockchainSettings.ContractAbi);
        blockchainSettings.ContractAbi = abi;
    }
    else
    {
        Console.WriteLine($"⚠️ No se encontró el archivo ABI en {abiPath}");
    }
}

// Registrar BlockchainSettings como singleton con los valores ya cargados
builder.Services.AddSingleton(blockchainSettings);

// Si necesitás un Web3 para lecturas
builder.Services.AddSingleton(provider =>
{
    return new Web3(blockchainSettings.NodeUrl);
});


builder.Services.AddScoped<JuegoLogica>();
builder.Services.AddScoped<HistoriaLogica>();
builder.Services.AddScoped<UsuarioLogica>();
builder.Services.AddScoped<ExploradorLogica>();
builder.Services.AddSingleton<EstadoGlobal>();

builder.Services.AddScoped<ILogicaDeJuego, JuegoLogica>();
builder.Services.AddScoped<ILogicaHistorial, HistoriaLogica>();
builder.Services.AddScoped<ILogicaJugador, UsuarioLogica>();
builder.Services.AddScoped<ILogicaExplorador, ExploradorLogica>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();