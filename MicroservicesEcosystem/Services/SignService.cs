using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using MicroservicesEcosystem.Client.Internal.Interfaces;
using MicroservicesEcosystem.CustomDataTime;
using MicroservicesEcosystem.Exceptions;
using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Models.DTO;
using MicroservicesEcosystem.Repositories.Interfaces;
using MicroservicesEcosystem.Services.Interfaces;
using MicroservicesEcosystem.Types;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using ZXing;
using ZXing.Common;
using Document = MicroservicesEcosystem.Models.Document;
using Image = iText.Layout.Element.Image;

namespace MicroservicesEcosystem.Services
{
    public class SignService : ISignService
    {
        private readonly ITokenValidationService tokenValidationService;
        private readonly ITokenValidationRepository tokenValidationRepository;
        private readonly IConfiguration configuration;
        private readonly IDocumentRepository documentRepository;
        private readonly IMSIdentityClient mSIdentityClient;
        private readonly ISignatureRepository signatureRepository;
        private readonly HttpClient httpClient;
        public SignService(ITokenValidationService tokenValidationService, ITokenValidationRepository tokenValidationRepository, IConfiguration configuration,
            IDocumentRepository documentRepository, IMSIdentityClient mSIdentityClient, ISignatureRepository signatureRepository, HttpClient httpClient)
        {
            this.tokenValidationService = tokenValidationService;
            this.tokenValidationRepository = tokenValidationRepository;
            this.configuration = configuration;
            this.documentRepository = documentRepository;
            this.mSIdentityClient = mSIdentityClient;
            this.signatureRepository = signatureRepository;
            this.httpClient = httpClient;
        }

        public async Task<IActionResult> CreateFormDocument(DocumentRequest documentRequest)
        {
            TokenValidation tokenValidation = await tokenValidationRepository.GetTokenValidationByOrderAttentionId(int.Parse(documentRequest.orderAttentionId));
            if (tokenValidation == null) throw new ArgumentException(Errors.TokenValidationNotFound.ToString());
            Document document = new Document();
            document.Id = Guid.NewGuid();
            document.Type = documentRequest.type;
            document.FileUrl = configuration["MS_Internal:CUriIdentity"] + $"/api/user/medicalrecords/multimedia/fichasMedicas/formularios/F_{documentRequest.orderAttentionId}_M.pdf";
            document.Hash = await GetHashFromUrl(document.FileUrl);
            document.CreatedAt = DateTime.Now;
            document.Status = TypeStatus.PENDING.ToString();
            document.TokenValidationId = tokenValidation.Id;
            await documentRepository.Add(document);
            return await Task.FromResult(new OkObjectResult(new { Status = TypeStatus.SUCCESS.ToString() }));
        }

