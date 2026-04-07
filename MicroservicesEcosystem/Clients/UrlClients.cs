using MicroservicesEcosystem.Models.DTO;

namespace MicroservicesEcosystem.Clients
{
    public class UrlClients
    {
        public const string getBusinesinformation = "/api/business/information";
        public const string postEnvioOTPMessagePayment = "/api/messages/sms/otp/payment";
        public const string postEnvioOTPSmsMessage = "/api/messages/sms/otp";
        public const string postEnvioOTPSmsMessagePortal = "/api/messages/sms/otp/portal";
        public const string postEnvioOTPEmailMessagepPortals = "/api/messages/email/phone/otp/portal";
        public const string postEnvioOTPEmailMessage = "/api/messages/email/otp";
        public const string postEnvioOTPEmailMessagepPortal = "/api/messages/email/otp/portal";
        public const string postFile = "/api/user/file/upload";
        public const string postTimeLogs = "/api/medicalrecord/timeLogs";
    }
}
