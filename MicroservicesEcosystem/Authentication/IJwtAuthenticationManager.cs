using MicroservicesEcosystem.Authentication.Models;
using System.Collections.Generic;


namespace MicroservicesEcosystem.Authentication
{
    public interface IJwtAuthenticationManager
    {   
        TokenResponse AuthenticateOTP(Guid id, string otp);

    }
}
