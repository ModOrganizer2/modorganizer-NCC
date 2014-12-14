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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nexus.Client.CLI
{
    class DummyDataFileUtilFactory
    {
        private string m_installationPath;
        private string m_gamePath;
        private int m_copiedFiles = 0;
        private List<string> m_searchPaths = new List<string>();

        /// <summary>
        /// Creates an instance of DummyDataFileUtilFactory.
        /// </summary>
        /// <param name="installationPath">The path where the current mod is being installed</param>
        /// <param name="modlistFile">The path to the modlist.txt file</param>
        /// <param name="modsPath">The path to the mods directory where previously installed mods are located</param>
        public DummyDataFileUtilFactory(string installationPath, string modlistFile, string modsPath, string gamePath, List<string> additionalSearchPaths)
        {
            m_installationPath = installationPath;
            m_gamePath = gamePath;
            m_searchPaths = additionalSearchPaths;
            m_searchPaths.Add(Path.Combine(m_installationPath, "data"));

            StreamReader reader = new StreamReader(modlistFile);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // A mod name per line
                // Handle only activated mods (ignore inactive and unmanaged)
                if (line[0] == '+')
                {
                    string modName = line.Substring(1);
                    string modPath = Path.Combine(modsPath, modName);

                    // Ignore mod when it is being reinstalled
                    // That folder has no relevant data files anyway
                    // This happens when an activated mod is being reinstalled 
                    if (modPath.Equals(m_installationPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Logger.Debug("Ignoring mod {0} that appears to be the one that is being installed", modName);
                    }
                    else
                    {
                        m_searchPaths.Add(modPath);
                    }
                }
            }
        }

        /// <summary>
        /// Create a new instance DummyDataFileUtil that will use the dummy data folder created by this factory.
        /// </summary>
        /// <returns>New instance of DummyDataFileUtil</returns>
        public DummyDataFileUtil CreateDummyDataFileUtil()
        {
            return new DummyDataFileUtil(m_installationPath, m_gamePath, m_searchPaths);
        }
    }
}
