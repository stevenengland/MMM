using System.Threading.Tasks;

namespace StEn.MMM.Mql.Common.Base.Interfaces
{
	public interface IProxyCall
	{
		string FireAndForgetProxyCall<T>(Task<T> method);

		string ProxyCall<T>(Task<T> method);
	}
}
