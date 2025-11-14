using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;
using ST10367702_Final_CLDV6212.Data;
using ST10367702_Final_CLDV6212.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<StorageService>();
builder.Services.AddScoped<SqlRepository>();


// Bind configuration
var storageConnection = builder.Configuration.GetConnectionString("StorageAccount");

builder.Services.AddSingleton(sp =>
{
    return new TableServiceClient(storageConnection);
});

builder.Services.AddSingleton(sp =>
{
    return new BlobServiceClient(storageConnection);
});

builder.Services.AddSingleton(sp =>
{
    return new QueueServiceClient(storageConnection);
});

builder.Services.AddSingleton(sp =>
{
    return new ShareServiceClient(storageConnection);
});

// Custom service that wraps all storage operations
builder.Services.AddScoped<StorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
