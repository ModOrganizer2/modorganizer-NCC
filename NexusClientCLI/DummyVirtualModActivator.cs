using Nexus.Client.ModManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nexus.Client.BackgroundTasks;
using Nexus.Client.UI;

namespace Nexus.Client.CLI
{
    class DummyVirtualModActivator : IVirtualModActivator
    {
        public event EventHandler ModActivationChanged;

        protected IEnvironmentInfo EnvironmentInfo { get; private set; }

        public bool MultiHDMode
        {
            get { return false; }
        }

        public bool Initialized
        {
            get { return true; }
        }

        public bool DisableLinkCreation
        {
            get { return false; }
        }

        public bool DisableIniLogging
        {
            get { return true; }
        }

        public string VirtualPath
        {
            get
            {
                //string[] components = EnvironmentInfo.Settings.InstallationPaths[GameMode.ModeId].Split(Path.DirectorySeparatorChar);
                //return components[components.Length - 1];
                return "";
            }
        }

        public string HDLinkFolder
        {
            get { throw new NotImplementedException(); }
        }

        public Util.Collections.ThreadSafeObservableList<IVirtualModLink> VirtualLinks
        {
            get { throw new NotImplementedException(); }
        }

        public Util.Collections.ThreadSafeObservableList<IVirtualModInfo> VirtualMods
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> ActiveModList
        {
            get { throw new NotImplementedException(); }
        }

        public int ModCount
        {
            get { throw new NotImplementedException(); }
        }

        public Games.IGameMode GameMode { get; private set; }

        public DummyVirtualModActivator(Games.IGameMode gameMode, IEnvironmentInfo environment)
        {
            GameMode = gameMode;
            EnvironmentInfo = environment;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Setup()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void SaveList()
        {
            throw new NotImplementedException();
        }

        public void SetCurrentList(IList<IVirtualModLink> p_ilvVirtualLinks)
        {
            throw new NotImplementedException();
        }

        public List<IVirtualModLink> LoadList(string p_strXMLFilePath)
        {
            throw new NotImplementedException();
        }

        public List<IVirtualModLink> LoadImportedList(string p_strXML)
        {
            throw new NotImplementedException();
        }

        public bool LoadListOnDemand(string p_strProfilePath, out List<IVirtualModLink> p_lstVirtualLinks, out List<IVirtualModInfo> p_lstVirtualMods)
        {
            throw new NotImplementedException();
        }

        public void SaveModList(string p_strPath)
        {
            throw new NotImplementedException();
        }

        public string CheckVirtualLink(string p_strFilePath)
        {
            throw new NotImplementedException();
        }

        public int CheckFileLink(string p_strFilePath, out Mods.IMod p_modMod, out List<IVirtualModLink> lstFileLinks)
        {
            throw new NotImplementedException();
        }

        public bool PurgeLinks()
        {
            throw new NotImplementedException();
        }

        public void AddInactiveLink(Mods.IMod p_modMod, string p_strBaseFilePath, int p_intPriority)
        {
            throw new NotImplementedException();
        }

        public string AddFileLink(Mods.IMod p_modMod, string p_strBaseFilePath, bool p_booIsSwitching, bool p_booIsRestoring, int p_intPriority)
        {
            throw new NotImplementedException();
        }

        public string AddFileLink(Mods.IMod p_modMod, string p_strBaseFilePath, string p_strSourceFile, bool p_booIsSwitching, bool p_booIsRestoring, bool p_booHandlePlugin, int p_intPriority)
        {
            throw new NotImplementedException();
        }

        public void RemoveFileLink(string p_strFilePath, Mods.IMod p_modMod)
        {
            throw new NotImplementedException();
        }

        public void RemoveFileLink(IVirtualModLink p_ivlVirtualLink, Mods.IMod p_modMod)
        {
            throw new NotImplementedException();
        }

        public void UpdateLinkPriority(IVirtualModLink p_ivlFileLink)
        {
            throw new NotImplementedException();
        }

        public void UpdateLinkListPriority(List<IVirtualModLink> lstFileLinks)
        {
            throw new NotImplementedException();
        }

        public void DisableMod(Mods.IMod p_modMod)
        {
            throw new NotImplementedException();
        }

        public void DisableModFiles(Mods.IMod p_modMod)
        {
            throw new NotImplementedException();
        }

        public void FinalizeModDeactivation(Mods.IMod p_modMod)
        {
            throw new NotImplementedException();
        }

        public void EnableMod(Mods.IMod p_modMod)
        {
            throw new NotImplementedException();
        }

        public void FinalizeModActivation(Mods.IMod p_modMod)
        {
            throw new NotImplementedException();
        }

        public void LogIniEdits(Mods.IMod p_modMod, string p_strSettingsFileName, string p_strSection, string p_strKey, string p_strValue)
        {
            throw new NotImplementedException();
        }

        public void RestoreIniEdits()
        {
            throw new NotImplementedException();
        }

        public void PurgeIniEdits()
        {
            throw new NotImplementedException();
        }

        public void ImportIniEdits(string p_strIniXML)
        {
            throw new NotImplementedException();
        }

        public void SetNewFolders(string p_strVirtual, string p_strLink, bool? p_booMultiHD)
        {
            throw new NotImplementedException();
        }

        public void CheckLinkListIntegrity(IList<IVirtualModLink> p_ivlVirtualLinks, out Dictionary<string, string> p_dicUninstalled, out Dictionary<string, string> p_dicMissing, IList<string> p_lstForced)
        {
            throw new NotImplementedException();
        }

        public IModLinkInstaller GetModLinkInstaller()
        {
            return new DummyModLinkInstaller();
        }

        public void PurgeMods(List<Mods.IMod> p_lstMods, string p_strPath)
        {
            throw new NotImplementedException();
        }

        public bool CheckHasActiveLinks(Mods.IMod p_modMod)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentFileOwner(string p_strPath)
        {
            throw new NotImplementedException();
        }

        public BackgroundTasks.IBackgroundTask ActivatingMod(Mods.IMod p_modMod, bool p_booDisabling, UI.ConfirmActionMethod p_camConfirm)
        {
            throw new NotImplementedException();
        }

        public void SaveModList(string p_strPath, List<IVirtualModInfo> p_lstVirtualModInfo, List<IVirtualModLink> p_lstVirtualModList)
        {
            throw new NotImplementedException();
        }

        public string RequiresFixing()
        {
            throw new NotImplementedException();
        }

        public string RequiresFixing(string p_strFilePath)
        {
            throw new NotImplementedException();
        }

        bool IVirtualModActivator.SaveList()
        {
            throw new NotImplementedException();
        }

        public bool SaveList(bool p_booModActivationChange)
        {
            throw new NotImplementedException();
        }

        public List<IVirtualModLink> LoadImportedList(string p_strXML, string p_strSavePath)
        {
            throw new NotImplementedException();
        }

        public void UpdateDownloadId(string p_strCurrentProfilePath, Dictionary<string, string> p_dctNewDownloadID)
        {
            throw new NotImplementedException();
        }

        public void CheckLinkListIntegrity(IList<IVirtualModLink> p_ivlVirtualLinks, out List<IVirtualModInfo> p_lstMissingModInfo, IList<string> p_lstForced)
        {
            throw new NotImplementedException();
        }

        public IBackgroundTask FixConfigFiles(List<string> p_lstFiles, IModProfile p_mprProfile, ConfirmActionMethod p_camConfirm)
        {
            throw new NotImplementedException();
        }
    }
}
