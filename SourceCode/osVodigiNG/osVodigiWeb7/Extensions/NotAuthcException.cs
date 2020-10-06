using System;
namespace osVodigiWeb7x.Exceptions
{
    [Serializable]
    public class NotAuthcException : Exception
    {
        public NotAuthcException(string exceptionMessage)
            : base(String.Format("NotAuthException {0} ", exceptionMessage))
        {
        }

        public NotAuthcException()
           : base(String.Format("NotAuthException. Not authenticate to access this page. Must Authenticate first"))
        {
        }

 

        public NotAuthcException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
