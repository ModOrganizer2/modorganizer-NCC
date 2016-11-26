set _NMMPATH=%~dp0..\nmm
set _NMMCLIPATH=%~dp0
set outputPath=..\..\..\install\bin

mkdir  "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\ChinhDo.Transactions.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\Commanding.dll" "%outputPath%\NCC"
rem copy %_NMMPATH%\bin\Release\GamebryoBase.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\ICSharpCode.TextEditor.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\ModManager.Interface.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\Mods.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\NexusClient.Interface.dll" "%outputPath%\NCC"
rem copy %_NMMPATH%\bin\Release\NexusClientCLI.exe.manifest" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\ObjectListView.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\RestSharp.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\Scripting.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\SevenZipSharp.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\Transactions.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\UI.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\Util.dll" "%outputPath%\NCC"
copy "%_NMMPATH%\bin\Release\WeifenLuo.WinFormsUI.Docking.dll" "%outputPath%\NCC"
rem stored in repository in binary form
copy "%_NMMPATH%\bin\Release\Castle.Core.dll" "%outputPath%\NCC"

mkdir  "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\bin\Release\GameModes\Fallout3.*" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\bin\Release\GameModes\Fallout4.*" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\bin\Release\GameModes\FalloutNV.*" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\bin\Release\GameModes\Skyrim.*" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\bin\Release\GameModes\SkyrimSE.*" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\bin\Release\GameModes\Oblivion.*" "%outputPath%\NCC\GameModes"
copy "%_NMMPATH%\bin\Release\GameModes\GamebryoBase.dll" "%outputPath%\NCC\GameModes"

mkdir  "%outputPath%\NCC\GameModes\data"
rem copy %_NMMPATH%\..\bin\Release\BossDummy.dll" "%outputPath%\NCC\GameModes\data\boss32.dll"

mkdir "%outputPath%\NCC\ModFormats"
copy "%_NMMPATH%\bin\Release\ModFormats\FOMod.dll" "%outputPath%\NCC\ModFormats"
copy "%_NMMPATH%\bin\Release\ModFormats\OMod.dll" "%outputPath%\NCC\ModFormats"

mkdir  "%outputPath%\NCC\ScriptTypes"
copy "%_NMMPATH%\bin\Release\ScriptTypes\Antlr*.dll" "%outputPath%\NCC\ScriptTypes"
copy "%_NMMPATH%\bin\Release\ScriptTypes\CSharpScript.dll" "%outputPath%\NCC\ScriptTypes"
copy "%_NMMPATH%\bin\Release\ScriptTypes\ModScript.dll" "%outputPath%\NCC\ScriptTypes"
copy "%_NMMPATH%\bin\Release\ScriptTypes\XmlScript.dll" "%outputPath%\NCC\ScriptTypes"

mkdir  "%outputPath%\NCC\data"
copy "%_NMMPATH%\bin\Release\data\7z-32bit.dll" "%outputPath%\NCC\data"
copy "%_NMMPATH%\bin\Release\data\7z-64bit.dll" "%outputPath%\NCC\data"

rem copy the CLI binaries

copy "%_NMMCLIPATH%bin\Release\NexusClientCLI.exe" "%outputPath%\NCC"
copy "%_NMMCLIPATH%bin\Release\NexusClientCLI.exe.config" "%outputPath%\NCC"

goto end

:end
exit 0
