using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace MicroservicesEcosystem.Request
{
    public class Header
    {
        public static string GetHeader(string headerName, HttpContext request)
        {
            StringValues header = "";
            request.Request.Headers.TryGetValue(headerName, out header);
            return header.ToString().Trim();
        }
    }
}
