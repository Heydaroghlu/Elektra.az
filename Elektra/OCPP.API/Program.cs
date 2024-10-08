using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OCPP.Core.Application.Profiles;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Persistence.UnitOfWorks;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OCPP.Core.Application.Abstractions;
using OCPP.Core.Infrastructure.Hubs;
using OCPP.Core.Infrastructure.Services.HangFireService;
using OCPP.Core.Infrastructure.Services.Token;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddSignalR();
builder.Services.AddHttpClient(); // HttpClient hizmetini ekleyin

builder.Services.AddHangfire(configuration =>
{
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MapperProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddHangfireServer(); 
builder.Services.AddScoped<StoredProcedureService>();
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            //.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed((hosts) => true));
});

builder.Services.AddScoped<ITokenHandler, OCPP.Core.Infrastructure.Services.Token.TokenHandler>();
builder.Services.AddAuthentication("Elektra")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]))
        };
    });
var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();
app.MapHub<StatusHub>("/statushub");
app.MapControllers();

var scope = app.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<StoredProcedureService>();
RecurringJob.AddOrUpdate(
    "execute-sql-command",
    () => service.ExecuteStoredProcedure(),
    "*/10 * * * * *"); 
RecurringJob.AddOrUpdate(
    "execute-sql-command2",
    () => service.TransactionProcesses(),
    "*/10 * * * * *"); 
app.Run();
