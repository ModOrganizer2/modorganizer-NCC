using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nexus.Client.ModManagement.Scripting;
using Nexus.Client.ModManagement.InstallationLog;
using Nexus.Client.Mods;
using Nexus.Client.PluginManagement;
using Nexus.Client.Util;
using Nexus.Client.Games;
using ChinhDo.Transactions;
using SevenZip;
//using Extensions;
using System.Text.RegularExpressions;

namespace Nexus.Client.ModManagement
{
    /// <summary>
    /// This installs mod files.
    /// </summary>
    public class ModFileInstaller : IModFileInstaller
    {
        private List<string> m_lstOverwriteFolders = new List<string>();
        private List<string> m_lstDontOverwriteFolders = new List<string>();
        private bool m_booDontOverwriteAll = false;
        private bool m_booOverwriteAll = false;
        private ConfirmItemOverwriteDelegate m_dlgOverwriteConfirmationDelegate = null;

        #region Properties

        /// <summary>
        /// Gets or sets the mod being installed.
        /// </summary>
        /// <value>The mod being installed.</value>
        protected IMod Mod { get; set; }

        /// <summary>
        /// Gets the environment info of the current game mode.
        /// </summary>
        /// <value>The environment info of the current game mode.</value>
        protected IGameModeEnvironmentInfo GameModeInfo { get; private set; }

        /// <summary>
        /// Gets or sets the utility class to use to work with data files.
        /// </summary>
        /// <value>The utility class to use to work with data files.</value>
        protected IDataFileUtil DataFileUtility { get; set; }

        /// <summary>
        /// Gets or sets the transactional file manager to use to interact with the file system.
        /// </summary>
        /// <value>The transactional file manager to use to interact with the file system.</value>
        protected TxFileManager TransactionalFileManager { get; set; }

        /// <summary>
        /// Gets or sets the install log to use to log file installations.
        /// </summary>
        /// <value>The install log to use to log file installations.</value>
        protected IInstallLog InstallLog { get; set; }

        /// <summary>
        /// Gets manager to use to manage plugins.
        /// </summary>
        /// <value>The manager to use to manage plugins.</value>
        protected IPluginManager PluginManager { get; private set; }

        /// <summary>
        /// Gets whether the file is a mod or a plugin.
        /// </summary>
        /// <value>true or false.</value>
        protected bool IsPlugin { get; private set; }

        protected string ExtractPath { get; private set; }

        protected string Prefix { get; private set; }

        protected Dictionary<string, ArchiveFileInfo> TargetFiles { get; private set; }

        protected List<string> IgnoreFolders = new List<string> { "__MACOSX" };
        protected IList<string> StopFolders { get; private set; }
        private Dictionary<string, string> m_dicMovedArchiveFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private List<string> m_strFiles = null;
        private Dictionary<string, ArchiveFileInfo> m_dicFileInfo = null;

        #endregion

        #region Constructors

        /// <summary>
        /// A simple constructor that initializes the object with its dependencies.
        /// </summary>
        /// <param name="p_gmiGameModeInfo">The environment info of the current game mode.</param>
        /// <param name="p_modMod">The mod being installed.</param>
        /// <param name="p_ilgInstallLog">The install log to use to log file installations.</param>
        /// <param name="p_pmgPluginManager">The plugin manager.</param>
        /// <param name="p_dfuDataFileUtility">The utility class to use to work with data files.</param>
        /// <param name="p_tfmFileManager">The transactional file manager to use to interact with the file system.</param>
        /// <param name="p_dlgOverwriteConfirmationDelegate">The method to call in order to confirm an overwrite.</param>
        /// <param name="p_UsesPlugins">Whether the file is a mod or a plugin.</param>
        public ModFileInstaller(IGameModeEnvironmentInfo p_gmiGameModeInfo, IMod p_modMod, IInstallLog p_ilgInstallLog, IPluginManager p_pmgPluginManager, IDataFileUtil p_dfuDataFileUtility, TxFileManager p_tfmFileManager, ConfirmItemOverwriteDelegate p_dlgOverwriteConfirmationDelegate, bool p_UsesPlugins, string p_extractPath, IEnumerable<string> p_stopFolders)
        {
            GameModeInfo = p_gmiGameModeInfo;
            Mod = p_modMod;
            InstallLog = p_ilgInstallLog;
            PluginManager = p_pmgPluginManager;
            DataFileUtility = p_dfuDataFileUtility;
            TransactionalFileManager = p_tfmFileManager;
            TargetFiles = new Dictionary<string, ArchiveFileInfo>();
            m_dlgOverwriteConfirmationDelegate = p_dlgOverwriteConfirmationDelegate ?? ((s, b, m) => OverwriteResult.No);
            IsPlugin = p_UsesPlugins;
            ExtractPath = p_extractPath;
            StopFolders = new List<string>(p_stopFolders);
            if (!StopFolders.Contains("fomod", StringComparer.OrdinalIgnoreCase))
              StopFolders.Add("fomod");
            Archive archive = new Archive(Mod.ModArchivePath);
            Prefix = FindPathPrefix(archive);
            m_dicFileInfo = new Dictionary<string, ArchiveFileInfo>(StringComparer.OrdinalIgnoreCase);
            m_strFiles = new List<string>();
            LoadFileIndices();
        }

