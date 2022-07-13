using MediatR;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Application.Contracts.Commands.CreateAssessmentSession;
using OCS.TeamAssistant.Appraiser.Backend.Options;
using OCS.TeamAssistant.Appraiser.Backend.Services;
using OCS.TeamAssistant.Appraiser.DataAccess.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInMemoryDataAccess()
    .AddMediatR(typeof(CreateAssessmentSessionCommand));

builder.Services
    .AddSingleton<IEstimatesService, EstimatesService>()
    .AddSingleton<CommandFactory>()
    .AddSingleton<CommandResultProcessor>()
    .AddHostedService<TelegramBotListener>()
    .AddSingleton<TelegramBotMessageHandler>()
    .Configure<TelegramBotOptions>(builder.Configuration.GetSection(nameof(TelegramBotOptions)));

var app = builder.Build();

app.Map("/", () => "Backend running...");

app.Run();