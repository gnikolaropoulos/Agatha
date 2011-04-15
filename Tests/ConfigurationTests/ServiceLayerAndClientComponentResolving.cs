using System;
using System.Reflection;
using Agatha.Common;
using Agatha.Common.Caching;
using Agatha.Common.InversionOfControl;
using Agatha.Common.WCF;
using Agatha.ServiceLayer;
using TestTypes;
using Xunit;

namespace Tests.ConfigurationTests
{
	public abstract class ServiceLayerAndClientComponentResolving<TContainer> where TContainer : IContainer
	{
		static ServiceLayerAndClientComponentResolving()
		{
			IoC.Container = null;
			KnownTypeProvider.ClearAllKnownTypes();
			var configuration = new ServiceLayerAndClientConfiguration(Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly(), 
																		Activator.CreateInstance<TContainer>());
			configuration.AddRequestHandlerAssembly(typeof(RequestHandlerB).Assembly);
			configuration.AddRequestAndResponseAssembly(typeof(RequestB).Assembly);
			configuration.Initialize();
		}

		[Fact]
		public void CanResolveServiceLayerConfiguration()
		{
			Assert.NotNull(IoC.Container.Resolve<ServiceLayerConfiguration>());
		}

		[Fact]
		public void ServiceLayerConfigurationIsSingleton()
		{
			Assert.Same(IoC.Container.Resolve<ServiceLayerConfiguration>(), IoC.Container.Resolve<ServiceLayerConfiguration>());
		}

		[Fact]
		public void CanResolveRequestProcessor()
		{
			Assert.NotNull(IoC.Container.Resolve<IRequestProcessor>());
		}

		[Fact]
		public void RequestProcessorIsTransient()
		{
			Assert.NotSame(IoC.Container.Resolve<IRequestProcessor>(), IoC.Container.Resolve<IRequestProcessor>());
		}

		[Fact]
		public void CanResolveAsyncRequestProcessor()
		{
			Assert.NotNull(IoC.Container.Resolve<IAsyncRequestProcessor>());
		}

		[Fact]
		public void AsyncRequestProcessorIsTransient()
		{
			Assert.NotSame(IoC.Container.Resolve<IAsyncRequestProcessor>(), IoC.Container.Resolve<IAsyncRequestProcessor>());
		}

		[Fact]
		public void CanResolveRequestHandler()
		{
			Assert.NotNull(IoC.Container.Resolve<IRequestHandler<RequestA>>());
		}

		[Fact]
		public void CanResolveRequestHandlerFromExtraAssembly()
		{
			Assert.NotNull(IoC.Container.Resolve<IRequestHandler<RequestB>>());
		}

		[Fact]
		public void RequestHandlerIsTransient()
		{
			Assert.NotSame(IoC.Container.Resolve<IRequestHandler<RequestA>>(), IoC.Container.Resolve<IRequestHandler<RequestA>>());
		}

		[Fact]
		public void CanResolveOneWayRequestHandler()
		{
			Assert.NotNull(IoC.Container.Resolve<IOneWayRequestHandler<OneWayRequestA>>());
		}

		[Fact]
		public void CanResolveOneWayRequestHandlerFromExtraAssembly()
		{
			Assert.NotNull(IoC.Container.Resolve<IRequestHandler<RequestB>>());
		}

		[Fact]
		public void OneWayRequestHandlerIsTransient()
		{
			Assert.NotSame(IoC.Container.Resolve<IOneWayRequestHandler<OneWayRequestA>>(), IoC.Container.Resolve<IOneWayRequestHandler<OneWayRequestA>>());
		}
	
		[Fact]
		public void CanResolveRequestDispatcher()
		{
			Assert.NotNull(IoC.Container.Resolve<IRequestDispatcher>());
		}

		[Fact]
		public void RequestDispatcherIsTransient()
		{
			Assert.NotSame(IoC.Container.Resolve<IRequestDispatcher>(), IoC.Container.Resolve<IRequestDispatcher>());
		}

