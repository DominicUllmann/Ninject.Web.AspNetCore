using System.Runtime.Serialization;

namespace ServiceFacadeWebApi.Contracts
{
	[DataContract]
	public class StationForecastRequest
	{
		[DataMember]
		public string Station { get; set; }

	}
}
