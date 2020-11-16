using System.ComponentModel.DataAnnotations.Schema;

namespace osVodigiWeb7x.Models
{
    //[Table("Account", Schema="master")]
    [Table("Account")]
    public class Account
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public string FTPServer { get; set; }
        public string FTPUsername { get; set; }
        public string FTPPassword { get; set; }
        public bool IsActive { get; set; }
    }
}