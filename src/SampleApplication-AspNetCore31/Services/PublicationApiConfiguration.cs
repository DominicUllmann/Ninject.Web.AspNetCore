using System;

namespace Publication.Infrastructure.Service
{
	public class PublicationApiConfiguration
	{

		public PublicationApiConfiguration(Type apiType, string uri)
		{
			ApiType = apiType;
			Uri = uri;
		}
		/// <summary>
		/// The type which will be resovled from the DI for this api.
		/// Normally an interface.
		/// </summary>
		public Type ApiType { get; set; }
		/// <summary>
		/// The resource part of the url, i.e. without host, port, protocol.
		/// </summary>
		public string Uri { get; set; }

	}
}
