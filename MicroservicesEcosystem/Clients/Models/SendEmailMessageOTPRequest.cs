using MicroservicesEcosystem.Models.DTO;

namespace MicroservicesEcosystem.Clients.Models
{
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
