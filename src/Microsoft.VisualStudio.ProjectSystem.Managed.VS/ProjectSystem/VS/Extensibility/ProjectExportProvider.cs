﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.VisualStudio.Composition;

namespace Microsoft.VisualStudio.ProjectSystem.VS.Extensibility
{
    /// <summary>
    /// MEF component which has methods for consumers to get to project specific MEF exports
    /// </summary>
    [Export(typeof(IProjectExportProvider))]
    [AppliesTo(ProjectCapability.CSharpOrVisualBasicOrFSharp)]
    internal class ProjectExportProvider : IProjectExportProvider
    {
        private readonly IProjectServiceAccessor _projectServiceAccessor;

        [ImportingConstructor]
        public ProjectExportProvider(IProjectServiceAccessor serviceAccessor)
        {
            _projectServiceAccessor = serviceAccessor;
        }

        /// <summary>
        /// Returns the export for the given project without having to go to the 
        /// UI thread. This is the preferred method for getting access to project specific
        /// exports
        /// </summary>
        public T GetExport<T>(string projectFilePath) where T : class
        {
            if (string.IsNullOrEmpty(projectFilePath))
            {
                throw new ArgumentNullException(nameof(projectFilePath));
            }

            var projectService = _projectServiceAccessor.GetProjectService();
            if (projectService == null)
            {
                return null;
            }

            var unconfiguredProject = projectService.LoadedUnconfiguredProjects
                                                    .FirstOrDefault(x => x.FullPath.Equals(projectFilePath,
                                                                            StringComparison.OrdinalIgnoreCase));
            return unconfiguredProject?.Services.ExportProvider.GetExportedValueOrDefault<T>();
        }
    }
}
