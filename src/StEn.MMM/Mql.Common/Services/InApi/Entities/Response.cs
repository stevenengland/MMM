using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StEn.MMM.Mql.Common.Services.InApi.Entities
{
	[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Response<T>
	{
		[JsonProperty(Required = Required.Always)]
		[DefaultValue(false)]
		public bool IsSuccess { get; set; }

		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		public string CorrelationKey { get; set; }

		[JsonProperty(Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
		public T Content { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
