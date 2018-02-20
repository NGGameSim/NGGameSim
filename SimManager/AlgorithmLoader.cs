using System;
using NGAPI;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace NGSim
{
	// Loads algorithms at runtime using reflection
	public static class AlgorithmLoader
	{
		private static readonly Type ALGORITHM_TYPE = typeof(Algorithm);

		// Loads an algorithm instance from the assembly given by path
		public static Algorithm LoadAlgorithm(string path)
		{
			if (!File.Exists(path))
				throw new Exception($"The assembly path '{path}' does not exist.");

			// Load the assembly and all of the assembly types
			Assembly asm = Assembly.LoadFrom(path);
			string asmname = asm.FullName;
			List<Type> types = new List<Type>(asm.GetTypes());

			// Find the first type that is a subclass of Algorithm, and perform type validations
			Type algType = null;
			foreach (var type in types)
			{
				if (!type.IsSubclassOf(ALGORITHM_TYPE))
					continue;

				if (type.IsAbstract)
					throw new Exception($"The type '{type.FullName}' is a subclass of Algorithm, but is marked as abstract.");

				if (type.IsGenericType)
					throw new Exception($"The type '{type.FullName}' is a subclass of Algorithm, but is a generic type.");

				var ctor = type.GetConstructor(Type.EmptyTypes);
				if (ctor == null)
					throw new Exception($"The type '{type.FullName}' is a subclass of Algorithm, but does not have a no-arg constructor.");

				// TODO: more checks may be required in the future

				algType = type;
				break;
			}
			if (algType == null)
				throw new Exception($"The algorithm library '{asmname}' does not contain a subclass of Algorithm.");

			// Attempt to create an instance
			Algorithm instance = Activator.CreateInstance(algType) as Algorithm;
			if (instance == null)
				throw new Exception($"Something real bad happened. Dont know how it got here.");

			// Good to go
			return instance;
		}
	}
}
