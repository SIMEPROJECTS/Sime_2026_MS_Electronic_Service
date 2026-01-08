using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Repositories.Interfaces;

namespace MicroservicesEcosystem.Repositories
{
    public class TokenValidationRepository : Repository<TokenValidation>, ITokenValidationRepository
    {
        public TokenValidationRepository(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor) : base(serviceProvider, httpContextAccessor)
        {
        }
    }
}
