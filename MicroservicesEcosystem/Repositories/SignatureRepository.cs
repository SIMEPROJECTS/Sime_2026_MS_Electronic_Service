using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Repositories.Interfaces;

namespace MicroservicesEcosystem.Repositories
{
    public class SignatureRepository : Repository<Signature>, ISignatureRepository
    {
        public SignatureRepository(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor) : base(serviceProvider, httpContextAccessor)
        {
        }
    }
}
