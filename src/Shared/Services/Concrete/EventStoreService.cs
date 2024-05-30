using EventStore.Client;
using Shared.Services.Abstract;
using System.Text.Json;

namespace Shared.Services.Concrete
{
	public class EventStoreService : IEventStoreService
	{
		//Todo Amaç mikroservis eventstore olduğu için bu statik veri (connectionstrings vs.) normalde appsettings.json dosyasından alınması gerekir (best practise).
		EventStoreClientSettings GetEventStoreClientSettings(string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false")
			=> EventStoreClientSettings.Create(connectionString);

		EventStoreClient Client { get => new(GetEventStoreClientSettings()); }

		//EventStore'a event eklemek için kullanılır.
		public async Task AppendToStreamAsync(string streamName, IEnumerable<EventData> eventDatas) => await Client.AppendToStreamAsync(
			streamName: streamName,
			eventData: eventDatas,
			expectedState: StreamState.Any
			);

		//Verilen evente karşı event data verecektir.
		public EventData GenerateEventData(object @event) => new(
			eventId: Uuid.NewUuid(),
			type: @event.GetType().Name,
			data: JsonSerializer.SerializeToUtf8Bytes(@event)
			);

		// Belirli bir stream'e abone olmak için kullanılır.
		public async Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppend)
		=> await Client.SubscribeToStreamAsync(
			streamName: streamName,
			start:FromStream.Start, //Başlangıçtan itibaren eventleri almak için
			eventAppeared: eventAppend,
			subscriptionDropped:(streamSubscription, subscriptionDroppedReason, exception)=> Console.WriteLine("Disconnected!") // Abonelik düşerse
			);
	}
}
