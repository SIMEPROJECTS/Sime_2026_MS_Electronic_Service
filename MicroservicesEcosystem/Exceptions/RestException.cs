using System;
namespace MicroservicesEcosystem.Exceptions
{
    public class RestException : Exception
    {
        public RestException(string message) : base(message)
        {

        }
        public RestException()
        {

        }
    }
}
