using BusinessLayer.Services;
using BusinessLayer.Services.Implementations;
using DataAccessLayer;
using DataAccessLayer.Data;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// UnitOfWork ve Repository registrations
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICodesRepository, CodesRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IFormulaRepository, FormulaRepository>();
builder.Services.AddScoped<IInjectionRepository, InjectionRepository>();
builder.Services.AddScoped<IInjectionDailyRepository, InjectionDailyRepository>();
builder.Services.AddScoped<IInjectionStockRepository, InjectionStockRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IPlıntusRepository, PlıntusRepository>();
builder.Services.AddScoped<IPlıntusDailyRepository, PlıntusDailyRepository>();
builder.Services.AddScoped<IPlıntusStockRepository, PlıntusStockRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<ISalesDetailRepository, SalesDetailRepository>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();

// Service registrations
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICodesService, CodesService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IFormulaService, FormulaService>();
builder.Services.AddScoped<IInjectionService, InjectionService>();
builder.Services.AddScoped<IInjectionDailyService, InjectionDailyService>();
builder.Services.AddScoped<IInjectionStockService, InjectionStockService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IPlıntusService, PlıntusService>();
builder.Services.AddScoped<IPlıntusDailyService, PlıntusDailyService>();
builder.Services.AddScoped<IPlıntusStockService, PlıntusStockService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<ISalesDetailService, SalesDetailService>();
builder.Services.AddScoped<ISizeService, SizeService>();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
