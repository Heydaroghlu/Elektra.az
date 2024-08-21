using OCPP.Core.Application.Repositories;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCPP.Core.Persistence.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly OCPPCoreContext _context;

        public UnitOfWork(OCPPCoreContext context)
        {
            _context = context;
            RepositoryUser = new Repository<AppUser>(_context);
            RepositoryChargePoint = new Repository<ChargePoint>(_context);
            RepositoryConnectorStatus=new Repository<ConnectorStatus>(_context);
            RepositoryChargeLocations=new Repository<ChargeLocation>(context);
            RepositoryWishlist=new Repository<Wishlist>(_context);
            RepositoryCpImage=new Repository<CpImage>(_context);
            RepositoryPaymentSetting=new Repository<PaymentSetting>(_context);
            RepositoryConnectorLog =new Repository<ConnectorLog>(_context);
            RepositoryChargeConnector= new Repository<ChargeConnector>(_context);
            RepositoryUrl=new Repository<UrlData>(_context);
            RepositoryMessageLog=new Repository<MessageLog>(_context);
            RepositoryChargeTag= new Repository<ChargeTag>(_context);
            RepositoryTransaction=new Repository<Transaction>(_context);
        }

        public IRepository<AppUser> RepositoryUser { get; set; }
        public IRepository<ChargePoint> RepositoryChargePoint { get; set; }
        public IRepository<ConnectorStatus> RepositoryConnectorStatus { get; set; }
        public IRepository<ChargeLocation> RepositoryChargeLocations { get; set; }
        public IRepository<Wishlist> RepositoryWishlist { get; set; }
        public IRepository<CpImage> RepositoryCpImage { get; set; }
        public IRepository<PaymentSetting> RepositoryPaymentSetting { get; set; }
        public IRepository<ConnectorLog> RepositoryConnectorLog { get; set; }
        public IRepository<ChargeConnector> RepositoryChargeConnector { get; set; }
        public IRepository<UrlData> RepositoryUrl { get; set; }
        public IRepository<MessageLog> RepositoryMessageLog { get; set; }

        public IRepository<ChargeTag> RepositoryChargeTag { get; set; }
        public IRepository<Transaction> RepositoryTransaction { get; set; }





        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
        public void Commit() => _context.SaveChanges();

    }
}
