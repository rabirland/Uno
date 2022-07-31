using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Uno.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var appUri = new Uri(builder.HostEnvironment.BaseAddress);

builder.Services.AddHttpClient("Uno.ServerAPI", client => client.BaseAddress = appUri);

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("Uno.ServerAPI");
    return client;
});

builder.Services.AddScoped(sp => new Api(sp.GetRequiredService<HttpClient>()));

await builder.Build().RunAsync();
