using System;
using StEn.MMM.Mql.Common.Services.InApi.Entities;

namespace StEn.MMM.Mql.Common.Services.InApi.Factories
{
	public static class MessageFactory
	{
		public static Response<Message<string>> Success(string correlationKey = null)
		{
			return new Response<Message<string>>()
			{
				CorrelationKey = correlationKey,
				IsSuccess = true,
			};
		}

		public static Response<Message<T>> Success<T>(Message<T> message, string correlationKey = null)
		{
			return new Response<Message<T>>()
			{
				CorrelationKey = correlationKey,
				IsSuccess = true,
				Content = message,
			};
		}

		public static Response<Error> Error(Exception e, string correlationKey = null)
		{
			return new Response<Error>()
			{
				CorrelationKey = correlationKey,
				IsSuccess = false,
				Content = new Error()
				{
					Message = "An exception occured.",
					ExceptionMessage = e.Message,
				},
			};
		}
	}
}
