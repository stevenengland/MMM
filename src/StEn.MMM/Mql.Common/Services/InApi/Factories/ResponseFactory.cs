using System;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Services.InApi.Entities;

namespace StEn.MMM.Mql.Common.Services.InApi.Factories
{
	public static class ResponseFactory
	{
		public static bool IsDebugEnabled { get; set; }

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

		public static Response<Error> Error(Exception ex, string message = null, string correlationKey = null)
		{
			var response = new Response<Error>()
			{
				CorrelationKey = correlationKey,
				IsSuccess = false,
				Content = new Error()
				{
					Message = string.IsNullOrWhiteSpace(message) ? ErrorMessageByException(ex) : message,
				},
			};

			if (IsDebugEnabled)
			{
				response.Content.ExceptionMessage = ex.Message;
				response.Content.StackTrace = ex.StackTrace;
				response.Content.ExceptionType = ex.GetType().Name;
			}

			return response;
		}

		private static string ErrorMessageByException(Exception ex)
		{
			string message;
			switch (ex)
			{
				case OperationCanceledException operationCanceledException:
					message = "The operation was cancelled.";
					break;
				case JsonSerializationException jsonSerializationException:
					message = "There was a problem serializing/deserializing a message.";
					break;
				default:
					message = "An exception occured.";
					break;

			}

			return message;
		}
	}
}
