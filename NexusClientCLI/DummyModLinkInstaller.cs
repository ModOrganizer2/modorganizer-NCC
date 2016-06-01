using Nexus.Client.ModManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Client.CLI
{
  class DummyModLinkInstaller : IModLinkInstaller
  {
    public string AddFileLink(Mods.IMod p_modMod, string p_strBaseFilePath, string p_strSourceFile, bool p_booIsSwitching)
    {
			return AddFileLink(p_modMod, p_strBaseFilePath, p_strSourceFile, p_booIsSwitching, false);
    }

    public string AddFileLink(Mods.IMod p_modMod, string p_strBaseFilePath, string p_strSourceFile, bool p_booIsSwitching, bool p_booHandlePlugin)
    {
      // nop, linking is done by MO
      return string.Empty;
    }
  }
}
