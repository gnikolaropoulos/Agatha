using System;
using Agatha.Common;

namespace Agatha.ServiceLayer
{
    public interface IRequestHandler : IDisposable
	{
		Response Handle(Request request);
		Response CreateDefaultResponse();
	}

    public interface IRequestHandler<TRequest> : IRequestHandler where TRequest : Request
	{
		Response Handle(TRequest request);
	}

    public interface IOneWayRequestHandler : IDisposable
    {
        void Handle(OneWayRequest request);
    }

    public interface IOneWayRequestHandler<TRequest> : IOneWayRequestHandler where TRequest : OneWayRequest
    {
        void Handle(TRequest request);
    }

    public abstract class OneWayRequestHandler : Disposable, IOneWayRequestHandler
    {
        public abstract void Handle(OneWayRequest request);

        /// <summary>
        /// Default implementation is empty
        /// </summary>
        protected override void DisposeManagedResources() { }
    }

    public abstract class RequestHandler : Disposable, IRequestHandler
	{
		public abstract Response Handle(Request request);
		public abstract Response CreateDefaultResponse();

		/// <summary>
		/// Default implementation is empty
		/// </summary>
		protected override void DisposeManagedResources() { }
	}

    public abstract class OneWayRequestHandler<TRequest> : OneWayRequestHandler, IOneWayRequestHandler<TRequest> where TRequest : OneWayRequest
    {
        public override void Handle(OneWayRequest request)
        {
            var typedRequest = (TRequest) request;
            BeforeHandle(typedRequest);
            Handle(typedRequest);
            AfterHandle(typedRequest);
        }

        public virtual void BeforeHandle(TRequest request) { }
        public virtual void AfterHandle(TRequest request) { }

        public abstract void Handle(TRequest request);
    }

    public abstract class RequestHandler<TRequest, TResponse> : RequestHandler, IRequestHandler<TRequest>
		where TRequest : Request
		where TResponse : Response
	{
		public override Response Handle(Request request)
		{
			var typedRequest = (TRequest)request;
			BeforeHandle(typedRequest);
			var response = Handle(typedRequest);
			AfterHandle(typedRequest);
			return response;
		}

		public virtual void BeforeHandle(TRequest request) { }
		public virtual void AfterHandle(TRequest request) { }

		public abstract Response Handle(TRequest request);

		public override Response CreateDefaultResponse()
		{
			return CreateTypedResponse();
		}

		public TResponse CreateTypedResponse()
		{
			return (TResponse)Activator.CreateInstance(typeof(TResponse));
		}
	}
}