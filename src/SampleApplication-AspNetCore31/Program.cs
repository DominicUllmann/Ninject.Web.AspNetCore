using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Ninject;
using Ninject.Web.AspNetCore.Hosting;
using Ninject.Web.Common.SelfHost;
using Publication.Infrastructure.Service;
using System;
using System.Linq;
using System.Reflection;

namespace SampleApplication_AspNetCore31
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Simple (and probably unreliable) IIS detection mechanism
			var model = Environment.GetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES") == "Microsoft.AspNetCore.Server.IISIntegration" ? "IIS" : null;
			// The hosting model can be explicitly configured with the SERVER_HOSTING_MODEL environment variable.
			// See https://www.andrecarlucci.com/en/setting-environment-variables-for-asp-net-core-when-publishing-on-iis/ for
			// setting the variable in IIS.
			model = Environment.GetEnvironmentVariable("SERVER_HOSTING_MODEL") ?? model;
			// Command line arguments have higher precedence than environment variables
			model = args.FirstOrDefault(arg => arg.StartsWith("--use"))?.Substring(5) ?? model;

			var hostConfiguration = new AspNetCoreHostConfiguration(args)
					.UseStartup<Startup>();

			switch (model)
			{
				case "Kestrel":
					hostConfiguration.UseKestrel();
					break;

				case "HttpSys":
					hostConfiguration.UseHttpSys();
					break;

				case "IIS":
				case "IISExpress":
					hostConfiguration.UseIIS();
					break;

				default:
					throw new ArgumentException($"Unknown hosting model '{model}'");
			}

			var host = new NinjectSelfHostBootstrapper(CreateKernel, hostConfiguration);
			host.Start();
		}

		public static IKernel CreateKernel()
		{
			var kernel = new StandardKernel();
			kernel.Load(Assembly.GetExecutingAssembly());

			kernel.Bind<Lazy<IModelMetadataProvider>>().ToMethod(x =>
				new Lazy<IModelMetadataProvider>(() => x.Kernel.Get<IModelMetadataProvider>()));

			kernel.Bind<ControllerFromInterfacesConvention>().ToSelf().InTransientScope(); // no longer needed after startup => transient
			kernel.Bind<IConfigureOptions<MvcOptions>>().To<ConfigureControllerConventionOptions>().InTransientScope(); // no longer needed after startup => transient

			return kernel;
		}

		public static IWebHostBuilder CreateWebHostBuilder()
		{
			return new WebHostBuilder();
		}
	}
}
