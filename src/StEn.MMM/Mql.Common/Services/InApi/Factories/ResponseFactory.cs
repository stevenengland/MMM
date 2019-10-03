using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StEn.MMM.Mql.Common.Base.Extensions;
using StEn.MMM.Mql.Common.Services.InApi.Entities;

namespace StEn.MMM.Mql.Common.Services.InApi.Factories
{
	public class ResponseFactory
	{
		public bool IsDebugEnabled { get; set; }

		public Response<string> Success(string correlationKey = null)
		{
			return new Response<string>()
			{
				CorrelationKey = correlationKey,
				IsSuccess = true,
			};
		}

		public Response<T> Success<T>(T message, string correlationKey = null)
		{
			return new Response<T>()
			{
				CorrelationKey = correlationKey,
				IsSuccess = true,
				Content = message,
			};
		}

		public Response<Error> Error(Exception ex, string message = null, string correlationKey = null)
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

			if (this.IsDebugEnabled)
			{
				response.Content.ExceptionMessage = ex.GetAllExceptionMessages();
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
				case ArgumentException _:
					message = "One or more arguments are not valid.";
					break;
				case JsonSerializationException _:
					message = "There was a problem serializing/deserializing a message.";
					break;
				case OperationCanceledException _:
					message = "The operation was cancelled.";
					break;
				case KeyNotFoundException _:
					message = "The key could not be found.";
					break;
				default:
					message = "An exception occured.";
					break;
			}

			return message;
		}
	}
}
