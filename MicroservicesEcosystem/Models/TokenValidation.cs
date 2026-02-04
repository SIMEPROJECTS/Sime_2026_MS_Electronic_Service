using MicroservicesEcosystem.Models.DTO;
using MicroservicesEcosystem.Repositories.Interfaces;
using MicroservicesEcosystem.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroservicesEcosystem.Models
{
    [Table("tokenValidation", Schema = "tkn")]
    public class TokenValidation : IEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("type")]     
        public string Type { get; set; }

        [Required]
        [Column("token_value")]      
        public string TokenValue { get; set; }

        [Required]
        [Column("status")]     
        public string Status { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("msMedicalRecord_orderAttention_code")]
        public int? MsMedicalRecordOrderAttentionCode { get; set; }

        [Required]
        [Column("name")]      
        public string Name { get; set; }

        [Required]
        [Column("dni")]     
        public string Dni { get; set; }

        [Required]
        [Column("email")]     
        public string Email { get; set; }

        [Required]
        [Column("phone")]
        public string Phone { get; set; }

        [Column("token")]
        public string? Token { get; set; }

        public TokenValidation() { }

        public TokenValidation(MedicalRecordDocumentRequest medicalRecordDocumentRequest)
        {
            this.Id = Guid.NewGuid();
            this.Type = TypeStatus.SIGN.ToString();
            this.TokenValue = "N/A";
            this.Status = TypeStatus.ACTIVO.ToString();
            this.CreatedAt = DateTime.Now;
            this.ExpiresAt = DateTime.MaxValue;
            this.Name = medicalRecordDocumentRequest.Name;
            this.Dni = medicalRecordDocumentRequest.Dni;
            this.Email = medicalRecordDocumentRequest.Email;
            this.Phone = medicalRecordDocumentRequest.Phone;
        }
    }
}
