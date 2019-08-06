using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StEn.MMM.Mql.Common.Services.InApi.Entities
{
	[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Message<T>
	{
		[JsonProperty(Required = Required.Always)]
		public T Payload { get; set; }
	}
}
