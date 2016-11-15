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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Nexus.Client.Mods;
using Nexus.Client.Util;
using Nexus.Client.Util.Collections;
using Nexus.Client.ModManagement.InstallationLog;

namespace Nexus.Client.ModManagement.InstallationLog
{
    class DummyInstallLog : IInstallLog
    {

        private Dictionary<string, Pair<IMod, string>> m_InstalledFiles = null;
        private Set<string> m_UsedKeys = null;

        public DummyInstallLog()
        {

            m_InstalledFiles = new Dictionary<string, Pair<IMod, string>>();
            m_UsedKeys = new Set<string>();
        }

        public string OriginalValuesKey
        {
            get
            {
                return "bla";
            }
        }

        public ReadOnlyObservableList<IMod> ActiveMods
        {
            get
            {
                return new ReadOnlyObservableList<IMod>(null);
            }
        }

        public void Backup()
        {
        }

        public IList<string> GetInstalledGameSpecificValueEdits(IMod p_modInstaller)
        {
            return null;
        }

        public IList<IMod> GetGameSpecificValueEditInstallers(string p_strKey)
        {
            return null;
        }

        public void LogOriginalGameSpecificValue(string p_strKey, byte[] p_bteValue)
        {
            // nop
        }

        public byte[] GetPreviousGameSpecificValue(string p_strKey)
        {
            return null;
        }

        public IMod GetCurrentGameSpecificValueEditOwner(string p_strKey)
        {
            return null;
        }

        public string GetCurrentGameSpecificValueEditOwnerKey(string p_strKey)
        {
            return null;
        }

        public void RemoveGameSpecificValueEdit(IMod p_modInstallingMod, string p_strKey)
        {
            // nop
        }

        public void ReplaceGameSpecificValueEdit(IMod p_modInstallingMod, string p_strKey, byte[] p_bteValue)
        {
            // nop
        }

        public void AddGameSpecificValueEdit(IMod p_modInstallingMod, string p_strKey, byte[] p_bteValue)
        {
            // nop
        }

        public void AddIniEdit(IMod p_modInstallingMod, string p_strSettingsFileName, string p_strSection, string p_strKey, string p_strValue)
        {
            // nop
        }

        public void ReplaceIniEdit(IMod p_modInstallingMod, string p_strSettingsFileName, string p_strSection, string p_strKey, string p_strValue)
        {
            // nop
        }

        public void RemoveIniEdit(IMod p_modInstallingMod, string p_strSettingsFileName, string p_strSection, string p_strKey)
        {
            // nop
        }

        public IMod GetCurrentIniEditOwner(string p_strSettingsFileName, string p_strSection, string p_strKey)
        {
            return null;
        }

        public string GetCurrentIniEditOwnerKey(string p_strSettingsFileName, string p_strSection, string p_strKey)
        {
            return null;
        }

        public string GetPreviousIniValue(string p_strSettingsFileName, string p_strSection, string p_strKey)
        {
            return null;
        }

        public void LogOriginalIniValue(string p_strSettingsFileName, string p_strSection, string p_strKey, string p_strValue)
        {
            // nop
        }

        public IList<IniEdit> GetInstalledIniEdits(IMod p_modInstaller)
        {
            return null;
        }

        public IList<IMod> GetIniEditInstallers(string p_strSettingsFileName, string p_strSection, string p_strKey)
        {
            return null;
        }

        private string GenerateKey()
        {
            string key;
            do
            {
                key = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            } while (m_UsedKeys.Contains(key));
            m_UsedKeys.Add(key);
            return key;
        }

        public void AddDataFile(IMod p_modInstallingMod, string p_strDataFilePath)
        {
            string key = GenerateKey();
            m_InstalledFiles[p_strDataFilePath.ToLower()] = new Pair<IMod, string>(p_modInstallingMod, key);
        }

        public void RemoveDataFile(IMod p_modInstallingMod, string p_strDataFilePath)
        {
            m_InstalledFiles.Remove(p_strDataFilePath.ToLower());
        }

        public IMod GetCurrentFileOwner(string p_strPath)
        {
            string strPathLower = p_strPath.ToLower();
            if (m_InstalledFiles.ContainsKey(strPathLower))
            {
                return m_InstalledFiles[strPathLower].First;
            }
            else
            {
                return null;
            }
        }

        public IMod GetPreviousFileOwner(string p_strPath)
        {
            return null;
        }

        public string GetCurrentFileOwnerKey(string p_strPath)
        {
            string strPathLower = p_strPath.ToLower();
            if (m_InstalledFiles.ContainsKey(strPathLower))
            {
                return m_InstalledFiles[strPathLower].Second;
            }
            else
            {
                return null;
            }
        }

        public string GetPreviousFileOwnerKey(string p_strPath)
        {
            return null;
        }

        public void LogOriginalDataFile(string p_strDataFilePath)
        {
            // nop
        }

        public IList<string> GetInstalledModFiles(IMod p_modInstaller)
        {
            Set<string> result = new Set<string>();
            foreach (KeyValuePair<string, Pair<IMod, string>> file in m_InstalledFiles)
            {
                if (file.Value.First == p_modInstaller)
                {
                    result.Add(file.Key);
                }
            }
            return result;
        }

        public IList<IMod> GetFileInstallers(string p_strPath)
        {
            Set<IMod> result = new Set<IMod>();
            string strPathLower = p_strPath.ToLower();
            if (m_InstalledFiles.ContainsKey(strPathLower))
            {
                result.Add(m_InstalledFiles[strPathLower].First);
            }
            return result;
        }

        public void AddActiveMod(IMod p_modMod)
        {
            // nop
        }

        public void ReplaceActiveMod(IMod p_modOldMod, IMod p_modNewMod)
        {
            // nop
        }

        public string GetModKey(IMod p_modMod)
        {
            return null;
        }

        /*
        protected IMod GetMod(string p_strKey)
        {
            return null;
        }
        */

        public IEnumerable<KeyValuePair<IMod, IMod>> GetMismatchedVersionMods()
        {
            return null;
        }

        public void RemoveMod(IMod p_modUninstaller)
        {
            // nop
        }

        public void Release()
        {
            // nop
        }

        public byte[] GetXMLModList()
        {
            return null;
        }

        public byte[] GetXMLIniList()
        {
            return null;
        }

        public IInstallLog ReInitialize(string p_strLogPath)
        {
            return this; // ???
        }
    }
}
