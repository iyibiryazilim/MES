using LBS.WebAPI.Service.DataStores;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddLogging();
builder.Services.AddSingleton<IHttpClientService, HttpClientService>();
builder.Services.AddTransient<IWorkstationGroupService, WorkstationGroupDataStore>();
builder.Services.AddTransient<IWorkstationServise, WorkStationDataStore>();
builder.Services.AddTransient<IProductService,ProductDataStore>();
builder.Services.AddTransient<IWorkOrderService, WorkOrderDataStore>();
builder.Services.AddTransient<IProductionService, ProductionOrderDataStore>();
builder.Services.AddTransient<IEmployeeGroupService, EmployeeGroupDataStore>();
builder.Services.AddTransient<IEmployeeService, EmployeeDataStore>();
builder.Services.AddTransient<IStopCauseService, StopCauseDataStore>();
builder.Services.AddTransient<IEndProductService, EndProductDataStore>();
builder.Services.AddTransient<ISemiProductService, SemiProductDataStore>();
builder.Services.AddTransient<IRawProductService, RawProductDataStore>();
builder.Services.AddTransient<ISalesOrderLineService, SalesOrderLineDataStore>();
builder.Services.AddTransient<IPurchaseOrderLineService, PurchaseOrderLineDataStore>();
builder.Services.AddTransient<IOperationService, OperationDataStore>();
builder.Services.AddTransient<IShiftService, ShiftDataStore>();



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

