To build NCC, check out Nexus Mod Manager source to this directory. Than open the solution in Visual Studio,
add NexusClientCli and BossDummy projects and compile both.
Afterwards, NCC is placed in bin/Release. Rename "BossDummy.dll" under "bin/release/" to "boss32.dll" so it takes the
place of the original file.