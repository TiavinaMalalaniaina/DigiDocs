namespace BackOffice.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException()
        : base("Email ou mot de passe incorrect")
        {
        }

        public LoginFailedException(string message)
            : base(message)
        {
        }
    }
}
