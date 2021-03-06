﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ninject.Web.AspNetCore
{
	public class NInjectServiceScope : IServiceScope
	{

		private bool _disposed;
		private readonly IKernel _kernel;
		private readonly RequestScope _scope;

		public NInjectServiceScope(IKernel kernel)
		{
			_kernel = kernel;
			_scope = new RequestScope();
		}

		// note that we can't return the IKernel directly here, although it would implement IServiceProvider.
		// the problem is, that Ninject incorrectly throws an exception if no binding resolvable whereas
		// IServiceProvider.GetService requires to return null in this case.
		// see https://docs.microsoft.com/en-us/dotnet/api/system.iserviceprovider.getservice?view=netcore-3.1
		public IServiceProvider ServiceProvider => _kernel.Get<IServiceProvider>();

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				this._disposed = true;

				if (disposing)
				{
					_scope.Dispose();
				}
			}
		}

	}
}
