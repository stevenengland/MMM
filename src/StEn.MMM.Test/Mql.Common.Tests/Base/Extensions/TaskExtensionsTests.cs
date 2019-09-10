using System;
using System.Threading;
using System.Threading.Tasks;
using StEn.MMM.Mql.Common.Base.Extensions;
using StEn.MMM.Mql.Common.Base.Interfaces;
using Xunit;

namespace Mql.Common.Tests.Base.Extensions
{
	public class TaskExtensionsTests : IErrorHandler, ISuccessHandler
	{
		private bool successHandled;
		private bool errorHandled;

		[Fact]
		public async Task FireAndForgetSuccessIsHandledAsync()
		{
			this.SuccessTaskAsync().FireAndForgetSafe("key", this, this);
			await Task.Delay(2000);
			Assert.True(this.successHandled);
		}

		[Fact]
		public async Task FireAndForgetErrorIsHandledAsync()
		{
			this.ThrowingTaskAsync().FireAndForgetSafe("key", this, this);
			await Task.Delay(2000);
			Assert.True(this.errorHandled);
		}

#pragma warning disable xUnit1013 // Public method should be marked as test
		public void HandleFireAndForgetError(Exception ex, string correlationKey)
#pragma warning restore xUnit1013 // Public method should be marked as test
		{
			this.errorHandled = true;
		}

#pragma warning disable xUnit1013 // Public method should be marked as test
		public void HandleFireAndForgetSuccess<T>(T data, string correlationKey)
#pragma warning restore xUnit1013 // Public method should be marked as test
		{
			this.successHandled = true;
		}

		private async Task<string> SuccessTaskAsync()
		{
			await Task.Delay(0);
			return "success";
		}

		private async Task<string> ThrowingTaskAsync()
		{
			await Task.Delay(0);
			throw new AccessViolationException("You are not allowed to be here.");
		}
	}
}
