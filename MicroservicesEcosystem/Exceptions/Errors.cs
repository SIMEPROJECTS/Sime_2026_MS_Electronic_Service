using System;

namespace MicroservicesEcosystem.Exceptions
{
    public class Errors
    {
        public const string NoProductInKardex = "El producto no se encuentra en el registro";
        public const string KardexNotFound = "No existe un registro de kardex";
        public const string OrderAtendNotFound = "No existe esa orden de atencion";
        public const string ServerError = "Ocurrio un error por favor comuniquese con el departamento de sistemas";
        public const string ServiceTimeout = "Tiempo de la solicitud agotado. Por favor intente de nuevo";
        public const string ServiceUnknowError = "Ocurrió un error al conectarse al servidor. Por favor intente de nuevo";
        public const string AccessDenied = "Acceso denegado";
        public const string ServiceBadRequest = "No se pudo completar la solicitud, por favor intente de nuevo";
        public const string ServiceUnauthorized = "No esta autorizado a acceder al recurso solicitado";
        public const string SalesDocumentNotFound = "No se encontro el metedodo de pago";
        public const string OverpaidAmount = "El monto pagado excede al monto a pagar";
        public const string InvalidPaymentAmount = "El monto es invalido";
        public const string InvalidPaymentType = "El tipo de pago es invalido";
        public const string TurnDoesntExist = "El turno no existe";
        public const string PaymentError = "Error en el pago";
        public const string TypeError = "El tipo de documento no es válido";
        public const string DetailsError = "Para generar una factura debe tener items";
        public const string PaymentMethodsError = "Debe ingresar los metodo de pago";
        public const string AlreadyActiveTurn = "El usuario ya tiene un turno activo";
        public const string NoActiveTurn = "El usuario no tiene turno activo";
        public const string NoProducts= "Producto no encontrado";
        public const string NoClientRegister = "El Cliente No Se Encuentra Registrado";
        public const string NoPatientRegister = "El Paciente No Se Encuentra Registrado";

        public const string RequestError = "Error en consulta";

        public const string VoucherNotFound = "El voucher no ha sido encontrado";
        public const string TransferNotFound = "Transferencia no ha sido encontrada o ha sido anulada";
        public const string noPrinter = "La caja del usuario no tiene una impresora asignada";
        public const string TransferDuplicate = "La transferencia ya se encuentra registrada";
        public const string VoucherNotSigned = "El voucher no se pudo firmar";
        public const string InsufficientCreditBalance = "El saldo de crédito es insuficiente para realizar la operación";
        public const string PaymentMethodNotFound = "El método de pago no ha sido encontrado";  

        public static string BuildError(string error, params object[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            return string.Format(error, args);
        }
    }
}
