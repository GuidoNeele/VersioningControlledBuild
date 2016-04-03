/*
 * Filename:    SourceSafeOptions.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Options used for the next version.
 * Copyright:   Julijan Šribar, 2004-2008
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

namespace BuildAutoIncrement
{
	/// <summary>
	///	  SourceSafe related settings.
	/// </summary>
	[Serializable]
	public class SourceSafeOptions : ICloneable {

		public SourceSafeOptions() {
			m_getLatestVersionBefore = false;
			m_checkinModifiedAfter = false;
		}

		public bool GetLatestVersionsBeforeRunningAddinCommand 
		{
			get { return m_getLatestVersionBefore; }
			set { m_getLatestVersionBefore = value; }
		}

		public bool CheckinModifiedAfterAddinCommand 
		{
			get { return m_checkinModifiedAfter; }
			set { m_checkinModifiedAfter = value; }
		}

		#region ICloneable implementation

		object ICloneable.Clone() 
		{
			return Clone();
		}

		public SourceSafeOptions Clone() 
		{
			return (SourceSafeOptions)this.MemberwiseClone();
		}

		#endregion // ICloneable implementation

		private bool m_getLatestVersionBefore;

		private bool m_checkinModifiedAfter;
	}
}
