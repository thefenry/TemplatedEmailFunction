using EmailService.Models;
using EmailTemplates.Helpers;
using EmailTemplates.Services;
using EmailTemplates.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TemplatedEmailFunction.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _log;
        private readonly IRazorViewToStringRenderer _templateCompilerService;

        public EmailService(ILogger<EmailService> log, IRazorViewToStringRenderer templateCompilerService)
        {
            _log = log;
            _templateCompilerService = templateCompilerService;
        }

        /// <summary>
        /// Send email using the request email template
        /// </summary>
        /// <param name="emailRequest">Contains viewModel, dataType, emailConfig</param>
        public async Task SendEmail(EmailTemplateRequest emailRequest)
        {
            try
            {
                BaseEmailClass deserializedModel = TemplateDeserializer.GetEmailData(emailRequest.Data, emailRequest.DataType);

                string htmlContent = await _templateCompilerService.RenderViewToStringAsync(emailRequest.DataType, deserializedModel);

                if (string.IsNullOrWhiteSpace(htmlContent))
                {
                    var errorMessage = $"Something went wrong. No Email template was generated for data type: {emailRequest.DataType}";
                    _log.LogError(errorMessage);
                    throw new NullReferenceException(errorMessage);
                }

                _log.LogInformation($"Email template was generated for {emailRequest.DataType}");

                emailRequest.EmailData.BodyHtml = htmlContent;
                //TODO: Add Email Client       await _emailClient.SendEmailAsync(emailRequest.EmailData);
            }
            catch (Exception ex)
            {
                _log.LogError($"An Error occurred when email was sent. {ex.Message}");
                throw;
            }
        }
    }
}