        #endregion

        /// <summary>
        /// Verifies if the given file can be written.
        /// </summary>
        /// <remarks>
        /// This method checks if the given path is valid. If so, and the file does not
        /// exist, the file can be written. If the file does exist, than the user is
        /// asked to overwrite the file.
        /// </remarks>
        /// <param name="p_strPath">The file path, relative to the Data folder, whose writability is to be verified.</param>
        /// <returns><c>true</c> if the location specified by <paramref name="p_strPath"/>
        /// can be written; <c>false</c> otherwise.</returns>
        protected bool TestDoOverwrite(string p_strPath)
        {
            string strDataPath = Path.Combine(GameModeInfo.InstallationPath, p_strPath);
            if (!File.Exists(strDataPath))
                return true;
            string strLoweredPath = strDataPath.ToLowerInvariant();
            if (m_lstOverwriteFolders.Contains(Path.GetDirectoryName(strLoweredPath)))
                return true;
            if (m_lstDontOverwriteFolders.Contains(Path.GetDirectoryName(strLoweredPath)))
                return false;
            if (m_booOverwriteAll)
                return true;
            if (m_booDontOverwriteAll)
                return false;

            IMod modOld = InstallLog.GetCurrentFileOwner(p_strPath);
            if (modOld == Mod)
                return true;
            string strMessage = null;
            if (modOld != null)
            {
                strMessage = String.Format("Data file '{{0}}' has already been installed by '{0}'" + Environment.NewLine +
                                "Overwrite with this mod's file?", modOld.ModName);
            }
            else
            {
                strMessage = "Data file '{0}' already exists." + Environment.NewLine +
                                "Overwrite with this mod's file?";
            }
            switch (m_dlgOverwriteConfirmationDelegate(String.Format(strMessage, p_strPath), true, (modOld != null)))
            {
                case OverwriteResult.Yes:
                    return true;
                case OverwriteResult.No:
                    return false;
                case OverwriteResult.NoToAll:
                    m_booDontOverwriteAll = true;
                    return false;
                case OverwriteResult.YesToAll:
                    m_booOverwriteAll = true;
                    return true;
                case OverwriteResult.NoToGroup:
                    Queue<string> folders = new Queue<string>();
                    folders.Enqueue(Path.GetDirectoryName(strLoweredPath));
                    while (folders.Count > 0)
                    {
                        strLoweredPath = folders.Dequeue();
                        if (!m_lstOverwriteFolders.Contains(strLoweredPath))
                        {
                            m_lstDontOverwriteFolders.Add(strLoweredPath);
                            foreach (string s in Directory.GetDirectories(strLoweredPath))
                            {
                                folders.Enqueue(s.ToLowerInvariant());
                            }
                        }
                    }
                    return false;
                case OverwriteResult.YesToGroup:
                    folders = new Queue<string>();
                    folders.Enqueue(Path.GetDirectoryName(strLoweredPath));
                    while (folders.Count > 0)
                    {
                        strLoweredPath = folders.Dequeue();
                        if (!m_lstDontOverwriteFolders.Contains(strLoweredPath))
                        {
                            m_lstOverwriteFolders.Add(strLoweredPath);
                            foreach (string s in Directory.GetDirectories(strLoweredPath))
                            {
                                folders.Enqueue(s.ToLowerInvariant());
                            }
                        }
                    }
                    return true;
                default:
                    throw new Exception("Sanity check failed: OverwriteDialog returned a value not present in the OverwriteResult enum");
            }
        }

