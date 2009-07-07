﻿using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace CppBinding.Project
{
	public class SpecifyCliRuntimeLibraryCommand : AbstractCommand
	{
		/// <summary>
		/// Adds the runtime library metadata to ClCompile items in project file.
		/// Creates ItemDefinitionGroup sections for Debug and Release configurations.
		/// </summary>
		public override void Run()
		{
			if (!(Owner is MSBuildBasedProject))
				throw new ArgumentException("Project must be an MSBuild based project.");
			MSBuildBasedProject project = (MSBuildBasedProject)Owner;
			
			if ("true".Equals(project.GetEvaluatedProperty("CLRSupport"), StringComparison.OrdinalIgnoreCase))
			{
				SpecifyRuntimeLibrary(project, "Debug", "MultiThreadedDebugDLL");
				SpecifyRuntimeLibrary(project, "Release", "MultiThreadedDLL");
			}
		}
		
		private static void SpecifyRuntimeLibrary(MSBuildBasedProject project, string configuration, string runtime)
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, configuration, null);
			group.SetElementMetadata("ClCompile", "RuntimeLibrary", runtime);
		}
	}
}
