using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Publication.Infrastructure.Service
{
	public class ConfigureControllerConventionOptions : IConfigureOptions<MvcOptions>
	{

		private readonly ControllerFromInterfacesConvention _convention;

		public ConfigureControllerConventionOptions(ControllerFromInterfacesConvention convention)
		{
			_convention = convention;
		}

		public void Configure(MvcOptions options)
		{
			options.Conventions.Add(_convention);
		}
	}
}
