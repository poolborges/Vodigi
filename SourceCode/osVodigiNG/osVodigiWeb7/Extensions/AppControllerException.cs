using System;
namespace osVodigiWeb7x.Exceptions
{
    [Serializable]
    public class AppControllerException : Exception
    {
        public AppControllerException(string controller, string action, string exceptionMessage)
            : base(String.Format("Controller: {0} / Action: {1} with message: {2}", controller, action, exceptionMessage))
        {
        }

        public AppControllerException(string controller, string action, Exception innerException)
            : this(String.Format("Controller: {0} / Action: {1}", controller, action), innerException)
        {
        }

        public AppControllerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
