using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using StEn.MMM.Mql.Common.Base.Interfaces;

namespace StEn.MMM.Mql.Common.Base.Extensions.TaskExtensions
{
	public static class TaskExtension
	{
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
#pragma warning disable S3168
#pragma warning disable VSTHRD100
		public static async void FireAndForgetSafe<T>(this Task<T> task, string correlationKey, IErrorHandler errorHandler = null, ISuccessHandler successHandler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
#pragma warning restore S3168
#pragma warning restore VSTHRD100
		{
			try
			{
#pragma warning disable VSTHRD003
				var result = await task;
#pragma warning restore VSTHRD003
				successHandler?.HandleFireAndForgetSuccess(result, correlationKey);
			}
			catch (Exception ex)
			{
				errorHandler?.HandleFireAndForgetError(ex, correlationKey);
			}
		}

		public static T FireSafe<T>(this Task<T> task)
		{
			return Task.Run(() => task).WaitAndUnwrapException();

			/*
			avoid launching a thread from ThreadPool (safe in terms of sync - context but costs more resources
			if (SynchronizationContext.Current == null)
			{
				return task.WaitAndUnwrapException();
			}

			if (task.IsCompleted)
			{
				return task.WaitAndUnwrapException();
			}

			var tcs = new TaskCompletionSource<T>();
			task.ContinueWith(
				t =>
			{
				var ex = t.Exception;
				if (ex != null)
				{
					tcs.SetException(ex);
				}
				else
				{
					tcs.SetResult(t.Result);
				}
			}, TaskScheduler.Default);
			*/
		}
	}
}
