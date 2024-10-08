using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Persistence.Services.Payment;

namespace OCPP.Core.Infrastructure.Services.HangFireService
{
    public class StoredProcedureService
    {
        private readonly IConfiguration _configuration;
        private readonly OCPPCoreContext _context;
        public IUnitOfWork _unitOfWork;
        public StoredProcedureService(IConfiguration configuration,OCPPCoreContext context,IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _context = context;
            _unitOfWork = unitOfWork;
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

        public void TransactionProcesses()
        {
            PaymentTrService service = new PaymentTrService(_unitOfWork);
            var transactions = _context.Transactions.Where(x => x.StopTime == null && x.IsPayment == false).Include(x=>x.StartTag).ToList();
            foreach (var transaction in transactions)
            {
                var user = _context.Users.Include(x => x.ChargeTag)
                    .Where(x => x.ChargeTag.TagId == transaction.StartTag.TagId).FirstOrDefault();
                if (user != null && !user.ChargeTag.WithBalance)
                {
                    PaymentLog paymentLog = _context.PaymentLogs
                        .Where(x => x.AppUserId == user.Id)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();  

                     if (paymentLog != null)
                     {
                         transaction.CardUID = Convert.ToInt32(paymentLog.TransactionId);
                         transaction.Uid = "TransactionProcesses";
                     }
                }
            }
            _context.SaveChanges();
        }
    }
}
