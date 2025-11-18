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

// Configurar la sesión
builder.Services.AddDistributedMemoryCache(); // Almacenamiento en memoria para la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar el DbContext (Conexión a PostgreSQL en Render)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AventuraBlockchainDbContext>(options =>
    options.UseSqlServer(connectionString)
);


// Cargar la sección BlockchainSettings
var blockchainSettingsSection = builder.Configuration.GetSection("BlockchainSettings");
var blockchainSettings = blockchainSettingsSection.Get<BlockchainSettings>() ?? new BlockchainSettings();
builder.Services.AddSingleton(blockchainSettings);


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

builder.Services.AddSingleton<Account>(provider =>
{
    var settings = provider.GetRequiredService<BlockchainSettings>();
    var privateKey = settings.BackendPrivateKey;
    Console.WriteLine($"PRIVATE KEY LEN = {privateKey?.Length}");
    Console.WriteLine($"PRIVATE KEY     = '{privateKey}'");

    return new Account(privateKey);

});

// Registrar Web3 usando ese Account
builder.Services.AddSingleton<Web3>(provider =>
{
    var account = provider.GetRequiredService<Account>();
    var settings = provider.GetRequiredService<BlockchainSettings>();
    return new Web3(account, settings.NodeUrl);
});



builder.Services.AddScoped<JuegoLogica>();
builder.Services.AddScoped<ExploradorLogica>();
builder.Services.AddScoped<ITemporizador, TemporizadorLogica>();

builder.Services.AddScoped<ILogicaDeJuego, JuegoLogica>();
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
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();