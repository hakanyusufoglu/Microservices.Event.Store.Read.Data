using Product.Event.Handler.Service.Services;
using Shared.Services.Abstract;
using Shared.Services.Concrete;
using EventStoreService = Shared.Services.Concrete.EventStoreService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<EventStoreBackgroundService>();

builder.Services.AddSingleton<IEventStoreService, EventStoreService>();
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();

var host = builder.Build();
host.Run();
