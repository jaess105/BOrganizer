using DB.Core;
using Rechnungen;
using Rechnungen.DB;
using Rechnungen.Services.General;
using Rechnungen.Services.Invoices;
using Rechnungen.Creator.PDF;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString =
    builder.Configuration.GetValue<string>("DB:POSTGRES") ??
    throw new ArgumentException("Could not find connections string");

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddSingleton<IRechnungsCreator>(_ => PdfRechnungsCreator.Init());
builder.Services.AddSingleton<IDbConnectionFactory>(_ => DbConnectionFactory.Init(connectionString));

// repos
builder.Services.AddSingleton<IBusinessRepository>(p =>
    new BusinessDao(connectionString, p.GetRequiredService<IDbConnectionFactory>()));
builder.Services.AddSingleton<IRechnungsNummerRepository>(p =>
    new RechnungsNummerDao(connectionString, p.GetRequiredService<IDbConnectionFactory>()));
builder.Services.AddSingleton<ICreditRepository>(p =>
    new CreditDao(connectionString, p.GetRequiredService<IDbConnectionFactory>()));
builder.Services.AddSingleton<IInvoiceRepository>(p =>
    new InvoiceDao(connectionString,
        p.GetRequiredService<IBusinessRepository>(),
        p.GetRequiredService<ICreditRepository>(),
        p.GetRequiredService<IRechnungsNummerRepository>(),
        p.GetRequiredService<IDbConnectionFactory>()));

// services
builder.Services.AddSingleton<IBusinessService, BusinessService>();
builder.Services.AddSingleton<IRechnungsService, RechnungsService>();

WebApplication app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{invoiceId?}");

app.Run();