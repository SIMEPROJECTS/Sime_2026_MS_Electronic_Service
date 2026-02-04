namespace MicroservicesEcosystem.Models.DTO
{
    public class SignRequest
    {
        public string otp { get; set; }
        public string orderAttentionId { get; set; } //OrderAttentionCode in new system
    }
}
