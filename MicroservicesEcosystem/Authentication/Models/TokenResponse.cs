using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesEcosystem.Authentication.Models
{
    public class TokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public DateTime ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        public TokenResponse() { }

        public TokenResponse(string token, DateTime expires,string tokenType)
        {
            this.AccessToken = token;
            this.ExpiresIn = expires;
            this.TokenType = tokenType;
            this.Scope = "openid profile";
        }
    }
   
}
