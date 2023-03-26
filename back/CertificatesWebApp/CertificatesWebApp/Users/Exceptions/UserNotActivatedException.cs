namespace CertificatesWebApp.Users.Exceptions
{
    public class UserNotActivatedException : Exception
    {
        public UserNotActivatedException(string message) : base(message)
        {

        }
    }
}
