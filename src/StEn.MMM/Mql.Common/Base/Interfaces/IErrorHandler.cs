using System;

namespace StEn.MMM.Mql.Common.Base.Interfaces
{
	public interface IErrorHandler
	{
		void HandleFireAndForgetError(Exception ex, string correlationKey);
	}
}