        public async Task<IActionResult> SignPatientForm(SignRequest signRequest)
        {
            await documentRepository.BeginTransactionAsync();
            try
            {
                TokenValidation tokenValidation =
                    await tokenValidationRepository.GetTokenValidationByOrderAttentionId(
                        int.Parse(signRequest.orderAttentionId));

                if (tokenValidation == null)
                    throw new ArgumentException(Errors.TokenValidationNotFound.ToString());

                if (!BCrypt.Net.BCrypt.Verify(signRequest.otp, tokenValidation.TokenValue))
                    throw new ArgumentException(Errors.InvalidOTP.ToString());

                List<Document> documents =
                    await documentRepository.GetDocumentsByTokenValidationId(tokenValidation.Id);

                foreach (var document in documents)
                {
                    using var response = await httpClient.GetAsync(
                        document.FileUrl,
                        HttpCompletionOption.ResponseContentRead);

                    response.EnsureSuccessStatusCode();

                    byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();

                    List<(float X, float Y)> posiciones =
                        document.Type == "AIG"
                            ? new() { (275f, 175f) }
                            : new() { (130f, 400f), (120f, 30f) };

                    string qrContent = $"Verificado por SIME - {signRequest.orderAttentionId} - {LocalDateTimeNow.Now()} -- " + BCrypt.Net.BCrypt.HashPassword($"Verificado por SIME - {signRequest.orderAttentionId} - {LocalDateTimeNow.Now()}");

                    byte[] pdfFirmado = SignPatient(
                        pdfBytes,
                        tokenValidation.Name,
                        posiciones,
                        false,
                        signRequest.orderAttentionId);

                    var fileUploadRequest = new FileUploadRequest
                    {
                        ContainerName = "medicalrecords",
                        FileName =
                            $"multimedia/fichasMedicas/formularios/F_{signRequest.orderAttentionId}_A.pdf",
                        Base64File = Convert.ToBase64String(pdfFirmado)
                    };

                    await mSIdentityClient.postFile(fileUploadRequest);

                    document.Status = TypeStatus.SIGNED.ToString();
                    document.SignedAt = DateTime.Now;
                    document.FileUrl = configuration["MS_Internal:CUriIdentity"] +
                                       $"/api/user/medicalrecords/multimedia/fichasMedicas/formularios/F_{signRequest.orderAttentionId}_A.pdf";
                    document.Hash = await GetHashFromUrl(document.FileUrl);

                    await documentRepository.Update(document);

                    var signature = new Signature(document, qrContent)
                    {
                        Type = TypeStatus.SIMPLE.ToString(),
                        DeviceInfo = ""
                    };

                    await signatureRepository.Add(signature);
                }

                await documentRepository.CommitTransactionAsync();

                return new OkObjectResult(new { Status = TypeStatus.SUCCESS.ToString() });
            }
            catch
            {
                await documentRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public static async Task<string> GetHashFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("La URL no puede ser nula o vacía.", nameof(url));

            try
            {
                using HttpClient httpClient = new HttpClient();
                using var stream = await httpClient.GetStreamAsync(url);
                using var sha256 = SHA256.Create();

                byte[] hashBytes = await sha256.ComputeHashAsync(stream);

                StringBuilder hashString = new StringBuilder(hashBytes.Length * 2);
                foreach (var b in hashBytes)
                {
                    hashString.Append(b.ToString("x2"));
                }

                return hashString.ToString();
            }
            catch (HttpRequestException ex)
            {
                throw new ArgumentException(Errors.HttpError.ToString());
            }
            catch (CryptographicException ex)
            {
                throw new ArgumentException(Errors.HashError.ToString());
            }
        }

        private byte[] SignPatient(byte[] pdfBytes, string nombreFirmante, List<(float X, float Y)> posiciones, bool firmarTodasPaginas, string qrContent)
        {
            using var pdfStream = new MemoryStream(pdfBytes);
            using var outputStream = new MemoryStream();

            var reader = new PdfReader(pdfStream);
            var writer = new PdfWriter(outputStream);
            var pdfDoc = new PdfDocument(reader, writer);

            int totalPages = pdfDoc.GetNumberOfPages();
            var fechaActual = DateTime.Now;

            // ======================================================
            // 🔹 GENERAR QR SIN System.Drawing (MULTIPLATAFORMA)
            // ======================================================
            var qrWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 100,
                    Width = 100,
                    Margin = 0
                }
            };

            var pixelData = qrWriter.Write(qrContent);

            byte[] qrBytes;
            using (var qrStream = new MemoryStream())
            using (var image = new Image<Rgba32>(pixelData.Width, pixelData.Height))
            {
                int index = 0;

                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var row = accessor.GetRowSpan(y);
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new Rgba32(
                                pixelData.Pixels[index + 2], // R
                                pixelData.Pixels[index + 1], // G
                                pixelData.Pixels[index],     // B
                                pixelData.Pixels[index + 3]  // A
                            );
                            index += 4;
                        }
                    }
                });

                image.Save(qrStream, new PngEncoder());
                qrBytes = qrStream.ToArray();
            }

            // ======================================================
            // 🔹 DEFINIR PÁGINAS A FIRMAR
            // ======================================================
            var paginasAFirmar = new List<int>();
            if (firmarTodasPaginas)
            {
                for (int i = 1; i <= totalPages; i++)
                    paginasAFirmar.Add(i);
            }
            else
            {
                paginasAFirmar.Add(totalPages);
            }

            // ======================================================
            // 🔹 INSERTAR FIRMA + TEXTO + QR
            // ======================================================
            foreach (int pageNum in paginasAFirmar)
            {
                var page = pdfDoc.GetPage(pageNum);
                var pdfCanvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page);
                var canvas = new Canvas(pdfCanvas, page.GetPageSize());

                foreach (var (posX, posY) in posiciones)
                {
                    string textoFirma =
                        $"{nombreFirmante}\n{fechaActual:dd/MM/yyyy HH:mm}\nVerificado por SIME";

                    var texto = new Paragraph(textoFirma)
                        .SetFontSize(8)
                        .SetFixedPosition(pageNum, posX, posY + 5, 200);

                    var qrImage = new Image(ImageDataFactory.Create(qrBytes))
                        .ScaleAbsolute(40, 40)
                        .SetFixedPosition(pageNum, posX - 40, posY + 5);

                    canvas.Add(texto);
                    canvas.Add(qrImage);
                }

                canvas.Close();
            }

            pdfDoc.Close();
            return outputStream.ToArray();
        }
    }
}
