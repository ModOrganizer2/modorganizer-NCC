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
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using ChinhDo.Transactions;
using SevenZip;
using Nexus.Client.Games;
using Nexus.Client.Util;
using Nexus.Client.Mods;
using Nexus.Client.UI;
using Nexus.Client.ModManagement.Scripting;
using Nexus.Client.ModManagement;
using Nexus.Client.ModManagement.InstallationLog;
using Nexus.Client.PluginManagement;
using Nexus.Client.Settings;
using Nexus.Client.BackgroundTasks;


namespace Nexus.Client.CLI
{
    class ScriptRunner : ThreadedBackgroundTask
    {
        private IScriptExecutor m_Executor;
        private IScript m_Script;

        public ScriptRunner(IScriptExecutor executor, IScript script)
        {
            m_Executor = executor;
            m_Script = script;
        }

        public bool Execute()
        {
            Start();
            return true;
        }

        protected override object DoWork(object[] p_objArgs)
        {
            Thread.Sleep(0); // ensures that the main thread gets an opportunity to start the main loop
            try {
            	return m_Executor.Execute(m_Script);
            } catch (Exception e) {
            	MessageBox.Show(e.Message, "Installation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            	return null;
            }
        }
    }

    class Program
    {
        /// <summary>
        /// Creates a mod of the appropriate type from the specified file.
        /// Code copy&pasted from class ModRegistry
        /// </summary>
        /// <param name="p_strModPath">The path to the mod file.</param>
        /// <returns>A mod of the appropriate type from the specified file, if the type of hte mod
        /// can be determined; <c>null</c> otherwise.</returns>
        private static IMod CreateMod(string modPath, IModFormatRegistry formatRegistry)
        {
            List<KeyValuePair<FormatConfidence, IModFormat>> lstFormats = new List<KeyValuePair<FormatConfidence, IModFormat>>();
            foreach (IModFormat mftFormat in formatRegistry.Formats)
                lstFormats.Add(new KeyValuePair<FormatConfidence, IModFormat>(mftFormat.CheckFormatCompliance(modPath), mftFormat));
            lstFormats.Sort((x, y) => -x.Key.CompareTo(y.Key));
            if (lstFormats[0].Key <= FormatConfidence.Convertible)
            {
                Console.WriteLine("failed to determine format for " + modPath);
                return null;
            }
            return lstFormats[0].Value.CreateMod(modPath);
        }

        static int DoInstall(string game, string filename, string installationPath, string pluginsFile, ref string errorString)
        {

            if (game == null)
            {
                errorString = "no game specified";
                return 1;
            }
            if (filename == null)
            {
                errorString = "no file specified";
                return 1;
            }
            if (pluginsFile == null)
            {
                errorString = "no plugin file specified";
                return 1;
            }
            try
            {
                EnvironmentInfo environmentInfo = new EnvironmentInfo(Properties.Settings.Default);

                string exeLocation = Assembly.GetExecutingAssembly().Location;
                string exePath = System.IO.Path.GetDirectoryName(exeLocation);
                string[] gameModes = Directory.GetFiles(Path.Combine(exePath, "GameModes"), String.Format("{0}.dll", game));
                if (gameModes.Count() == 0)
                {
                    errorString = "unknown game";
                    return 1;
                }

                Assembly gameAssembly = Assembly.LoadFrom(gameModes[0]);
                IGameModeFactory gameModeFactory = null;
                Type[] exportedTypes = gameAssembly.GetExportedTypes();
                foreach (Type type in exportedTypes)
                {
                    if (typeof(IGameModeFactory).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(EnvironmentInfo) });
                        gameModeFactory = (IGameModeFactory)constructor.Invoke(new Object[] { environmentInfo });
                    }
                }
                if (gameModeFactory == null)
                {
                    errorString = "invalid game";
                    return 1;
                }

                string str7zPath = Path.Combine(environmentInfo.ProgrammeInfoDirectory, environmentInfo.Is64BitProcess ? "7z-64bit.dll" : "7z-32bit.dll");
                SevenZipCompressor.SetLibraryPath(str7zPath);

                FileUtil fileUtil = new NexusFileUtil(environmentInfo);

                environmentInfo.Settings.InstallationPaths[gameModeFactory.GameModeDescriptor.ModeId] = installationPath;
                environmentInfo.Settings.ModFolder[gameModeFactory.GameModeDescriptor.ModeId] = installationPath;
                environmentInfo.Settings.InstallInfoFolder[gameModeFactory.GameModeDescriptor.ModeId] = environmentInfo.TemporaryPath;
                if (environmentInfo.Settings.DelayedSettings[gameModeFactory.GameModeDescriptor.ModeId] == null)
                    environmentInfo.Settings.DelayedSettings[gameModeFactory.GameModeDescriptor.ModeId] = new KeyedSettings<string>();
                if (environmentInfo.Settings.DelayedSettings["ALL"] == null)
                    environmentInfo.Settings.DelayedSettings["ALL"] = new KeyedSettings<string>();

                ViewMessage warning = null;
                IGameMode gameMode = gameModeFactory.BuildGameMode(fileUtil, out warning);

