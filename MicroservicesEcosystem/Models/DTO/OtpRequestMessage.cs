namespace MicroservicesEcosystem.Models.DTO
{
    public class OtpRequestMessage
    {
        public string Dni { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? OrderAttentionId { get; set; }
        public string? Type { get; set; }

    }
    public class OtpRequestSmsMessage
    {
        public string Dni { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int? OrderAttentionId { get; set; }
        public string? Type { get; set; }

    }
    public class OtpRequestSmsEmailMessage
    {
        public string Dni { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid Broker { get; set; }
        public int? OrderAttentionId { get; set; }
        public string? Type { get; set; }
    }
}
