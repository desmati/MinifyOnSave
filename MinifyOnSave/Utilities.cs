using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MinifyOnSave
{
    public static class Utilities
    {
        public static void LogToOutputWindow(object str)
        {
            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Guid generalPaneGuid = VSConstants.GUID_BuildOutputWindowPane;
            IVsOutputWindowPane generalPane;
            outWindow.GetPane(ref generalPaneGuid, out generalPane);
            generalPane.OutputString(str.ToString() + Environment.NewLine);
            generalPane.Activate();
        }

        #region Public
        /// <summary>
        /// Gets the current DTE (Development Tools Environment) object
        /// </summary>
        /// <returns></returns>
        public static DTE2 GetDTE()
        {
            return Package.GetGlobalService(typeof(DTE)) as DTE2;
        }

        /// <summary>
        /// Gets an insance of the Visual Studio Service Provider
        /// </summary>
        /// <returns></returns>
        public static ServiceProvider GetServiceProvider()
        {
            var dte = GetDTE();
            return new ServiceProvider((IServiceProvider)dte);
        }

        /// <summary>
        /// Gets the current status bar object
        /// </summary>
        /// <returns></returns>
        public static IVsStatusbar GetStatusBar()
        {
            return Package.GetGlobalService(typeof(SVsStatusbar)) as IVsStatusbar;
        }

        /// <summary>
        /// Gets the current solution object
        /// </summary>
        /// <returns></returns>
        public static IVsSolution GetSolution()
        {
            return Package.GetGlobalService(typeof(IVsSolution)) as IVsSolution;
        }

        /// <summary>
        /// Gets all projects in the open solution
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Project> GetProjects()
        {
            return GetProjectsInSolution().Select(GetDTEProject).Where(project => project != null);
        }

        /// <summary>
        /// Gets the number of projects to be built under the current solution configuration
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfProjectsToBuild()
        {
            int count = 0;
            var contexts = GetDTE().Solution.SolutionBuild.ActiveConfiguration.SolutionContexts;
            for (var i = 0; i < contexts.Count; i++)
            {
                if (contexts.Item(i + 1).ShouldBuild)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Converts an integer to a boolean value.  [1 = true | anything else = false]
        /// </summary>
        /// <param name="value">Integer to be converted to boolean (1 or 0)</param>
        /// <returns>Boolean representing the integer value passed in</returns>
        public static bool IntToBool(int value, int trueValue = 1)
        {
            return value == trueValue;
        }

        /// <summary>
        /// Converts a boolean into an integer value. [true = 1 | false = 0]
        /// </summary>
        /// <param name="value">Boolean to be converted into an integer</param>
        /// <returns>Integer representing the boolean value passed in</returns>
        public static int BoolToInt(bool value)
        {
            return value ? 1 : 0;
        }
        #endregion

        #region Private
        private static IEnumerable<IVsHierarchy> GetProjectsInSolution()
        {
            const __VSENUMPROJFLAGS flags = __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION;
            var solution = GetSolution();

            if (solution == null)
                yield break;

            IEnumHierarchies enumHierarchies;
            var guid = Guid.Empty;
            solution.GetProjectEnum((uint)flags, ref guid, out enumHierarchies);
            if (enumHierarchies == null)
                yield break;

            var hierarchy = new IVsHierarchy[1];
            uint fetched;
            while (enumHierarchies.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1)
            {
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                    yield return hierarchy[0];
            }
        }

        private static Project GetDTEProject(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
                throw new ArgumentNullException(nameof(hierarchy));

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as Project;
        }
        #endregion
    }
}
