namespace Shared.Events
{
	public class NewProductAddedEvent
	{
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		//Event oluştuğu andaki product sayısı
		public int InitialCount { get; set; }
		public bool IsAvailable { get; set; }
		public decimal InitialPrice { get; set; }
	}
}
