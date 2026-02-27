namespace MicroservicesEcosystem.Models.DTO
{
    public class SignMedicalRecordRequest
    {
        public string Dni { get; set; }
        public string OrderAttentionId { get; set; } //OrderAttentionCode in new System
    }
}
