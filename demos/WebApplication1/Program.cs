using Serilog;
using WebApiClientCore.Extensions.OAuths.TokenProviders;
using WebApplication1;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, logger) => logger.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddRazorPages();

//先运行 WebApplication2 作为 API服务器
builder.Services.AddHttpApi<ITestApi>();
builder.Services.AddTokenProvider<ITestApi, TokenProvider>(serviceProvider =>
{
    var service = serviceProvider.GetRequiredService<ITestApi>();
    return new TestDynamicProvider(serviceProvider, service);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();