                IModCacheManager cacheManager = new NexusModCacheManager(environmentInfo.TemporaryPath, gameMode.GameModeEnvironmentInfo.ModDirectory, fileUtil);

                IScriptTypeRegistry scriptTypeRegistry = ScriptTypeRegistry.DiscoverScriptTypes(Path.Combine(Path.GetDirectoryName(exeLocation), "ScriptTypes"), gameMode);
                if (scriptTypeRegistry.Types.Count == 0)
                {
                    errorString = "No script types were found.";
                    return 2;
                }

                IModFormatRegistry formatRegistry = ModFormatRegistry.DiscoverFormats(cacheManager, scriptTypeRegistry, Path.Combine(Path.GetDirectoryName(exeLocation), "ModFormats"));
                if (formatRegistry.Formats.Count == 0)
                {
                    errorString = "No formats were found.";
                    return 2;
                }

                // we install the mod from the temporary path. Unfortunately this requires the archive to be copied, otherwise the sandbox will
                // prevent the installer script from accessing the archive in its original location
                string fileNameTemporary = Path.Combine(environmentInfo.TemporaryPath, Path.GetFileName(filename));
                File.Copy(filename, fileNameTemporary);

                IMod mod = CreateMod(fileNameTemporary, formatRegistry);
                if (mod == null)
                {
                    errorString = "failed to initialize mod";
                    return 3;
                }

                if (mod.HasInstallScript)
                {
                    IDataFileUtil dataFileUtility = new DataFileUtil(gameMode.GameModeEnvironmentInfo.InstallationPath);
                    TxFileManager fileManager = new TxFileManager();
                    IInstallLog installLog = new DummyInstallLog();
                    IIniInstaller iniIniInstaller = new IniInstaller(mod, installLog, fileManager, delegate { return OverwriteResult.No; });
                    IPluginManager pluginManager = new DummyPluginManager(pluginsFile, gameMode, mod);
                    IGameSpecificValueInstaller gameSpecificValueInstaller = gameMode.GetGameSpecificValueInstaller(mod, installLog, fileManager, new NexusFileUtil(environmentInfo), delegate { return OverwriteResult.No; });
                    IModFileInstaller fileInstaller = new ModFileInstaller(gameMode.GameModeEnvironmentInfo, mod, installLog, pluginManager, dataFileUtility, fileManager, delegate { return OverwriteResult.No; }, false);
                    InstallerGroup installers = new InstallerGroup(dataFileUtility, fileInstaller, iniIniInstaller, gameSpecificValueInstaller, pluginManager);
                    IScriptExecutor executor = mod.InstallScript.Type.CreateExecutor(mod, gameMode, environmentInfo, installers, SynchronizationContext.Current);
//                    mod.BeginReadOnlyTransaction(fileUtil);
                    // run the script in a second thread and start the main loop in the main thread to ensure we can handle message boxes and the like
                    ScriptRunner runner = new ScriptRunner(executor, mod.InstallScript);

                    runner.Execute();
                    runner.TaskEnded += delegate
                    {
                        iniIniInstaller.FinalizeInstall();
                        gameSpecificValueInstaller.FinalizeInstall();
//                        mod.EndReadOnlyTransaction();
                        Application.Exit();
                    };

                    Application.Run();
                    switch (runner.Status)
                    {
                        case BackgroundTasks.TaskStatus.Cancelled: return 11;
                        case BackgroundTasks.TaskStatus.Error: return 6;
                        case BackgroundTasks.TaskStatus.Incomplete: return 10;
                        default: return 0;
                    }
                }
                else
                {
                    errorString = "no install script";
                    return 4;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("exception: " + e.Message);
                MessageBox.Show(e.Message, "Installation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 5;
            }
        }

        [STAThread]
        static int Main(string[] args)
        {
            string game = null;
            string filename = null;
            string installationPath = null;
            string pluginsFile = null;

            // determine action
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].ToLower().Equals("/g") || args[i].ToLower().Equals("-g"))
                {
                    if (args.Length - i <= 1)
                    {
                        Console.WriteLine("invalid number of parameters, expected game name");
                        return 1;
                    }
                    ++i;
                    game = args[i];
                }

                if (args[i].ToLower().Equals("/i") || args[i].ToLower().Equals("-i"))
                {
                    if (args.Length - i <= 2)
                    {
                        Console.WriteLine("invalid number of parameters, expected filename and installation path");
                        return 1;
                    }
                    filename = args[i + 1];
                    installationPath = args[i + 2];
                    i += 2;
                }

                if (args[i].ToLower().Equals("/p") || args[i].ToLower().Equals("-p"))
                {
                    if (args.Length - i <= 1)
                    {
                        Console.WriteLine("invalid number of parameters, expected plugins file");
                        return 1;
                    }
                    ++i;
                    pluginsFile = args[i];
                }
            }

            string errorString = "";
            int result = DoInstall(game, filename, installationPath, pluginsFile, ref errorString);
            if ((result != 0) && (errorString.Length != 0))
            {
                MessageBox.Show(errorString, "Installation Failed: " + result, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }
    }
}
