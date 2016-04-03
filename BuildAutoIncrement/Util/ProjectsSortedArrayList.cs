/*
 * Filename:    ProjectsSortedArrayList.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: List of projects sorted as in Solution Explorer window.
 * Copyright:   Julijan Šribar, 2004-2013
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the author(s) be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
using System;
using System.Collections;
using System.Diagnostics;

namespace BuildAutoIncrement {
	/// <summary>
	///   Sorted list of <c>ProjectInfo</c> objects. Used to assure that 
	///   projects are displayed in the same order as in Solution Explorer 
	///   window of Visual Studio.
	/// </summary>
    public sealed class ProjectsSortedArrayList : ArrayList {

        private class ProjectComparer : IComparer {

            int IComparer.Compare(object obj1, object obj2) {
                return Compare((ProjectInfo)obj1, (ProjectInfo)obj2);
            }

            public int Compare(ProjectInfo pi1, ProjectInfo pi2) {
                if (pi1.ProjectTypeInfo.ProjectType != pi2.ProjectTypeInfo.ProjectType) {
                    int firstTypePriority = ProjectTypeCompare(pi1.ProjectTypeInfo.ProjectType);
                    if (firstTypePriority != 0)
                        return firstTypePriority;
                    int secondTypePriority = ProjectTypeCompare(pi2.ProjectTypeInfo.ProjectType);
                    if (secondTypePriority != 0)
                        return -secondTypePriority;
                }
                string projectName1 = pi1.ProjectName;
                string projectName2 = pi2.ProjectName;
                return string.Compare(projectName1, projectName2);
            }

            private int ProjectTypeCompare(ProjectType projectType) {
                switch (projectType) {
                case ProjectType.SolutionFolder:
                    return -1;
                case ProjectType.SetupProject:
                    return +1;
                }
                return 0;
            }
        }

        public void Add(ProjectInfo pi) {
            base.Add(pi);
            base.Sort(m_projectComparer);
        }

        private new int Add(object o) {
            Add((ProjectInfo)o);
            return base.IndexOf(o);
        }

        public override void AddRange(ICollection c) {
            base.AddRange(c);
            base.Sort(m_projectComparer);
        }

        private ProjectComparer m_projectComparer = new ProjectComparer();
    }
}