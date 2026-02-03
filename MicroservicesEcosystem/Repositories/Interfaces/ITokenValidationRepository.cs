using MicroservicesEcosystem.Models;

namespace MicroservicesEcosystem.Repositories.Interfaces
{
    public interface ITokenValidationRepository : IRepository<TokenValidation>
    {
        public Task<TokenValidation> GetTokenValidationByOrderAttentionId(int orderAttentionId);
        public Task<TokenValidation> GetTokenValidationByDni (string dni);
    }
}
