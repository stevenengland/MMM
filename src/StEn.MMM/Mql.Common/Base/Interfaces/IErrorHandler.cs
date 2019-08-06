using System;

namespace StEn.MMM.Mql.Common.Base.Interfaces
{
	public interface IErrorHandler
	{
		void HandleError(Exception ex, string correlationKey);
	}
}
