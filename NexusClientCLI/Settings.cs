using System;
using Nexus.Client.Settings;

namespace Nexus.Client.CLI.Properties
{
    /// <summary>
    /// This class adds the <see cref="ISettings"/> to the project's <see cref="Properties.Settings"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// This file should not contain any memebers or properties.
    /// </remarks>
    internal sealed partial class Settings : ISettings
    {
        public DateTime LastCheckedMissingDownloadID
        {
            get
            {
                return DateTime.Now; // idfk, doesn't seem used as of 11/15/2016 - N3X
            }

            set
            {
                //nop
            }
        }

        public int MaxConcurrentDownloads
        {
            get
            {
                return 1; // idfk, value set for toasters - N3X
            }

            set
            {
                // nop
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

        public bool SkyrimSEFirstInstallWarning
        {
            get
            {
                return true; // Avoid any popups.  Yeees, we totally showed that popup! - N3X
            }

            set
            {
                // nop
            }
        }
    }
}
