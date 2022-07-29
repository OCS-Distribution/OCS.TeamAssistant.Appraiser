using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OCS.TeamAssistant.Appraiser.Dashboard.Services;
using OCS.TeamAssistant.Appraiser.Model;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
	   .AddScoped(sp => new HttpClient {BaseAddress = new(builder.HostEnvironment.BaseAddress)})
	   .AddScoped<IAssessmentSessionsService, AssessmentSessionsService>();

await builder.Build().RunAsync();