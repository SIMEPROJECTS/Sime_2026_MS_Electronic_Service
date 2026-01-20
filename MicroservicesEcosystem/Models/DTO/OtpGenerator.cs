using System.Security.Cryptography;
using System.Text;

namespace MicroservicesEcosystem.Models.DTO
{
    public class OtpGenerator
    {
        public string Otp { get; set; }
        public string token { get; set; }

        public string GenerateOTP(string key, long counter, int digits)
        {
            byte[] counterBytes = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counterBytes); // Revert bytes if the architecture is little-endian
            }

            using (HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(counterBytes);

                int offset = hash[hash.Length - 1] & 0x0F; // Last 4 bits of the last byte
                int binaryCode = ((hash[offset] & 0x7F) << 24) |
                                  ((hash[offset + 1] & 0xFF) << 16) |
                                  ((hash[offset + 2] & 0xFF) << 8) |
                                  (hash[offset + 3] & 0xFF);

                string otp = (binaryCode % (int)Math.Pow(10, digits)).ToString().PadLeft(digits, '1');

                return otp;
            }
        }

       
        public bool ValidateOTP(string key, long counter, string otp)
        {
            string generatedOTP = GenerateOTP(key, counter, otp.Length);
            return generatedOTP == otp;
        }

        public Boolean VerifyPassword(String passwordRequest, String password)
        {
            return BCrypt.Net.BCrypt.Verify(passwordRequest, password);
        }

        public int RandomNumber()
        {
            Random random = new Random();
            int rn = 0;

            for (int i = 0; i < 6; i++)
            {
                rn = random.Next(1, 101);
            }

            return rn;
        }
    }
    public class OtpGeneratorOrder
    {
        public string? Otp { get; set; }
        public int OrderCode { get; set; }

        public Boolean VerifyPassword(String passwordRequest, String password)
        {
            return BCrypt.Net.BCrypt.Verify(passwordRequest, password);
        }
    }

}
