using MicroservicesEcosystem.Repositories.Interfaces;
using MicroservicesEcosystem.Services.Interfaces;

namespace MicroservicesEcosystem.Services
{
    public class TokenValidationService : ITokenValidationService
    {
        private readonly ITokenValidationRepository tokenValidationRepository;

        public TokenValidationService(ITokenValidationRepository tokenValidationRepository)
        {
            this.tokenValidationRepository = tokenValidationRepository;
        }
    }
}
