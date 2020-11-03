﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;

namespace Ninject.Web.AspNetCore.Hosting
{
	/// <summary>
	/// Most of the code here is taken from Microsoft.AspNetCore.WebHost.CreateDefaultBuilder but without the
	/// Kestrel & IIS configuration which would introduce many more additional dependencies and also cause
	/// problems because it _always_ configures Kestrel & IIS which means that two IServer instances are
	/// provided which interferes with Ninject's behavior of throwing exceptions when resolving a service
	/// for which multiple bindings apply.
	/// </summary>
	public class DefaultWebHostConfiguration
	{
		private readonly string[] _cliArgs;
		private readonly WebHostBuilder _builder = new WebHostBuilder();

		public DefaultWebHostConfiguration(string[] cliArgs)
		{
			_cliArgs = cliArgs;
		}

		public DefaultWebHostConfiguration ConfigureContentRoot()
		{
			if (string.IsNullOrEmpty(_builder.GetSetting(WebHostDefaults.ContentRootKey)))
			{
				_builder.UseContentRoot(Directory.GetCurrentDirectory());
			}

			return this;
		}

		public DefaultWebHostConfiguration ConfigureAppSettings()
		{
			_builder.ConfigureAppConfiguration((WebHostBuilderContext hostingContext, IConfigurationBuilder config) =>
			{
				IWebHostEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
				config
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.AddJsonFile("appsettings." + hostingEnvironment.EnvironmentName + ".json", optional: true, reloadOnChange: true);

				if (hostingEnvironment.IsDevelopment())
				{
					var assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
					if (assembly != null)
					{
						config.AddUserSecrets(assembly, optional: true);
					}
				}

				config.AddEnvironmentVariables();
				if (_cliArgs != null)
				{
					config.AddCommandLine(_cliArgs);
				}
			});

			return this;
		}

		public DefaultWebHostConfiguration ConfigureLogging()
		{
			_builder.ConfigureLogging((WebHostBuilderContext hostingContext, ILoggingBuilder logging) =>
			{
				logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				logging.AddConsole();
				logging.AddDebug();
				//logging.AddEventSourceLogger();
			});

			return this;
		}

		public DefaultWebHostConfiguration ConfigureAllowedHosts()
		{
			_builder.ConfigureServices((WebHostBuilderContext hostingContext, IServiceCollection services) =>
			{
				services.PostConfigure((HostFilteringOptions options) =>
				{
					if (options.AllowedHosts == null || options.AllowedHosts.Count == 0)
					{
						string[] array = hostingContext.Configuration["AllowedHosts"]?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
						options.AllowedHosts = ((array != null && array.Length != 0) ? array : new string[] { "*" });
					}
				});
				services.AddSingleton((IOptionsChangeTokenSource<HostFilteringOptions>)new ConfigurationChangeTokenSource<HostFilteringOptions>(hostingContext.Configuration));
				services.AddTransient<IStartupFilter, HostFilteringStartupFilter>();
			});

			return this;
		}

		public DefaultWebHostConfiguration ConfigureForwardedHeaders()
		{
			_builder.ConfigureServices((WebHostBuilderContext hostingContext, IServiceCollection services) =>
			{
				if (string.Equals("true", hostingContext.Configuration["ForwardedHeaders_Enabled"], StringComparison.OrdinalIgnoreCase))
				{
					services.Configure(delegate (ForwardedHeadersOptions options)
					{
						options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
						options.KnownNetworks.Clear();
						options.KnownProxies.Clear();
					});
					services.AddTransient<IStartupFilter, ForwardedHeadersStartupFilter>();
				}
			});

			return this;
		}

		public DefaultWebHostConfiguration ConfigureRouting()
		{
			_builder.ConfigureServices((WebHostBuilderContext hostingContext, IServiceCollection services) =>
			{
				services.AddRouting();
			});

			return this;
		}

		public DefaultWebHostConfiguration ConfigureAll()
		{
			return this
				.ConfigureContentRoot()
				.ConfigureAppSettings()
				.ConfigureLogging()
				.ConfigureAllowedHosts()
				.ConfigureForwardedHeaders();
		}

		public IWebHostBuilder GetBuilder()
		{
			return _builder;
		}
	}
}
