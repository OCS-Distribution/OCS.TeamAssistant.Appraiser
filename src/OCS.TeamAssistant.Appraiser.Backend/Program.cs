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
	.AddCommands();

var app = builder.Build();

app.Map("/", () => "Backend running...");

app.Run();