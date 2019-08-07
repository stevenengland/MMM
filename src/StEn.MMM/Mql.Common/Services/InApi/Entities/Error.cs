using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StEn.MMM.Mql.Common.Services.InApi.Entities
{
	[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Error
	{
		[JsonProperty(Required = Required.Always)]
		public string Message { get; internal set; }

		[JsonProperty(Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
		public string ExceptionMessage { get; internal set; }

		[JsonProperty(Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
		public string StackTrace { get; internal set; }

		[JsonProperty(Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
		public string ExceptionType { get; internal set; }
	}
}
