namespace TP4SCS.Library.Utils.Healpers
{
    public class EmailOptions
    {
        public string SmtpServer { get; set; } = null!;

        public int SmtpPort { get; set; }

        public string SenderEmail { get; set; } = null!;

        public string SenderPassword { get; set; } = null!;
    }

}
