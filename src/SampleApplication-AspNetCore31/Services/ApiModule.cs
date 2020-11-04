using Ninject.Modules;
using Ninject.Web.Common;
using Publication.Infrastructure.Service;
using ServiceFacadeWebApi.Contracts;

namespace ServiceFacadeWebApi.Services
{
	public class ApiModule : NinjectModule
	{
		public override void Load()
		{
			Kernel.Bind<ITestService>().ToMethod(x => new TestController("TestFromInterface")).InSingletonScope();
			Kernel.Bind<IForecastStationService>().To<ForecastStationService>().InRequestScope().WithConstructorArgument("station", "ZH");
			Kernel.Bind<IForecastStationService>().To<ForecastStationService>().InRequestScope().WithConstructorArgument("station", "TG");
			Kernel.Bind<IForecastService>().To<ForecastServiceController>().InRequestScope();
		}
	}
}
