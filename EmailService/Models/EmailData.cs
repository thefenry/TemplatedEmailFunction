using System.Collections.Generic;

namespace EmailService.Models
{
    public class EmailData
    {
        public EmailData(EmailAdressInfo from, List<EmailAdressInfo> recipients, List<EmailAttachment>? attachments, EmailAdressInfo? bcc = null)
        {
            From = from;
            Recipients = recipients;
            Bcc = bcc;
            Attachments = attachments;
        }

        public EmailAdressInfo From { get; set; }

        public List<EmailAdressInfo> Recipients { get; set; }

        public EmailAdressInfo? Bcc { get; internal set; }

        public string? BodyHtml { get; set; }

        public List<EmailAttachment>? Attachments { get; set; }
    }
}
