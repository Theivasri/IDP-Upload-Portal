using IDPUpload_Portal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<CosmosMongoService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AzureFunctionService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();