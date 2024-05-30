using Shared.Services.Abstract;
using Shared.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IEventStoreService, EventStoreService>();
//Ama� eventsource mant���n� g�stermek oldu�u i�in do�rudan singleton kullan�ld�. Projenin senaryosuna g�re Scoped ya da Transient olarak da kullan�labilir.
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
