using MicroservicesEcosystem.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesEcosystem.Services.Interfaces
{
    public interface ISignService
    {
        public Task<IActionResult> SignPatientForm(SignRequest signRequest);
        public Task<IActionResult> CreateFormDocument(DocumentRequest documentRequest);
        public Task<IActionResult> SignMedicalRecordDocument(MedicalRecordDocumentRequest documentRequest);
        public Task<IActionResult> SignPatientFormV2(SignFormRequest signFormRequest);
    }
}
