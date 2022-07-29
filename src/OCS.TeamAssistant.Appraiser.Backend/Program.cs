using OCS.TeamAssistant.Appraiser.Backend;
using OCS.TeamAssistant.Appraiser.Backend.Commands;
using OCS.TeamAssistant.Appraiser.Backend.Services;
using OCS.TeamAssistant.Appraiser.DataAccess.InMemory;
using OCS.TeamAssistant.Appraiser.Notifications;

var builder = WebApplication.CreateBuilder(args);

var telegramBotOptions = builder.Configuration.GetSection(nameof(TelegramBotOptions)).Get<TelegramBotOptions>();

builder.Services
    .AddInMemoryDataAccess()
	.AddNotifications(telegramBotOptions.LinkTemplate, CommandsList.Set, CommandsList.NoIdea)
	.AddServices(telegramBotOptions.AccessToken)
	.AddCommands()
	.AddMvc();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}

app
	.UseStaticFiles()
	.UseBlazorFrameworkFiles()
	.UseRouting()
	.UseEndpoints(endpoints =>
		{
			endpoints.MapDefaultControllerRoute();
			endpoints.MapFallbackToPage("/_Host");
		});

app.Run();