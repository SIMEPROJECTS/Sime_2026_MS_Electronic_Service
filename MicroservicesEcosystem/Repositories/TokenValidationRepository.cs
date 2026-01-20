using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Repositories.Interfaces;
using MicroservicesEcosystem.Types;
using Microsoft.EntityFrameworkCore;

namespace MicroservicesEcosystem.Repositories
{
    public class TokenValidationRepository : Repository<TokenValidation>, ITokenValidationRepository
    {
        public TokenValidationRepository(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor) : base(serviceProvider, httpContextAccessor)
        {
        }

        public async Task<TokenValidation> GetTokenValidationByOrderAttentionId(int orderAttentionId)
        {
            var result = await (from db in context.Set<TokenValidation>()
                                where db.MsMedicalRecordOrderAttentionCode == orderAttentionId & (db.Status == TypeStatus.USADO.ToString() || db.Status == TypeStatus.ENVIADO.ToString())
                                select db).FirstOrDefaultAsync();
            return result;
        }

       
    }
}
