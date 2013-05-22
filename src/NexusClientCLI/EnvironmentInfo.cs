/* This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. */


using System;
using System.IO;
using Microsoft.Win32;
using Nexus.Client.Settings;

namespace Nexus.Client
{
	/// <summary>
	/// Provides information about the current programme environment.
	/// </summary>
	public class EnvironmentInfo : IEnvironmentInfo
	{
		private string m_strApplicationPersonalDataFolderPath = null;
		private string m_strPersonalDataFolderPath = null;

		#region Properties

		/// <summary>
		/// Gets the path to the user's personal data folder.
		/// </summary>
		/// <value>The path to the user's personal data folder.</value>
		public string PersonalDataFolderPath
		{
			get
			{
				return m_strPersonalDataFolderPath;
			}
		}

		/// <summary>
		/// Gets the path to the mod manager's folder in the user's personal data folder.
		/// </summary>
		/// <value>The path to the mod manager's folder in the user's personal data folder.</value>
		public string ApplicationPersonalDataFolderPath
		{
			get
			{
				return m_strApplicationPersonalDataFolderPath;
			}
		}

		/// <summary>
		/// Gets whether the programme is running under the Mono framework.
		/// </summary>
		/// <value>Whether the programme is running under the Mono framework.</value>
		public bool IsMonoMode
		{
			get
			{
				return Type.GetType("Mono.Runtime") != null;
			}
		}

		/// <summary>
		/// Gets the temporary path used by the application.
		/// </summary>
		/// <value>The temporary path used by the application.</value>
		public string TemporaryPath
		{
			get
			{
				string strPath = Path.Combine(Path.GetTempPath(),System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
				if (!Directory.Exists(strPath))
					Directory.CreateDirectory(strPath);
				return strPath;
			}
		}

		/// <summary>
		/// Gets the path to the directory where programme data is stored.
		/// </summary>
		/// <value>The path to the directory where programme data is stored.</value>
		public string ProgrammeInfoDirectory
		{
			get
			{
                return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "data");
			}
		}

		/// <summary>
		/// Gets whether the current process is 64bit.
		/// </summary>
		/// <value>Whether the current process is 64bit.</value>
		public bool Is64BitProcess
		{
			get
			{
				return (IntPtr.Size == 8);
			}
		}

		/// <summary>
		/// Gets the application and user settings.
		/// </summary>
		/// <value>The application and user settings.</value>
		public ISettings Settings { get; private set; }

		/// <summary>
		/// Gets the version of the running application.
		/// </summary>
		/// <value>The version of the running application.</value>
		public Version ApplicationVersion
		{
			get
			{
				return new Version(ProgrammeMetadata.VersionString);
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// A simple constructor that initializes the object with the given dependencies.
		/// </summary>
		/// <param name="p_setSettings">The application and user settings.</param>
		public EnvironmentInfo(ISettings p_setSettings)
		{
			Settings = p_setSettings;
			m_strPersonalDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			if (String.IsNullOrEmpty(m_strPersonalDataFolderPath))
				m_strPersonalDataFolderPath = Registry.GetValue(@"HKEY_CURRENT_USER\software\microsoft\windows\currentversion\explorer\user shell folders", "Personal", null).ToString();

			m_strApplicationPersonalDataFolderPath = Path.Combine(m_strPersonalDataFolderPath, p_setSettings.ModManagerName);
		}

		#endregion
	}
}
