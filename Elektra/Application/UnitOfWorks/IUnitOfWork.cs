using OCPP.Core.Application.DTOs.CpImageDTOs;
using OCPP.Core.Application.Repositories;
using OCPP.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.UnitOfWorks
{
    public interface IUnitOfWork
    {
        
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

        Task<int> CommitAsync();
        void Commit();
    }
}
