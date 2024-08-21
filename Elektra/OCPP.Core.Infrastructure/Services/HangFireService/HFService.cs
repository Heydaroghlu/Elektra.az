using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Infrastructure.Services.HangFireService
{
    public class StoredProcedureService
    {
        private readonly IConfiguration _configuration;

        public StoredProcedureService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ExecuteStoredProcedure()
        {
            var connectionString = _configuration.GetConnectionString("SqlServer");
            using (var connection = new SqlConnection(connectionString))
            {
                var sql = @"
               DECLARE @CurrentTime DATETIME = DATEADD(HOUR, 4, GETUTCDATE());
Update ConnectorStatus set ReservUser=NULL, LastStatus='Available',ReservTime=NULL, ReservMinute=0 Where 
ReservTime<@CurrentTime and LastStatus='Reserved'";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
    }
}
