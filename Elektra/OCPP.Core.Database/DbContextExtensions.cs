﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public static class DbContextExtensions
    {
        public static void AddOCPPDbContext(this IServiceCollection services, IConfiguration configuration)
        {

            string sqlServerConnectionString = configuration.GetConnectionString("SqlServer");
            string sqliteConnectionString = configuration.GetConnectionString("SQLite");

            if (string.IsNullOrWhiteSpace(sqlServerConnectionString))
            {
                //services.AddDbContext<OCPPCoreContext>(options => options.UseSqlite(sqliteConnectionString));
            }
            else
            {
                services.AddDbContext<OCPPCoreContext>(options => options.UseSqlServer(sqlServerConnectionString));
            }
            
           
        }
    }
}
