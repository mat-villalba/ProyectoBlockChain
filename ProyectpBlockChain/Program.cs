using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Nethereum.Web3.Accounts;
using ProyectoBlockChain.Data.Data;
using ProyectoBlockChain.Logica;
using Nethereum.Web3;
using ProyectoBlockChain.Logica.Interfaces;
using System;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseNpgsql(connectionString)
);

// Registrar Nethereum (Conexión a la Blockchain)
string nodeUrl = builder.Configuration["BlockchainSettings:NodeUrl"];
string privateKey = builder.Configuration["BlockchainSettings:BackendPrivateKey"]; // Lee de appsettings.json

Account backendAccount = new(privateKey);
builder.Services.AddSingleton<Account>(backendAccount);
Web3 web3Instance = new Web3(backendAccount, nodeUrl);
builder.Services.AddSingleton<IWeb3>(web3Instance);


builder.Services.AddScoped<JuegoLogica>();
builder.Services.AddScoped<HistoriaLogica>();
builder.Services.AddScoped<UsuarioLogica>();
builder.Services.AddSingleton<EstadoGlobal>();

builder.Services.AddScoped<ILogicaDeJuego, JuegoLogica>();
builder.Services.AddScoped<ILogicaHistorial, HistoriaLogica>();
builder.Services.AddScoped<ILogicaJugador, UsuarioLogica>();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Jugador}/{action=IniciarSesion}/{id?}")
    .WithStaticAssets();


app.Run();