using EmailService.Models;
using Newtonsoft.Json;

namespace EmailService
{
    public class EmailManager
    {
        public string CreateEmailRequest<T>(T templateData, EmailData emailData)
        {
            EmailTemplateRequest content = new EmailTemplateRequest()
            {
                Data = JsonConvert.SerializeObject(templateData),
                DataType = typeof(T).Name,
                EmailData = emailData
            };

            return JsonConvert.SerializeObject(content);
        }
    }
}
