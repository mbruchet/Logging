using System.Diagnostics;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Logging.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerce.Logging.Api
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly DiagnosticSource _diagnosticSource;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource)
        {
            _loggerFactory = loggerFactory;
            _diagnosticSource = diagnosticSource;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var settings = new LoggingSettings();
            Configuration.GetSection("Logging").Bind(settings);

            var options = Options.Create(settings);

            services.AddSingleton<ILoggingRepository>(provider => new LoggingRepository(settings.Repository.ProviderAssembly,
                new ConnectionOptions
                {
                    Provider = settings.Repository.ProviderType,
                    ConnectionString = settings.Repository.ConnectionString,
                }, _loggerFactory, _diagnosticSource, options));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
