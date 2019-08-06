using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StEn.MMM.Mql.Common.Services.InApi.Entities
{
	[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Error
	{
		[JsonProperty(Required = Required.Always)]
		public string Message { get; internal set; }

		[JsonProperty(Required = Required.Always)]
		public string ExceptionMessage { get; internal set; }
	}
}
