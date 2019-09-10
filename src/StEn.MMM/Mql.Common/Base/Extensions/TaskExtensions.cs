using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
using StEn.MMM.Mql.Common.Base.Interfaces;

namespace StEn.MMM.Mql.Common.Base.Extensions
{
	public static class TaskExtensions
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
#pragma warning disable VSTHRD003
			return Task.Run(() => task).WaitAndUnwrapException();
#pragma warning restore VSTHRD003
		}

		public static Task<T> DisposeAfterThreadCompletionAsync<T>(this Task<T> task, IEnumerable<IDisposable> disposableObjects)
		{
			// If there is no SyncContext for this thread (e.g. we are in a unit test
			// or console scenario instead of running in an app), then just use the
			// default scheduler because there is no UI thread to sync with.
			var syncContextScheduler = SynchronizationContext.Current != null ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Current;

			return task.ContinueWith(
				(tResult) =>
				{
					foreach (var disposableObject in disposableObjects)
					{
						DisposeObject(disposableObject);
					}

					return tResult.WaitAndUnwrapException();
				},
				continuationOptions: TaskContinuationOptions.None,
				scheduler: syncContextScheduler,
				cancellationToken: CancellationToken.None);
		}

		private static void DisposeObject(IDisposable disposableObject)
		{
			try
			{
				disposableObject.Dispose();
			}
			catch
			{
				// ignored
			}
		}
	}
}
