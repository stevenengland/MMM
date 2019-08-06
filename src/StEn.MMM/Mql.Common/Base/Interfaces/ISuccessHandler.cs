namespace StEn.MMM.Mql.Common.Base.Interfaces
{
	public interface ISuccessHandler
	{
		void HandleSuccess<T>(T data, string correlationKey);
	}
}
