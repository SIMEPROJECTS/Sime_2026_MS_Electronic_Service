using MicroservicesEcosystem.Models.DTO;

namespace MicroservicesEcosystem.Clients.Models
{

    public class BusinessRequestInfo
    {
        public Guid Id { get; set; }
        public string Ruc { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string? Observation { get; set; }
        public string EmailLegalRepresentative { get; set; }
        public Guid IdDynamics { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public AgreementRequestInfo AgreementRequestInfo { get; set; }
    }
    public class AgreementRequestInfo
    {
        public Guid Id { get; set; }
        public int TimeCredit { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int ResultsDeliveryTime { get; set; }
        public string TypeResultsDelivery { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<AgreementCityRequestInfo>? AgreementCityRequestInfo { get; set; }
        public List<MedicalCheckRequesInfo>? MedicalCheckRequesInfos { get; set; }
    }

    public class AgreementCityRequestInfo
    {
        public Guid Id { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class MedicalCheckRequesInfo
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
    public class SendEmailMessageOTPRequest
    {
        public string Dni { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public SendEmailMessageOTPRequest(OtpRequestMessage otpRequestMessage, string otp)
        {

            Dni = otpRequestMessage.Dni;
            Name = otpRequestMessage.Name;
            Email = otpRequestMessage.Email;
            Otp = otp;
        }
    }

    public class SendSmsMessageRequest
    {
        public Guid TemplateId { get; set; }
        public string Dni { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Otp { get; set; }
        public string Broker { get; set; }

        public SendSmsMessageRequest() { }


        public SendSmsMessageRequest(OtpRequestSmsMessage otpRequestSmsMessage, string otp, Guid templateId)
        {
            TemplateId = templateId;
            Dni = otpRequestSmsMessage.Dni;
            Name = otpRequestSmsMessage.Name;
            Email = "";
            Phone = otpRequestSmsMessage.Phone;
            Otp = otp;
        }
        public SendSmsMessageRequest(OtpRequestMessage otpRequestMessage, string otp, Guid templateId)
        {
            TemplateId = templateId;
            Dni = otpRequestMessage.Dni;
            Name = otpRequestMessage.Name;
            Email = otpRequestMessage.Email;
            Phone = "";
            Otp = otp;
        }
      
    }
}
