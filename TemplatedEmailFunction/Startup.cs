using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using TemplatedEmailFunction;
using TemplatedEmailFunction.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.ObjectPool;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TemplatedEmailFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var emailTemplateProjectName = "EmailTemplates";
            Assembly compiledViewAssembly = Assembly.LoadFile(Path.Combine(
                executionPath ?? throw new InvalidOperationException(), $"{emailTemplateProjectName}.Views.dll"));

            ExecutionContextOptions contextualization = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;
            string currentDirectory = contextualization.AppDirectory;

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddLogging();

            builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
            builder.Services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            builder.Services.AddScoped<RazorViewTemplateCompilerService>();
            builder.Services.AddScoped<Services.EmailService>();

            builder.Services.AddMvcCore()
                .AddViews()
                .AddRazorViewEngine()
                .AddApplicationPart(compiledViewAssembly);
        }
    }
}
