using System;
using System.Collections.Generic;
using System.Text;

namespace StEn.MMM.Mql.Common.Base.Extensions
{
	public static class ExceptionExtensions
	{
		public static string GetAllExceptionMessages(this Exception ex)
		{
			if (ex == null)
			{
				throw new ArgumentNullException($"The exception must not be null.");
			}

			var sb = new StringBuilder();
			var i = 1;
			while (ex != null)
			{
				if (!string.IsNullOrEmpty(ex.Message))
				{
					if (sb.Length > 0)
					{
						sb.Append("\n");
						sb.Append($"Inner #{i}: ");
						i++;
					}

					sb.Append(ex.Message);
				}

				ex = ex.InnerException;
			}

			return sb.ToString();
		}
	}
}
