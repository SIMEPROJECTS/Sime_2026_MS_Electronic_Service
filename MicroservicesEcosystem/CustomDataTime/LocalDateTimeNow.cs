using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesEcosystem.CustomDataTime
{
    public class LocalDateTimeNow
    {
        public static DateTimeOffset NowOffset()
        {
            DateTimeOffset localTime;
            localTime = new DateTimeOffset(DateTime.Now);
            return localTime;
        }
        public static DateTime Now()
        {
            var otherTime = NowOffset();
            return otherTime.DateTime;
        }
        public static String FormatDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
        public static String UpdatedFormatDate(DateTime? dateTime)
        {
            return dateTime?.ToString("yyyy-MM-dd");
        }
        public static String FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd 00:00:00");
        }

        public static int calculateAge(DateTime dateTime)
        {
            DateTime fechaActual = DateTime.Today;
            int edad = fechaActual.Year - dateTime.Year;
            return edad;
        }

        public static DateTime FormatDateLatina(string dateTime)
        {
            DateTime latinaFechaVigFin;
            if (DateTime.TryParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out latinaFechaVigFin))
            {
               return latinaFechaVigFin;
            }
            return latinaFechaVigFin;
        }


    }
}
