set _NMMPATH=%~dp0%NMM

mkdir ..\..\output\NCC
copy %_NMMPATH%\bin\Release\ChinhDo.Transactions.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\Commanding.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\GamebryoBase.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\ICSharpCode.TextEditor.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\ModManager.Interface.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\Mods.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\NexusClient.Interface.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\NexusClientCLI.exe ..\..\output\NCC
copy %_NMMPATH%\bin\Release\NexusClientCLI.exe.config ..\..\output\NCC
copy %_NMMPATH%\bin\Release\NexusClientCLI.exe.manifest ..\..\output\NCC
copy %_NMMPATH%\bin\Release\Scripting.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\SevenZipSharp.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\Transactions.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\UI.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\Util.dll ..\..\output\NCC
copy %_NMMPATH%\bin\Release\WeifenLuo.WinFormsUI.Docking.dll ..\..\output\NCC

mkdir ..\..\output\NCC\GameModes
copy %_NMMPATH%\bin\Release\GameModes\Fallout3.* ..\..\output\NCC\GameModes
copy %_NMMPATH%\bin\Release\GameModes\FalloutNV.* ..\..\output\NCC\GameModes
copy %_NMMPATH%\bin\Release\GameModes\Skyrim.* ..\..\output\NCC\GameModes
copy %_NMMPATH%\bin\Release\GameModes\Oblivion.* ..\..\output\NCC\GameModes
copy %_NMMPATH%\bin\Release\GameModes\GamebryoBase.dll ..\..\output\NCC\GameModes

mkdir ..\..\output\NCC\GameModes\data
echo copy bin\Release\BossDummy.dll ..\..\output\NCC\data\boss32.dll
echo copy bin\Release\BossDummy.dll ..\..\output\NCC\GameModes\data\boss32.dll
echo copy bin\Release\BossDummy.dll ..\..\output\NCC\data\boss64.dll
echo copy bin\Release\BossDummy.dll ..\..\output\NCC\GameModes\data\boss64.dll

mkdir ..\..\output\NCC\ModFormats
copy %_NMMPATH%\bin\Release\ModFormats\FOMod.dll ..\..\output\NCC\ModFormats
copy %_NMMPATH%\bin\Release\ModFormats\OMod.dll ..\..\output\NCC\ModFormats

mkdir ..\..\output\NCC\ScriptTypes
copy %_NMMPATH%\bin\Release\ScriptTypes\Antlr*.dll ..\..\output\NCC\ScriptTypes
copy %_NMMPATH%\bin\Release\ScriptTypes\CSharpScript.dll ..\..\output\NCC\ScriptTypes
copy %_NMMPATH%\bin\Release\ScriptTypes\ModScript.dll ..\..\output\NCC\ScriptTypes
copy %_NMMPATH%\bin\Release\ScriptTypes\XmlScript.dll ..\..\output\NCC\ScriptTypes

mkdir ..\..\output\NCC\data
copy %_NMMPATH%\bin\Release\data\7z-32bit.dll ..\..\output\NCC\data
copy %_NMMPATH%\bin\Release\data\7z-64bit.dll ..\..\output\NCC\data
