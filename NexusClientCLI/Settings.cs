﻿namespace Nexus.Client.CLI.Properties
{
  using System.Diagnostics;
  using System.IO;
  using System.Threading;

  using Client.Settings;
  /// <summary>
  /// This class adds the <see cref="ISettings"/> to the project's <see cref="Properties.Settings"/>
  /// class.
  /// </summary>
  /// <remarks>
  /// This file should not contain any memebers or properties.
  /// </remarks>
  internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase, ISettings
  {
    private static readonly object SettingsFileLock = new object();
    private const int PauseBetweenAttemptsMilliseconds = 500;
    private const int RetryAttempts = 3;

    /// <summary>
    /// A thread-safe call to save the current settings to file.
    /// </summary>
    public override void Save()
    {
      lock (SettingsFileLock)
      {
        var success = false;

        /* Realistically a while loop can be used, but in the interest of pessimism it
         * is better to err on the side of caution and not potentially create a
         * deadlock situation by accident (very small chance of this). */
        for (var i = 0; i < RetryAttempts; i++)
        {
          /* Normally programming by exception is a bad idea, but when it
           * comes to File IO there isn't much of a choice it seems:
           * https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use/876513#876513 */

          try
          {
            //Can throw System.IOException due to concurrent file access attempts
            base.Save();

            success = true;

            break;
          }
          catch (IOException)
          {
            //If an IO Exception does occur wait a little between attempts
            Thread.Sleep(PauseBetweenAttemptsMilliseconds);
          }
        }

        if (!success)
        {
          Trace.TraceWarning($"All {RetryAttempts} attempts made to save the configuration file have failed.");
        }
      }
    }

    /// <summary>
    /// Gets the full name of the mod manager.
    /// </summary>
    /// <value>The full name of the mod manager.</value>
    public string ModManagerName
    {
      get
      {
        return "NMM CLI";
      }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public global::System.DateTime LastCheckedMissingDownloadID
    {
      get
      {
        return ((global::System.DateTime)(this["LastCheckedMissingDownloadID"]));
      }
      set
      {
        this["LastCheckedMissingDownloadID"] = value;
      }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("5")]
    public int MaxConcurrentDownloads
    {
      get
      {
        return ((int)(this["MaxConcurrentDownloads"]));
      }
      set
      {
        this["MaxConcurrentDownloads"] = value;
      }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool SkyrimSEFirstInstallWarning
    {
      get
      {
        return ((bool)(this["SkyrimSEFirstInstallWarning"]));
      }
      set
      {
        this["SkyrimSEFirstInstallWarning"] = value;
      }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<KeyedSettingsOfBoolean />")]
    public global::Nexus.Client.Settings.KeyedSettings<bool> AddShellExtensions
    {
      get
      {
        return ((global::Nexus.Client.Settings.KeyedSettings<bool>)(this["AddShellExtensions"]));
      }
      set
      {
        this["AddShellExtensions"] = value;
      }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool AssociateWithUrl
    {
      get
      {
        return ((bool)(this["AssociateWithUrl"]));
      }
      set
      {
        this["AssociateWithUrl"] = value;
      }
    }
  }
}
