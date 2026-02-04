namespace MicroservicesEcosystem.Models.DTO
{
    public class MedicalRecordDocumentRequest
    {
        public string Dni { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public List<DocumentSigns> Documents {  get; set; }
    }

    public class DocumentSigns
    {
        public string filePath { get; set; }
        public List<SignPosition> signPositions { get; set; }
    }

    public class SignPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int PageNumber { get; set; }
    }
}
