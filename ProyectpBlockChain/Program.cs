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

// Registrar la configuración de blockchain
builder.Services.Configure<BlockchainSettings>(
    builder.Configuration.GetSection("BlockchainSettings"));

// Web3 solo si necesitás hacer llamadas de lectura desde el backend
builder.Services.AddSingleton(provider =>
{
    var blockchainSettings = provider.GetRequiredService<IOptions<BlockchainSettings>>().Value;
    return new Web3(blockchainSettings.NodeUrl); // Solo lectura
});

/*builder.Services.AddScoped<JuegoLogica>();*/
builder.Services.AddScoped<HistoriaLogica>();
builder.Services.AddScoped<UsuarioLogica>();
builder.Services.AddSingleton<EstadoGlobal>();

/*builder.Services.AddScoped<ILogicaDeJuego, JuegoLogica>();*/
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();