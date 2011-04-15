using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agatha.Common;
using Agatha.Common.InversionOfControl;
using Agatha.Common.WCF;

namespace Agatha.ServiceLayer
{
	public class ServiceLayerConfiguration
	{
		private readonly List<Assembly> requestHandlerAssemblies = new List<Assembly>();
		private readonly List<Assembly> requestsAndResponseAssemblies = new List<Assembly>();
		private readonly IContainer container;

		public Type RequestProcessorImplementation { get; set; }
		public Type AsyncRequestProcessorImplementation { get; set; }
		public IContainer Container { get; private set; }
		public Type ContainerImplementation { get; private set; }

		public Type BusinessExceptionType { get; set; }
		public Type SecurityExceptionType { get; set; }

		public ServiceLayerConfiguration(IContainer container)
		{
			this.container = container;
			SetDefaultImplementations();
		}

		public ServiceLayerConfiguration(Type containerImplementation)
		{
			ContainerImplementation = containerImplementation;
			SetDefaultImplementations();
		}

		public ServiceLayerConfiguration(Assembly requestHandlersAssembly, Assembly requestsAndResponsesAssembly, IContainer container)
			: this(container)
		{
			AddRequestHandlerAssembly(requestHandlersAssembly);
			AddRequestAndResponseAssembly(requestsAndResponsesAssembly);
		}

		public ServiceLayerConfiguration(Assembly requestHandlersAssembly, Assembly requestsAndResponsesAssembly, Type containerImplementation)
			: this(containerImplementation)
		{
			AddRequestHandlerAssembly(requestHandlersAssembly);
			AddRequestAndResponseAssembly(requestsAndResponsesAssembly);
		}

		public ServiceLayerConfiguration AddRequestHandlerAssembly(Assembly assembly)
		{
			requestHandlerAssemblies.Add(assembly);
			return this;
		}

		public ServiceLayerConfiguration AddRequestAndResponseAssembly(Assembly assembly)
		{
			requestsAndResponseAssemblies.Add(assembly);
			return this;
		}

		private void SetDefaultImplementations()
		{
			RequestProcessorImplementation = typeof(RequestProcessor);
			AsyncRequestProcessorImplementation = typeof(AsyncRequestProcessor);
		}

		public void Initialize()
		{
			if (IoC.Container == null)
			{
				IoC.Container = container ?? (IContainer)Activator.CreateInstance(ContainerImplementation);
			}

			IoC.Container.RegisterInstance(this);
			IoC.Container.Register(typeof(IRequestProcessor), RequestProcessorImplementation, Lifestyle.Transient);
			IoC.Container.Register(typeof(IAsyncRequestProcessor), AsyncRequestProcessorImplementation, Lifestyle.Transient);
			RegisterRequestAndResponseTypes();
			RegisterRequestHandlers();
		}

		private void RegisterRequestAndResponseTypes()
		{
			foreach (var assembly in requestsAndResponseAssemblies)
			{
				KnownTypeProvider.RegisterDerivedTypesOf<Request>(assembly);
				KnownTypeProvider.RegisterDerivedTypesOf<Response>(assembly);
			}
		}

		private void RegisterRequestHandlers()
		{
			var oneWayHandlerType = typeof(OneWayRequestHandler);
			var openOneWayHandlerType = typeof(IOneWayRequestHandler<>);
			var requestResponseHandlerType = typeof(RequestHandler);
			var openRequestReponseHandlerType = typeof(IRequestHandler<>);

			foreach (var assembly in requestHandlerAssemblies) 
			{
				foreach (var type in assembly.GetTypes())
				{
					if (type.IsAbstract)
						continue;

					if (type.IsSubclassOf(oneWayHandlerType))
					{
						var requestType = GetRequestType(type);

						if (requestType != null)
						{
							var handlerType = openOneWayHandlerType.MakeGenericType(requestType);
							IoC.Container.Register(handlerType, type, Lifestyle.Transient);
						}
						continue;
					}

					if (type.IsSubclassOf(requestResponseHandlerType))
					{
						var requestType = GetRequestType(type);
						if (requestType != null)
						{
							var handlerType = openRequestReponseHandlerType.MakeGenericType(requestType);
							IoC.Container.Register(handlerType, type, Lifestyle.Transient);
						}
					}
				}
			}
		}

		private static Type GetRequestType(Type type)
		{
			if (type.BaseType.IsGenericType)
			{
				// in this case, the Handler inherits from a generic handler interface (RequestHandler<TRequest, TResponse> or OneWayRequestHandler<TRequest> ) so we need the first generic type
				// argument from the basetype
				return GetFirstGenericTypeArgument(type.BaseType);
			}

			var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name.StartsWith("IRequestHandler`") || i.Name.StartsWith("IOneWayRequestHandler`"));

			if (interfaceType == null || interfaceType.GetGenericArguments().Count() == 0)
			{
				return null;
			}

			// in this case, the Handler only inherits from the basic RequestHandler but must implement the 
			// IRequestHandler<TRequest> interface
			return GetFirstGenericTypeArgument(interfaceType);
		}

		private static Type GetFirstGenericTypeArgument(Type type)
		{
			return type.GetGenericArguments()[0];
		}
	}
}