        /// <summary>
        /// Installs the specified file from the Mod to the file system.
        /// </summary>
        /// <param name="p_strModFilePath">The path of the file in the Mod to install.</param>
        /// <param name="p_strInstallPath">The path on the file system where the file is to be installed.</param>
        /// <returns><c>true</c> if the file was written; <c>false</c> if the user chose
        /// not to overwrite an existing file.</returns>
        public bool InstallFileFromMod(string p_strModFilePath, string p_strInstallPath)
        {
            try
            {
                string filePath = GetRealPath(p_strModFilePath);
                string extractPath = Path.Combine(ExtractPath, filePath);
                Directory.CreateDirectory(Path.GetDirectoryName(extractPath));
                string strPath = filePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                ArchiveFileInfo afiFile = m_dicFileInfo[strPath];
                TargetFiles[p_strInstallPath] = afiFile;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected string GetRealPath(string p_strPath)
        {
            string strPath = p_strPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            strPath = strPath.Trim(Path.DirectorySeparatorChar);
            string strAdjustedPath = null;
            if (m_dicMovedArchiveFiles.TryGetValue(strPath, out strAdjustedPath))
                return strAdjustedPath;
            if (String.IsNullOrEmpty(Prefix))
                return p_strPath;
            if (strPath.ToLowerInvariant().IndexOf(Prefix.ToLowerInvariant()) == 0)
                return p_strPath;
            else
                return Path.Combine(Prefix, p_strPath);
        }

        /// <summary>
        /// This finds where in the archive the FOMod file structure begins.
        /// </summary>
        /// <remarks>
        /// This methods finds the path prefix to the folder containing the core files and folders of the FOMod. If
        /// there are any files that are above the core folder, than they are given new file names inside the
        /// core folder.
        /// </remarks>
        protected string FindPathPrefix(Archive m_arcFile)
        {
            string strPrefixPath = null;
            Stack<string> stkPaths = new Stack<string>();
            stkPaths.Push("/");

            while (stkPaths.Count > 0)
            {
                string strSourcePath = stkPaths.Pop();
                string[] directories = m_arcFile.GetDirectories(strSourcePath);
                bool booFoundPrefix = false;
                foreach (string strDirectory in directories)
                {
                    bool booSkipFolder = false;

                    foreach (string Folder in IgnoreFolders)
                        if (strDirectory.IndexOf(Folder, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            booSkipFolder = true;
                            break;
                        }

                    if (booSkipFolder)
                        continue;

                    stkPaths.Push(strDirectory);
                    if (StopFolders.Contains(Path.GetFileName(strDirectory).ToLowerInvariant()))
                    {
                        booFoundPrefix = true;
                        break;
                    }
                }
                if (booFoundPrefix)
                {
                    strPrefixPath = strSourcePath;
                    break;
                }
            }

            strPrefixPath = (strPrefixPath == null) ? "" : strPrefixPath.Trim(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (!String.IsNullOrEmpty(strPrefixPath))
                strPrefixPath = InitializeMovedArchive(strPrefixPath, m_arcFile);
            
            return strPrefixPath;
        }

        private string InitializeMovedArchive(string p_strPathPrefix, Archive m_arcFile)
        {
            p_strPathPrefix = p_strPathPrefix.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            p_strPathPrefix = p_strPathPrefix.Trim(Path.DirectorySeparatorChar);
            p_strPathPrefix += Path.DirectorySeparatorChar;
            m_dicMovedArchiveFiles.Clear();
            string[] strFiles = m_arcFile.GetFiles("/", true);
            Int32 intTrimLength = p_strPathPrefix.Length;
            for (Int32 i = strFiles.Length - 1; i >= 0; i--)
            {
                strFiles[i] = strFiles[i].Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                string strFile = strFiles[i];
                string strNewFileName = null;
                if (!strFile.StartsWith(p_strPathPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    strNewFileName = strFile;
                    string strDirectory = Path.GetDirectoryName(strNewFileName);
                    string strFileName = Path.GetFileNameWithoutExtension(strFile);
                    string strExtension = Path.GetExtension(strFile);
                    for (Int32 j = 1; m_dicMovedArchiveFiles.ContainsKey(strNewFileName); j++)
                        strNewFileName = Path.Combine(strDirectory, strFileName + " " + j + strExtension);
                }
                else
                    strNewFileName = strFile.Remove(0, intTrimLength);
                m_dicMovedArchiveFiles[strNewFileName] = strFile;
            }

            return p_strPathPrefix;
        }

        /// <summary>
        /// Caches information about the files in the archive.
        /// </summary>
        protected void LoadFileIndices()
        {
            SevenZipExtractor extractor = new SevenZipExtractor(Mod.ModArchivePath);
            m_dicFileInfo.Clear();
            m_strFiles.Clear();

            foreach (ArchiveFileInfo afiFile in extractor.ArchiveFileData)
                if (!afiFile.IsDirectory)
                {
                    m_dicFileInfo[afiFile.FileName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)] = afiFile;
                    m_strFiles.Add(afiFile.FileName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
                }

            extractor.Dispose();
        }

        private string installPath(string installPath)
        {
            DataFileUtility.AssertFilePathIsSafe(installPath);
            return Path.Combine(GameModeInfo.InstallationPath, installPath);
        }

        private static readonly Regex m_rgxCleanPath = new Regex("[" + Path.DirectorySeparatorChar + Path.AltDirectorySeparatorChar + "]{1,}");

        private void CreateDirectoryRecursion(string[] paths, int roffset)
        {
            string path = string.Join("" + Path.DirectorySeparatorChar, paths.Take(paths.Length - roffset).ToArray());
            if (!Directory.Exists(path)) {
                CreateDirectoryRecursion(paths, roffset + 1);
                Directory.CreateDirectory(path);
            }
        }

        private void CreateDirectory(string path)
        {
            string normalizedPath = m_rgxCleanPath.Replace(Path.GetFullPath(path), Path.DirectorySeparatorChar.ToString());

            string[] paths = normalizedPath.Split(Path.DirectorySeparatorChar);

            CreateDirectoryRecursion(paths, 0);
        }

        /// <summary>
        /// Writes the file represented by the given byte array to the given path.
        /// </summary>
        /// <remarks>
        /// This method writes the given data as a file at the given path. If the file
        /// already exists the user is prompted to overwrite the file.
        /// </remarks>
        /// <param name="p_strPath">The path where the file is to be created.</param>
        /// <param name="p_bteData">The data that is to make up the file.</param>
        /// <returns><c>true</c> if the file was written; <c>false</c> if the user chose
        /// not to overwrite an existing file.</returns>
        /// <exception cref="IllegalFilePathException">Thrown if <paramref name="p_strPath"/> is
        /// not safe.</exception>
        public virtual bool GenerateDataFile(string p_strPath, byte[] p_bteData)
        {
            DataFileUtility.AssertFilePathIsSafe(p_strPath);

            string[] components = p_strPath.Split(Path.DirectorySeparatorChar);
            p_strPath = string.Join("" + Path.DirectorySeparatorChar, components.Skip(1).Take(components.Length - 1).ToArray());
            string strInstallFilePath = installPath(p_strPath);


            //string strInstallFilePath = null;
            //strInstallFilePath = Path.Combine(GameModeInfo.InstallationPath, p_strPath);

            string installDirPath = Path.GetDirectoryName(strInstallFilePath);

            FileInfo Info = new FileInfo(strInstallFilePath);
            if (!Directory.Exists(installDirPath)) {
                CreateDirectory(installDirPath);
            }
            else
            {
                if (!TestDoOverwrite(p_strPath))
                    return false;

                if (File.Exists(strInstallFilePath))
                {
                    if (Info.IsReadOnly == true)
                        File.SetAttributes(strInstallFilePath, File.GetAttributes(strInstallFilePath) & ~FileAttributes.ReadOnly);
                    string strInstallDirectory = Path.GetDirectoryName(p_strPath);
                    string strBackupDirectory = Path.Combine(GameModeInfo.OverwriteDirectory, strInstallDirectory);
                    string strOldModKey = InstallLog.GetCurrentFileOwnerKey(p_strPath);
                    if (strOldModKey == null)
                    {
                        InstallLog.LogOriginalDataFile(p_strPath);
                        strOldModKey = InstallLog.OriginalValuesKey;
                    }
                    string strInstallingModKey = InstallLog.GetModKey(Mod);
                    //if this mod has installed this file already we just replace it and don't
                    // need to back it up.
                    if (!strOldModKey.Equals(strInstallingModKey))
                    {
                        //back up the current version of the file if the current mod
                        // didn't install it
                        if (!Directory.Exists(strBackupDirectory))
                            CreateDirectory(strBackupDirectory);

                        //we get the file name this way in order to preserve the file name's case
                        string strFile = Path.GetFileName(Directory.GetFiles(installDirPath, Path.GetFileName(strInstallFilePath))[0]);
                        strFile = strOldModKey + "_" + strFile;

                        string strBackupFilePath = Path.Combine(strBackupDirectory, strFile);
                        Info = new FileInfo(strBackupFilePath);
                        if ((Info.IsReadOnly == true) && (File.Exists(strBackupFilePath)))
                            File.SetAttributes(strBackupFilePath, File.GetAttributes(strBackupFilePath) & ~FileAttributes.ReadOnly);
                        TransactionalFileManager.Copy(strInstallFilePath, strBackupFilePath, true);
                    }
                    TransactionalFileManager.Delete(strInstallFilePath);
                }
            }
            TransactionalFileManager.WriteAllBytes(strInstallFilePath, p_bteData);
            // Checks whether the file is a gamebryo plugin
            if (IsPlugin)
                if (PluginManager.IsActivatiblePluginFile(strInstallFilePath))
                    PluginManager.AddPlugin(strInstallFilePath);
            InstallLog.AddDataFile(Mod, p_strPath);
            return IsPlugin;
        }

        /// <summary>
        /// Uninstalls the specified file.
        /// </summary>
        /// <remarks>
        /// unsupported
        /// </remarks>
        /// <param name="p_strPath">The path to the file that is to be uninstalled.</param>
        /// <param name="p_booSecondaryInstallPath">Whether to use the secondary install path.</param>
        public bool UninstallDataFile(string p_strPath)
        {
          // NOP - Not supported in CLI version
          return false;
        }

        /// <summary>
        /// Deletes any empty directories found between the start path and the end directory.
        /// </summary>
        /// <param name="p_strStartPath">The path from which to start looking for empty directories.</param>
        /// <param name="p_strStopDirectory">The directory at which to stop looking.</param>
        protected void TrimEmptyDirectories(string p_strStartPath, string p_strStopDirectory)
        {
            string strEmptyDirectory = p_strStartPath;
            while (true)
            {
                if (Directory.Exists(strEmptyDirectory) &&
                    (Directory.GetFiles(strEmptyDirectory).Length + Directory.GetDirectories(strEmptyDirectory).Length == 0) &&
                    !strEmptyDirectory.Equals(p_strStopDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    for (Int32 i = 0; i < 5 && Directory.Exists(strEmptyDirectory); i++)
                        FileUtil.ForceDelete(strEmptyDirectory);
                }
                else
                    break;
                strEmptyDirectory = Path.GetDirectoryName(strEmptyDirectory);
            }
        }

        /// <summary>
        /// Finalizes the installation of the files.
        /// </summary>
        public virtual void FinalizeInstall()
        {
            SevenZipExtractor extractor = new SevenZipExtractor(Mod.ModArchivePath);
            List<int> indexes = new List<int>();
            foreach (var entry in TargetFiles) {
                indexes.Add(entry.Value.Index);
            }
            indexes.Sort();
            extractor.ExtractFiles(ExtractPath, indexes.ToArray());
            extractor.Dispose();
            foreach (var entry in TargetFiles)
            {
                if (File.Exists(Path.Combine(ExtractPath, entry.Value.FileName)))
                {
                    byte[] bteModFile = File.ReadAllBytes(Path.Combine(ExtractPath, entry.Value.FileName));
                    GenerateDataFile(entry.Key, bteModFile);
                }
            }
        }
        
        public List<string> InstallErrors {
            get {
                return new List<string>();
            }
        }

        public bool PluginCheck(string p_strPath, bool p_booRemove)
        {
          throw new NotImplementedException();
        }
    }
}
