using Microsoft.Extensions.Options;
using Nasa.configuracion;
using Nasa.Servicio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddHttpClientFactory();

builder.Services.AddControllersWithViews();

builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("ApiKeys:OpenWeatherMap"));
builder.Services.Configure<NasaApiSettings>(builder.Configuration.GetSection("ApiKeys:NasaApiKey"));

// 2. Configuración de endpoints
builder.Services.Configure<WeatherApiSettings>(options =>
{
    options.ApiKey = builder.Configuration["ApiKeys:OpenWeatherMap"];
    options.Endpoint = builder.Configuration["ApiEndpoints:OpenWeatherMap"];
});

builder.Services.Configure<NasaApiSettings>(options =>
{
    options.ApiKey = builder.Configuration["ApiKeys:NasaApiKey"];
    options.ApodEndpoint = builder.Configuration["ApiEndpoints:NasaApod"];
    options.MarsPhotosEndpoint = builder.Configuration["ApiEndpoints:NasaMarsPhotos"];
});

// Configuración de HttpClient con políticas de reintento
builder.Services.AddHttpClient("WeatherClient", client => {
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("NasaClient", client => {
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 3. Registro de servicios
builder.Services.AddScoped<ClimaApiService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var settings = provider.GetRequiredService<IOptions<WeatherApiSettings>>().Value;
    return new ClimaApiService(
        httpClientFactory.CreateClient("WeatherClient"),
        settings
    );
});

builder.Services.AddScoped<NasaApiService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var settings = provider.GetRequiredService<IOptions<NasaApiSettings>>().Value;
    return new NasaApiService(
        httpClientFactory.CreateClient("NasaClient"),
        settings
    );
});

/*builder.Services.AddHttpClient<NasaApiService>();

builder.Services.AddTransient<NasaApiService>();

builder.Services.AddHttpClient<ClimaApiService>();*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
