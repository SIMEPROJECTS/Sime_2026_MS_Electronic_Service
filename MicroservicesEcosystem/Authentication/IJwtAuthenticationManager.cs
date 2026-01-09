using MicroservicesEcosystem.Authentication.Models;
using System.Collections.Generic;


namespace MicroservicesEcosystem.Authentication
{
    public interface IJwtAuthenticationManager
    {
        TokenResponse AuthenticateOTPInsurance(string otp);     
        TokenResponse AuthenticateOTP(Guid id);

    }
}
