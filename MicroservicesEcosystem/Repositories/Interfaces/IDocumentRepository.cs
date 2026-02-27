using MicroservicesEcosystem.Models;

namespace MicroservicesEcosystem.Repositories.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        public Task<List<Document>> GetDocumentsByTokenValidationId(Guid tokenValidationId);
        public Task<List<Document>> GetDocumentsByFilePath(string filePath);
    }
}
