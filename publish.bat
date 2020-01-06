set _NMMPATH=%~dp0..\Nexus-Mod-Manager
set _NMMCLIPATH=%~dp0
set outputPath=..\..\install\bin

mkdir  "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\AntlrUtil.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\ChinhDo.Transactions.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\Commanding.dll" "%outputPath%\NCC"
rem copy %_NMMPATH%\Stage\Release\GamebryoBase.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\ICSharpCode.TextEditor.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\ModManager.Interface.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\Mods.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\NexusClient.Interface.dll" "%outputPath%\NCC"
rem copy %_NMMPATH%\Stage\Release\NexusClientCLI.exe.manifest" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\ObjectListView.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\RestSharp.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\Scripting.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\SevenZipSharp.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\Transactions.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\UI.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\Util.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\WeifenLuo.WinFormsUI.Docking.dll" "%outputPath%\NCC"
rem stored in repository in binary form
copy "%_NMMPATH%\Stage\Release\Castle.Core.dll" "%outputPath%\NCC"

mkdir  "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\Stage\Release\GameModes\GamebryoBase*.dll" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\Stage\Release\GameModes\Enderal*.dll" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\Stage\Release\GameModes\Fallout*.dll" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\Stage\Release\GameModes\Morrowind*.dll" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\Stage\Release\GameModes\Oblivion*.dll" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\Stage\Release\GameModes\Skyrim*.dll" "%outputPath%\NCC\GameModes"

rem we should reimplement the dummy classes eventually
mkdir  "%outputPath%\NCC\GameModes\data"
copy "%_NMMPATH%\Stage\Release\GameModes\data\boss32.dll" "%outputPath%\NCC\GameModes\data"
copy "%_NMMPATH%\Stage\Release\GameModes\data\boss64.dll" "%outputPath%\NCC\GameModes\data"
copy "%_NMMPATH%\Stage\Release\GameModes\data\loot32.dll" "%outputPath%\NCC\GameModes\data"
copy "%_NMMPATH%\Stage\Release\GameModes\data\loot64.dll" "%outputPath%\NCC\GameModes\data"

mkdir "%outputPath%\NCC\ModFormats"
copy "%_NMMPATH%\Stage\Release\ModFormats\FOMod.dll" "%outputPath%\NCC\ModFormats"
copy "%_NMMPATH%\Stage\Release\ModFormats\OMod.dll" "%outputPath%\NCC\ModFormats"

mkdir  "%outputPath%\NCC\ScriptTypes"
copy "%_NMMPATH%\Stage\Release\ScriptTypes\Antlr*.dll" "%outputPath%\NCC\ScriptTypes"
copy "%_NMMPATH%\Stage\Release\ScriptTypes\CSharpScript.dll" "%outputPath%\NCC\ScriptTypes"
copy "%_NMMPATH%\Stage\Release\ScriptTypes\ModScript.dll" "%outputPath%\NCC\ScriptTypes"
copy "%_NMMPATH%\Stage\Release\ScriptTypes\XmlScript.dll" "%outputPath%\NCC\ScriptTypes"

mkdir  "%outputPath%\NCC\data"
copy "%_NMMPATH%\Stage\Release\data\7z-32bit.dll" "%outputPath%\NCC\data"
copy "%_NMMPATH%\Stage\Release\data\7z-64bit.dll" "%outputPath%\NCC\data"

rem copy the CLI binaries

copy "%_NMMPATH%\Stage\Release\NexusClientCLI.exe" "%outputPath%\NCC"
copy "%_NMMPATH%\Stage\Release\NexusClientCLI.exe.config" "%outputPath%\NCC"

goto end

:end
exit 0
