using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StEn.MMM.Mql.Common.Base.Interfaces
{
	public interface IProxyCall
	{
		string FireAndForgetProxyCall<T>(Task<T> telegramMethod);

		string ProxyCall<T>(Task<T> telegramMethod);
	}
}
