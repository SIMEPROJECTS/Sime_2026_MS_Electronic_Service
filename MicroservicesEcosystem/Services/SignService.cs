using iText.IO.Image;
using iText.Kernel.Pdf;
using MicroservicesEcosystem.Client.Internal.Interfaces;
using MicroservicesEcosystem.CustomDataTime;
using MicroservicesEcosystem.Exceptions;
using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Models.DTO;
using MicroservicesEcosystem.Repositories.Interfaces;
using MicroservicesEcosystem.Services.Interfaces;
using MicroservicesEcosystem.Types;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;

namespace MicroservicesEcosystem.Services
{
    public class SignService : ISignService
    {
        private readonly ITokenValidationService tokenValidationService;
        private readonly ITokenValidationRepository tokenValidationRepository;
        private readonly IConfiguration configuration;
        private readonly IDocumentRepository documentRepository;
        private readonly IMSIdentityClient mSIdentityClient;
        public SignService(ITokenValidationService tokenValidationService, ITokenValidationRepository tokenValidationRepository, IConfiguration configuration,
            IDocumentRepository documentRepository, IMSIdentityClient mSIdentityClient)
        {
            this.tokenValidationService = tokenValidationService;
            this.tokenValidationRepository = tokenValidationRepository;
            this.configuration = configuration;
            this.documentRepository = documentRepository;
            this.mSIdentityClient = mSIdentityClient;
        }

        public async Task<IActionResult> CreateFormDocument(DocumentRequest documentRequest)
        {
            TokenValidation tokenValidation = await tokenValidationRepository.GetTokenValidationByOrderAttentionId(int.Parse(documentRequest.idOrderAttention));
            if (tokenValidation == null) throw new ArgumentException(Errors.TokenValidationNotFound.ToString());
            Document document = new Document();
            document.Id = Guid.NewGuid();
            document.Type = documentRequest.type;
            document.FileUrl = configuration["MS_Internal:CuriIdentity"] + $"/api/user/medicalrecords/multimedia/fichasMedicas/formularios/F_{documentRequest.idOrderAttention}_M.pdf";
            document.Hash = await GetHashFromUrl(document.FileUrl);
            document.CreatedAt = DateTime.Now;
            document.Status = TypeStatus.PENDING.ToString();
            document.TokenValidationId = tokenValidation.Id;
            await documentRepository.Add(document);
            return await Task.FromResult(new OkObjectResult(new { Status = TypeStatus.SUCCESS.ToString() }));
        }

        public async Task<IActionResult> SignPatientForm(SignRequest signRequest)
        {
            TokenValidation tokenValidation = await tokenValidationRepository.GetTokenValidationByOrderAttentionId(int.Parse(signRequest.orderAttentionId));
            if (tokenValidation == null) throw new ArgumentException(Errors.TokenValidationNotFound.ToString());
            List<Document> documents = await documentRepository.GetDocumentsByTokenValidationId(tokenValidation.Id);
            OtpGenerator otpGenerator = new OtpGenerator();
            otpGenerator.Otp = signRequest.otp;
            otpGenerator.token = tokenValidation.TokenValue;
            await tokenValidationService.ValidarOTP(otpGenerator);

            foreach(var document in documents)
            {
                byte[] pdfBytes;
                using (var http = new HttpClient())
                    pdfBytes = await http.GetByteArrayAsync(document.FileUrl);

                byte[] pdfFirmado = SignPatient(pdfBytes, tokenValidation.Name, new List<(float X, float Y)> { (270, 140) }, false, signRequest.orderAttentionId);

                string pdfBase64 = Convert.ToBase64String(pdfFirmado);
                FileUploadRequest fileUploadRequest = new FileUploadRequest
                {
                    ContainerName = "medicalrecords",
                    FileName = $"multimedia/fichasMedicas/formularios/F_{signRequest.orderAttentionId}_A.pdf",
                    Base64File = pdfBase64,
                };
                await mSIdentityClient.postFile(fileUploadRequest);
            }
            return await Task.FromResult(new OkObjectResult(new { Status = TypeStatus.SUCCESS.ToString() }));
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

        private byte[] SignPatient(byte[] pdfBytes, string nombreFirmante, List<(float X, float Y)> posiciones, bool firmarTodasPaginas, string idOrdenAtencion)
        {
            using (var pdfStream = new MemoryStream(pdfBytes))
            using (var outputStream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(pdfStream);
                PdfWriter writer = new PdfWriter(outputStream);
                PdfDocument pdfDoc = new PdfDocument(reader, writer);

                int totalPages = pdfDoc.GetNumberOfPages();

                // Obtenemos la fecha actual una sola vez para que coincida en el QR y el texto
                var fechaActual = DateTime.Now; // O usa LocalDateTimeNow.Now() si es tu clase personalizada

                // Texto del QR (Mantenemos tu lógica)
                string qrContent = $"Verificado por SIME - {idOrdenAtencion} - {LocalDateTimeNow.Now()}";

                // --- Generación del QR (Igual que antes) ---
                var barcodeWriter = new ZXing.BarcodeWriterPixelData
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 85,
                        Width = 85,
                        Margin = 0
                    }
                };

                var pixelData = barcodeWriter.Write(qrContent);
                using var qrBitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
                var bitmapData = qrBitmap.LockBits(new Rectangle(0, 0, qrBitmap.Width, qrBitmap.Height), ImageLockMode.WriteOnly, qrBitmap.PixelFormat);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    qrBitmap.UnlockBits(bitmapData);
                }

                using var msQr = new MemoryStream();
                qrBitmap.Save(msQr, ImageFormat.Png);
                byte[] qrBytes = msQr.ToArray();
                // ------------------------------------------

                // Definir qué páginas se firman
                var paginasAFirmar = new List<int>();
                if (firmarTodasPaginas)
                {
                    for (int i = 1; i <= totalPages; i++) paginasAFirmar.Add(i);
                }
                else
                {
                    paginasAFirmar.Add(totalPages); // Solo la última
                }

                // Iterar sobre las páginas seleccionadas
                foreach (int pageNum in paginasAFirmar)
                {
                    PdfPage page = pdfDoc.GetPage(pageNum);
                    var pdfCanvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page);
                    var canvas = new iText.Layout.Canvas(pdfCanvas, page.GetPageSize());

                    foreach (var (posX, posY) in posiciones)
                    {
                        // 1. Crear el bloque de TEXTO (Nombre, Fecha, Mensaje)
                        // Usamos Paragraph para manejar saltos de línea
                        string textoFirma = $"{nombreFirmante}\n{fechaActual:dd/MM/yyyy HH:mm}\nVerificado por SIME";

                        var parrafoFirma = new iText.Layout.Element.Paragraph(textoFirma)
                            .SetFontSize(8) // Tamaño pequeño para que parezca firma/sello
                            .SetFixedPosition(pageNum, posX, posY + 5, 200); // 200 es el ancho máximo de la caja de texto

                        // 2. Crear la imagen del QR
                        var qrImg = new iText.Layout.Element.Image(ImageDataFactory.Create(qrBytes))
                            .ScaleAbsolute(50, 50)
                            // Ajustamos la posición del QR. 
                            // Nota: Si quieres el QR a la IZQUIERDA del texto, restamos a X.
                            // Si el texto empieza en posX, el QR lo ponemos un poco antes.
                            .SetFixedPosition(pageNum, posX - 55, posY);

                        canvas.Add(parrafoFirma);
                        canvas.Add(qrImg);
                    }

                    canvas.Close();
                }

                pdfDoc.Close();
                return outputStream.ToArray();
            }
        }
    }
}
