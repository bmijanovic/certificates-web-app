namespace CertificatesWebApp.Users.Exceptions
{
    public class BadCredentialsException : Exception
    {
        public BadCredentialsException(string message) : base(message)
        {

        }
    }
}
