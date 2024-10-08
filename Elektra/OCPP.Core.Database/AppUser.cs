using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public double Balance { get; set; }
        public string OTP { get; set; }
        public int CheckOTP { get; set; }
        public DateTime OTPTimer { get; set; }
        public string ImgUrl { get; set; }
        public bool IsBlocked { get; set; }
        public int Type { get; set; }
        public DateTime RegisteredTime { get; set; }
        public ChargeTag ChargeTag { get; set; }
    }
}
