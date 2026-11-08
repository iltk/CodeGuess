using CodeGuess.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Each service gets its own HttpClient pointed at the API
var apiBaseUrl = new Uri(builder.Configuration["ApiBaseUrl"]!);

builder.Services.AddHttpClient<IDeckService, DeckService>(client => client.BaseAddress = apiBaseUrl);
builder.Services.AddHttpClient<IGameService, GameService>(client => client.BaseAddress = apiBaseUrl);

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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
