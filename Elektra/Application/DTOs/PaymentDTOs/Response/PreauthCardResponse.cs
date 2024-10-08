namespace OCPP.Core.Application.DTOs.PaymentDTOs.Response;

public class PreauthCardResponse
{

        public string StatusCode { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string BankOrderID { get; set; }
        public string BankSessionID { get; set; }
        public string ClientOrderId { get; set; }
        public int TransactionId { get; set; }

}