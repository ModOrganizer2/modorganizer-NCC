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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Nexus.Client.Plugins;
using Nexus.Client.Util.Collections;
using Nexus.Client.Games;
using Nexus.Client.Mods;
using Nexus.Client.UI;
using Nexus.Client.BackgroundTasks;


namespace Nexus.Client.PluginManagement
{
    class DummyPluginManager : IPluginManager
    {
        private ObservableSet<Plugin> m_Plugins = new ObservableSet<Plugin>();
        private ReadOnlyObservableList<Plugin> m_ROOLPlugins = null;

        public DummyPluginManager(string pluginsFile, IGameMode gameMode, IMod mod)
        {
            StreamReader reader = new StreamReader(pluginsFile);

            string installationPath = Path.Combine(gameMode.GameModeEnvironmentInfo.InstallationPath, gameMode.GetModFormatAdjustedPath(mod.Format, null, false));

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line[0] != '#')
                {
                    m_Plugins.Add(new Plugin(Path.Combine(installationPath, line.ToLower()), line, null));
                }
            }
            ((INotifyCollectionChanged)m_Plugins).CollectionChanged += new NotifyCollectionChangedEventHandler(ActivePlugins_CollectionChanged);
            m_ROOLPlugins = new ReadOnlyObservableList<Plugin>(m_Plugins);
        }

        private void ActivePlugins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public void Release()
        {
        }

        #region Properties

        public ReadOnlyObservableList<Plugin> ManagedPlugins
        {
            get
            {
                return m_ROOLPlugins;
            }
        }

        public ReadOnlyObservableList<Plugin> ActivePlugins
        {
            get
            {
                return m_ROOLPlugins;
            }
        }
        #endregion

        #region Plugin Registration

        public bool AddPlugin(string p_strPluginPath)
        {
            return true;
        }

        public void RemovePlugin(Plugin p_plgPlugin)
        {
            // nop
        }

        public void RemovePlugin(string p_strPluginPath)
        {
            // nop
        }

        public IBackgroundTask AutoPluginSorting(ConfirmActionMethod p_camConfirm)
        {
            return null;
        }

        public IBackgroundTask ManageMultiplePluginsTask(List<Plugin> p_lstPlugins, bool p_booEnable, ConfirmActionMethod p_camConfirm)
        {
            return null;
        }

        public bool IsPluginRegistered(string p_strPath)
        {
            return false;
        }

        public Plugin GetRegisteredPlugin(string p_strPath)
        {
            return null;
        }

        #endregion

        #region Plugin Activation/Deactivation

        public void SetPluginActivation(string p_strPath, bool p_booActive)
        {
            // nop
        }

        public void ActivatePlugin(Plugin p_plgPlugin)
        {
            // nop
        }

        public void ActivatePlugin(string p_strPath)
        {
            // nop
        }

        public void DeactivatePlugin(Plugin p_plgPlugin)
        {
            // nop
        }

        public void DeactivatePlugin(string p_strPath)
        {
            // nop
        }

        public bool IsPluginActive(string p_strPath)
        {
            string temp = Path.GetFileName(p_strPath);
            return m_Plugins.Contains(p => Path.GetFileName(p.Filename) == temp.ToLower());
        }

        public bool CanChangeActiveState(Plugin p_plgPlugin)
        {
            return false;
        }

        #endregion

        #region Plugin Ordering


        public Int32 GetPluginOrderIndex(Plugin p_plgPlugin)
        {
            return -1;
        }


        public void SetPluginOrderIndex(Plugin p_plgPlugin, int p_intNewIndex)
        {
            // nop
        }

        public void SetPluginOrder(IList<Plugin> p_lstOrderedPlugins)
        {
            // nop
        }


        public bool ValidateOrder(IList<Plugin> p_lstPlugins)
        {
            return true;
        }

        #endregion

        public bool IsActivatiblePluginFile(string p_strPath)
        {
            return false;
        }

        public bool CanActivatePlugins()
        {
            return true;
        }

        public int MaxAllowedActivePluginsCount
        {
            get
            {
                return 255;
            }
        }

        public List<Plugin> GetOrphanedPlugins(string p_strMasterName)
        {
            return null;
        }

        public string GetPluginDescription(string p_strPlugin)
        {
            return null;
        }

        public IBackgroundTask ApplyLoadOrder(Dictionary<Plugin, string> p_kvpRegisteredPlugins, bool p_booSortingOnly)
        {
            return null;
        }
    }
}
