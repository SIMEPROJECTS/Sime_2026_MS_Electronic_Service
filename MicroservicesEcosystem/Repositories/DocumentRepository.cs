using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroservicesEcosystem.Repositories
{
    public class DocumentRepository : Repository<Document>, IDocumentRepository
    {
        public DocumentRepository(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor) : base(serviceProvider, httpContextAccessor)
        {
        }

        public async Task<List<Document>> GetDocumentsByTokenValidationId(Guid tokenValidationId)
        {
            var result = await (from db in context.Set<Document>()
                                where db.TokenValidationId == tokenValidationId
                                select db).ToListAsync();
            return result;
        }
    }
}
