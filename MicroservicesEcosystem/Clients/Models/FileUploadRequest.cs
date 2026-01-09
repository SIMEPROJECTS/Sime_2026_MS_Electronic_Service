namespace MicroservicesEcosystem.Models.DTO
{
    public class FileUploadRequest
    {
        public string Base64File { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
        public string? ContentType { get; set; } = default!;
    }
}
