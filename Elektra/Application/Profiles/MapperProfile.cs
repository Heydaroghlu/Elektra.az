using AutoMapper;
using OCPP.Core.Application.DTOs.CallDTOs;
using OCPP.Core.Application.DTOs.CpImageDTOs;
using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Application.DTOs.CStatusDTOs;
using OCPP.Core.Application.DTOs.LocationDTOs;
using OCPP.Core.Application.DTOs.UserDTOs;
using OCPP.Core.Application.DTOs.WishListDTOs;
using OCPP.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCPP.Core.Application.DTOs.PaymentDTOs;
using OCPP.Core.Application.DTOs.ReservDTOs;
using OCPP.Core.Application.DTOs.TarifDTOs;
using OCPP.Core.Application.DTOs.TransactionDTOs;
using OCPP.Core.Application.DTOs.VersionDTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCPP.Core.Application.Profiles
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterDTO, AppUser>().ReverseMap();
            CreateMap<ChargepointPostDTO, ChargePoint>().ReverseMap();
            CreateMap<LocationPostDTO, ChargeLocation>().ReverseMap();
            CreateMap<AppUser,UserDTO>().ReverseMap();
            CreateMap<UserEditDTO,AppUser>().ReverseMap();
            CreateMap<ChargeLocation, LocationReturnDTO>();
            CreateMap<Wishlist, WishListDTO>();
            CreateMap<ChargePoint,CReturnOtherDTO>();
            CreateMap<CpImageDTO, CpImage>().ReverseMap();
            CreateMap<ChargePoint, CpReturnDTO>();
            CreateMap<CpImage, CpImageDTO>();
            CreateMap<ChargeLocation, LocationForMapDTO>();
            CreateMap<ChargePoint, CpForStatusDTO>();
            CreateMap<KeyValueDTO, UrlData>();
            CreateMap<ConnectorStatus, ReservsDTO>();
            CreateMap<Transaction, TransactionDTO>();
            CreateMap<ConnectorStatus,ConnectorDTO>();
            CreateMap<AppUser, UserUpDTO>();
            CreateMap<Transaction, TransactionPostDTO>();
            CreateMap<ConnectorLog, ReservPostDTO>();
            CreateMap<Transaction, TransactionPostDTO>();
            CreateMap<ConnectorLog, ReservPostDTO>();
            CreateMap<PaymentStatusDTO, PaymentStatus>();
            CreateMap<PaymentStatus, PaymentStatusReturnDTO>();
            CreateMap<AppUser, UserReturnDTO>();
            CreateMap<PaymentLogDTO, PaymentLog>();
            CreateMap<PaymentLog, PaymentLogReturnDTO>();
            CreateMap<TarifPostDTO, Tarif>();
            CreateMap<Tarif, TarinfReturnDTO>();
            CreateMap<ChargeTag, ChargeTagReturnDTO>();
            CreateMap<Transaction, TransactionDTO>();
            CreateMap<ChargeTag, TagReturnDTO>();
            CreateMap<VersionPostDTO, VersionHistory>();
        }
    }
}
