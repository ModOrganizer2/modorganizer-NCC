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
using Nexus.Client.ModManagement.Scripting;
using System.Collections.Generic;

namespace Nexus.Client.CLI
{
    /// <summary>
    /// Provides access to some files in previously installed mods' installation path.
    /// </summary>
    class DummyDataFileUtil : IDataFileUtil
    {
        private string m_gamePath;
        private DataFileUtil m_dataFileUtil;

        protected List<string> SearchPaths { get; set; }

        // To be used internally by DummyDataFileUtilFactory
        // Use DummyDataFileUtilFactory.CreateDummyDataFileUtil() to instantiate
        internal DummyDataFileUtil(string installationPath, string gamePath, List<string> installationPaths)
        {
            m_gamePath = gamePath;
            m_dataFileUtil = new DataFileUtil(installationPath);
            SearchPaths = installationPaths;
        }

        public void AssertFilePathIsSafe(string p_strPath)
        {
            m_dataFileUtil.AssertFilePathIsSafe(p_strPath);
        }

        public bool DataFileExists(string p_strPath)
        {
            string unfixedPath = p_strPath;
            if (unfixedPath.StartsWith("data", StringComparison.OrdinalIgnoreCase))
            {
                unfixedPath = unfixedPath.Substring(5);
            }
            foreach (string path in SearchPaths) {
                if (File.Exists(Path.Combine(path, unfixedPath)))
                {
                    return true;
                }

            }
            return false;
        }

        public string[] GetExistingDataFileList(string p_strPath, string p_strPattern, bool p_booAllFolders)
        {
            // Not implemented
            // Will implement if needed
            return new string[] {};
        }

        public byte[] GetExistingDataFile(string p_strPath)
        {
            AssertFilePathIsSafe(p_strPath);
            if (p_strPath.StartsWith("data", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string path in SearchPaths)
                {
                    string dataPath = Path.Combine(path, p_strPath.Substring(5));
                    Console.WriteLine("test: " + dataPath + " - " + File.Exists(dataPath));
                    if (File.Exists(dataPath))
                    {
                        return File.ReadAllBytes(dataPath);
                    }
                }
            }
            else
            {
                string dataPath = Path.Combine(m_gamePath, p_strPath);
                if (File.Exists(dataPath))
                {
                    return File.ReadAllBytes(dataPath);
                }
            }
            throw new FileNotFoundException();
        }
    }
}