		[Fact]
		public void CanResolveAsyncRequestDispatcher()
		{
			Assert.NotNull(IoC.Container.Resolve<IAsyncRequestDispatcher>());
		}

		[Fact]
		public void AsyncRequestDispatcherIsTransient()
		{
			Assert.NotSame(IoC.Container.Resolve<IAsyncRequestDispatcher>(), IoC.Container.Resolve<IAsyncRequestDispatcher>());
		}

		[Fact]
		public void CanResolveRequestDispatcherFactory()
		{
			Assert.NotNull(IoC.Container.Resolve<IRequestDispatcherFactory>());
		}

		[Fact]
		public void RequestDispatcherFactoryIsSingleton()
		{
			Assert.Same(IoC.Container.Resolve<IRequestDispatcherFactory>(), IoC.Container.Resolve<IRequestDispatcherFactory>());
		}

		[Fact]
		public void CanResolveAsyncRequestDispatcherFactory()
		{
			Assert.NotNull(IoC.Container.Resolve<IAsyncRequestDispatcherFactory>());
		}

		[Fact]
		public void AsyncRequestDispatcherIsSingleton()
		{
			Assert.Same(IoC.Container.Resolve<IAsyncRequestDispatcherFactory>(), IoC.Container.Resolve<IAsyncRequestDispatcherFactory>());
		}

		[Fact]
		public void CanResolveCacheProvider()
		{
			Assert.NotNull(IoC.Container.Resolve<ICacheProvider>());
		}

		[Fact]
		public void CacheProviderIsSingleton()
		{
			Assert.Same(IoC.Container.Resolve<ICacheProvider>(), IoC.Container.Resolve<ICacheProvider>());
		}

		[Fact]
		public void CanResolveCacheManager()
		{
			Assert.NotNull(IoC.Container.Resolve<ICacheManager>());
		}

		[Fact]
		public void CacheManagerIsSingleton()
		{
			Assert.Same(IoC.Container.Resolve<ICacheManager>(), IoC.Container.Resolve<ICacheManager>());
		}

		[Fact]
		public void CanResolveCacheConfiguration()
		{
			Assert.NotNull(IoC.Container.Resolve<CacheConfiguration>());
		}

		[Fact]
		public void CacheConfigurationIsSingleton()
		{
			Assert.Same(IoC.Container.Resolve<CacheConfiguration>(), IoC.Container.Resolve<CacheConfiguration>());
		}

		[Fact]
		public void RequestTypeIsRegistered()
		{
			Assert.Contains(typeof(RequestA), KnownTypeProvider.GetKnownTypes(null));
		}

		[Fact]
		public void RequestTypeFromExtraAssemblyIsRegistered()
		{
			Assert.Contains(typeof(RequestB), KnownTypeProvider.GetKnownTypes(null));
		}

		[Fact]
		public void ResponseTypeIsRegistered()
		{
			Assert.Contains(typeof(ResponseA), KnownTypeProvider.GetKnownTypes(null));
		}

		[Fact]
		public void ResponseTypeFromExtraAssemblyIsRegistered()
		{
			Assert.Contains(typeof(ResponseB), KnownTypeProvider.GetKnownTypes(null));
		}
	}

	public class ServiceLayerAndClientComponentResolvingWithCastle : ServiceLayerAndClientComponentResolving<Agatha.Castle.Container> { }

	public class ServiceLayerAndClientComponentResolvingWithUnity : ServiceLayerAndClientComponentResolving<Agatha.Unity.Container> { }

	public class ServiceLayerAndClientComponentResolvingWithNinject : ServiceLayerAndClientComponentResolving<Agatha.Ninject.Container> { }

	public class ServiceLayerAndClientComponentResolvingWithStructureMap : ServiceLayerAndClientComponentResolving<Agatha.StructureMap.Container> { }

	public class ServiceLayerAndClientComponentResolvingWithSpring : ServiceLayerAndClientComponentResolving<Agatha.Spring.Container> { }
}