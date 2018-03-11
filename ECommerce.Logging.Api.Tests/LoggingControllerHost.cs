using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.TestHost;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Logging.Api.Tests
{
    public class LoggingControllerHost
    {
        public TestServer Server { get; }

        public LoggingControllerHost()
        {
            var applicationBasePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\ECommerce.Logging.Api"));

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var builder = WebHost.CreateDefaultBuilder().UseContentRoot(applicationBasePath)
                .UseKestrel()
                .UseContentRoot(applicationBasePath)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(
                        new LoggerFactory()
                            .AddConsole()
                    );

                    services.AddLogging();

                    services.Configure((RazorViewEngineOptions options) =>
                    {
                        var previous = options.CompilationCallback;
                        options.CompilationCallback = (context) =>
                        {
                            previous?.Invoke(context);
                            var assembly = typeof(Startup).GetTypeInfo().Assembly;
                            var assemblies = assembly.GetReferencedAssemblies().Select(x => MetadataReference.CreateFromFile(Assembly.Load(x).Location))
                            .ToList();

                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard, Version = 2.0.0.0, Culture = neutral, PublicKeyToken = cc7b13ffcd2ddd51")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Threading.Tasks")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Dynamic.Runtime")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor.Runtime")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Razor")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Html.Abstractions")).Location));
                            assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text.Encodings.Web")).Location));
                            context.Compilation = context.Compilation.AddReferences(assemblies);
                        };
                    });
                })
                .UseStartup<Startup>();

            Server = new TestServer(builder);
        }
    }
}
