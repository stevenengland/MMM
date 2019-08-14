namespace StEn.MMM.Mql.Common.Base.Interfaces
{
	public interface ISuccessHandler
	{
		void HandleFireAndForgetSuccess<T>(T data, string correlationKey);
	}
}
