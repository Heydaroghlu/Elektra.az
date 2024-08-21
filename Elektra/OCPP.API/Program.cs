using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OCPP.Core.Application.Profiles;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Persistence.UnitOfWorks;
using System.Data;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using OCPP.Core.Infrastructure.Services.HangFireService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHangfire(configuration =>
{
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("SqlServer")); // SQL Server için
});
builder.Services.AddHangfireServer(); // Hangfire sunucusunu ekleyin

// Diğer servisleri ekleyin
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<StoredProcedureService>(); // Servisinizi ekleyin

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOCPPDbContext(builder.Configuration);
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 6;
    opt.Password.RequiredUniqueChars = 0;
    opt.Password.RequireUppercase = false;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<OCPPCoreContext>();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MapperProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Diğer yapılandırma kodları
app.UseRouting();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

// Hangfire Dashboard'ı ekleyin (isteğe bağlı)
app.UseHangfireDashboard();

app.MapControllers();

// Saklı prosedürü çalıştıracak iş tanımlayın
var scope = app.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<StoredProcedureService>();

// Hangfire işlerini tanımlayın
RecurringJob.AddOrUpdate(
    "execute-sql-command",
    () => service.ExecuteStoredProcedure(),
    "*/10 * * * * *"); // Cron ifadesi: Her 10 saniyede bir

app.Run();
