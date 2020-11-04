using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ServiceFacadeWebApi.Contracts;

namespace Publication.Infrastructure.Service
{
	// Ideas for this one taken from DefaultApplicationModelProvider as well as ApiBehaviorApplicationModelProvider
	// But this one works on interfaces and doesn't require any routing attributes
	public class ControllerFromInterfacesConvention : IApplicationModelConvention
	{

		private readonly Lazy<IModelMetadataProvider> _modelMetadataProvider;
		private readonly IList<PublicationApiConfiguration> _publicationApiConfigurations;

		// It's important to pass here IModelMetadataProvider as lazy as instantiating IModelMetadataProvider requires mvcoptions to be fully processed already
		// ohterwise, leads to cyclic dependeny issue, i.e. construction fails.
		public ControllerFromInterfacesConvention(Lazy<IModelMetadataProvider> modelMetadataProvider, IList<PublicationApiConfiguration> configurations)
		{
			_modelMetadataProvider = modelMetadataProvider;
			_publicationApiConfigurations = configurations;
		}

		public void Apply(ApplicationModel application)
		{
			AddControllerModel(application, new PublicationApiConfiguration(typeof(IForecastService), "api/ForecastService"));
			AddControllerModel(application, new PublicationApiConfiguration(typeof(ITestService), "api/TestService"));
		}

		private void AddControllerModel(ApplicationModel application, PublicationApiConfiguration publicationApiConfiguration)
		{
			var controllerModel = new ControllerModel(publicationApiConfiguration.ApiType.GetTypeInfo(), new List<object>());
			controllerModel.Application = application;

			var inferMappingConvention = new InferParameterBindingInfoConvention(_modelMetadataProvider.Value);

			foreach (var methodInfo in publicationApiConfiguration.ApiType.GetMethods())
			{
				var actionModel = CreateActionModel(publicationApiConfiguration, methodInfo);
				if (actionModel == null)
				{
					continue;
				}

				actionModel.Controller = controllerModel;
				controllerModel.Actions.Add(actionModel);

				foreach (var parameterInfo in actionModel.ActionMethod.GetParameters())
				{
					var parameterModel = CreateParameterModel(parameterInfo);
					if (parameterModel != null)
					{
						parameterModel.Action = actionModel;
						actionModel.Parameters.Add(parameterModel);
					}
				}

				inferMappingConvention.Apply(actionModel);
			}


			application.Controllers.Add(controllerModel);
		}

		private ActionModel CreateActionModel(PublicationApiConfiguration publicationApiConfiguration, MethodInfo methodInfo)
		{
			var result = new ActionModel(methodInfo, new List<object>());
			result.ActionName = methodInfo.Name;
			var selector = new SelectorModel();
			selector.AttributeRouteModel = new AttributeRouteModel
			{
				Name = "R1" + publicationApiConfiguration.ApiType.Name, Template = publicationApiConfiguration.Uri + "/{action}"
			};
			result.Selectors.Add(selector);
			return result;
		}

		private ParameterModel CreateParameterModel(ParameterInfo parameterInfo)
		{
			var attributes = parameterInfo.GetCustomAttributes(inherit: true);

			BindingInfo bindingInfo;
			if (_modelMetadataProvider.Value is ModelMetadataProvider modelMetadataProviderBase)
			{
				var modelMetadata = modelMetadataProviderBase.GetMetadataForParameter(parameterInfo);
				bindingInfo = BindingInfo.GetBindingInfo(attributes, modelMetadata);
			}
			else
			{
				// GetMetadataForParameter should only be used if the user has opted in to the 2.1 behavior.
				bindingInfo = BindingInfo.GetBindingInfo(attributes);
			}

			var parameterModel = new ParameterModel(parameterInfo, attributes)
			{
				ParameterName = parameterInfo.Name,
				BindingInfo = bindingInfo,
			};

			return parameterModel;
		}

	}
}
