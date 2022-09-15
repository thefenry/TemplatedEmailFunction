namespace EmailService.Models
{
    public class EmailTemplateRequest
    {
        public string Data { get; set; }

        public string DataType { get; set; }

        public EmailData EmailData { get; set; }
    }
}
