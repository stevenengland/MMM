using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StEn.MMM.Mql.Generator.Base.Extensions
{
	public static class AssemblyExtensions
	{
		/// <summary>
		/// Get the types within the assembly that match the predicate.
		/// <para>for example, to get all types within a namespace.</para>
		/// <para>    typeof(SomeClassInAssemblyYouWant).Assembly.GetMatchingTypesInAssembly(item => "MyNamespace".Equals(item.Namespace)).</para>
		/// </summary>
		/// <param name="assembly">The assembly to search.</param>
		/// <param name="predicate">The predicate query to match against.</param>
		/// <returns>The collection of types within the assembly that match the predicate.</returns>
		public static ICollection<Type> GetMatchingTypesInAssembly(this Assembly assembly, Predicate<Type> predicate)
		{
			ICollection<Type> types = new List<Type>();
			try
			{
				types = assembly.GetTypes().Where(i => i != null && predicate(i) && i.Assembly == assembly).ToList();
			}
			catch (ReflectionTypeLoadException ex)
			{
				foreach (var theType in ex.Types)
				{
					try
					{
						if (theType != null && predicate(theType) && theType.Assembly == assembly)
						{
							types.Add(theType);
						}
					}

					// This exception list is not exhaustive, modify to suit any reasons
					// you find for failure to parse a single assembly
					catch (BadImageFormatException)
					{
						// Type not in this assembly - reference to elsewhere ignored
					}
				}
			}

			return types;
		}
	}
}
