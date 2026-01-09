using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MicroservicesEcosystem.Client.Internal.Interfaces;
using MicroservicesEcosystem.Clients.Internal.Interface;
using MicroservicesEcosystem.Clients.Models;
using MicroservicesEcosystem.Exceptions;
using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Models.DTO;
using MicroservicesEcosystem.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;

namespace MicroservicesEcosystem.Clients.Internal
{
    public class MsIdentityClient : Client, IMSIdentityClient
    {

        private const string uriClient = "MS_Internal:CUriIdentity";

        public MsIdentityClient(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor, ILogger<MsIdentityClient> _logger) : base(Configuration, httpContextAccessor, _logger, uriClient)
        {


        }

        public async Task postFile(FileUploadRequest fileUploadRequest)
        {
            var jsonData = JsonConvert.SerializeObject(fileUploadRequest, Formatting.Indented);
            await MakeRequest(this.HttpClient, UrlClients.postFile, HttpMethod.Post, jsonData);

        }

    }
}