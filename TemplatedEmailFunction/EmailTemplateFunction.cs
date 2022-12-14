using EmailService.Models;
using EmailTemplates.Services;
using EmailTemplates.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TemplatedEmailFunction
{
    public class EmailTemplateFunction
    {
        private readonly Services.EmailService _emailService;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public EmailTemplateFunction(Services.EmailService emailService, IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _emailService = emailService;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        [FunctionName("SendEmailTemplateHttp")]
        public async Task<IActionResult> SendEmailTemplateHttp(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //TODO: Make it so we can get a sample payload

            return await SendEmail(req, log);
        }

        [FunctionName("GetSampleEmailTemplate")]
        public static IActionResult GetSampleEmailTemplate(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //TODO: Make it so we can get a sample payload
            var emailManager = new EmailService.EmailManager();

            var from = new EmailAdressInfo("JSmith@example.com") { Name = "John Smith" };
            var recipients = new List<EmailAdressInfo> { new EmailAdressInfo("JDoe@example.com") { Name = "Jane Doe" } };

            var newEmailData = new EmailData(from, recipients, null);

            var emailTemplateData = new BasicEmailTemplateModel
            {
                Name = "Peter Framptom"
            };

            var emailTemplateSample = emailManager.CreateEmailRequest(emailTemplateData, newEmailData);

            return new OkObjectResult(emailTemplateSample);
        }

        private async Task<IActionResult> SendEmail(HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var data = JsonConvert.DeserializeObject<EmailTemplateRequest>(requestBody);

            await _emailService.SendEmail(data);

            return new OkObjectResult($"Email successfully sent to the following: {string.Join(",", data.EmailData.Recipients.Select(r => r.Email))}");
        }
    }
}
