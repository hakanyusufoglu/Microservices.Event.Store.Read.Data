using EventStore.Client;

namespace Shared.Services.Abstract
{
	public interface IEventStoreService
	{
		//Bir eventin gerekli olan imzaları

		Task AppendToStreamAsync(string streamName, IEnumerable<EventData> eventDatas);

		//Verilen evente karşı event data verecektir.
		EventData GenerateEventData(object @event);

		//Belirli bir stream'e abone olmak için kullanılır.
		Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppend);
	}
}
