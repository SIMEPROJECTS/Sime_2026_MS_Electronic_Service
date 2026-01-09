

using MicroservicesEcosystem.Authentication;
using MicroservicesEcosystem.Clients.Internal;
using MicroservicesEcosystem.Clients.Internal.Interface;
using MicroservicesEcosystem.Repositories;
using MicroservicesEcosystem.Repositories.Interfaces;
using MicroservicesEcosystem.Services;
using MicroservicesEcosystem.Services.Interfaces;

namespace MicroservicesEcosystem.DependyInjection
{
    public class Repository
    {
        public static void Inject(IServiceCollection services)
        {
            services.AddScoped<ITokenValidationRepository, TokenValidationRepository>();
            services.AddScoped<ITokenValidationService, TokenValidationService>();
            services.AddScoped<IMSMessagesClient, MSMessagesClient>();            
        }
    }
}
