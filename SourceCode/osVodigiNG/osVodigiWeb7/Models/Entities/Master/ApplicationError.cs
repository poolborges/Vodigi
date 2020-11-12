namespace osVodigiWeb7x.Models
{
    public class ApplicationError
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ErrorMessage { get; set; }
    }
}