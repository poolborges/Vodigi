using System;
namespace osVodigiWeb6x.Exceptions
{
    [Serializable]
    public class NotAuthzException : Exception
    {
        public NotAuthzException(string requiredRole)
            : base(String.Format("Not have authorization to access this page. Dont have role {0} ", requiredRole))
        {
        }

        public NotAuthzException()
           : base(String.Format("Not have authorization to access this page. Must have correct role"))
        {
        }

 

        public NotAuthzException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
