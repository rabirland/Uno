using Uno.Server;
using Uno.Server.Middlewares;
using Uno.Shared;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("https://*:80", "https://localhost:80");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMarkedServices();

// ================================== APP
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.UseMiddleware<DataStreamMiddleware>(); // Advised to come after every other middleware
/*TODO: MAKE EXTENSION METHOD:
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.UseMiddleware<MyMiddlewareOne>();
});*/

// Premature validations
EnumMapper.EnsureValueMatching<GameMessages.CardColor, UnoGame.CardColor>();
EnumMapper.EnsureValueMatching<GameMessages.CardType, UnoGame.CardType>();
EnumMapper.EnsureValueMatching<GameMessages.RoundPhase, UnoGame.RoundPhase>();

app.Run();
