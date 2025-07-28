namespace MyBudgetTracker.Models
{
    public class MailSettings
    {
        public string? FromAddress { get; set; }
        public string? FromName { get; set; }
        public string? ToAddress { get; set; }
        public string? ToName { get; set; }
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
    }
}
