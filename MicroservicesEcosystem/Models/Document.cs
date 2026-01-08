using MicroservicesEcosystem.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;
using System.Security.Principal;

namespace MicroservicesEcosystem.Models
{
    [Table("document", Schema = "sgn")]
    public class Document : IEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("type")]
        public string Type { get; set; }

        [Required]
        [Column("file_url")]
        public string FileUrl { get; set; }

        [Required]
        [Column("hash")]
        public string Hash { get; set; }

        [Column("created_by_msIdentity_user_id")]
        public Guid? CreatedByMsIdentityUserId { get; set; }

        [Column("created_by_msIdentity_user_dni")]
        public string? CreatedByMsIdentityUserDni { get; set; }

        [Column("created_by_msIdentity_user_name")]
        public string? CreatedByMsIdentityUserName { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Required]
        [Column("tokenValidation_id")]
        public Guid TokenValidationId { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; }

        [Column("signed_at")]
        public DateTime? SignedAt { get; set; }
       
    }
}
