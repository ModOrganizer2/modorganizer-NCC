import os

script_dir = os.path.dirname(__file__)

from buildtools import log
from buildtools.buildsystem.visualstudio import VisualStudio2015Solution, ProjectType

target=os.path.join(script_dir, '..', 'NMM','NexusClient.sln')
csprojfile = os.path.join('..','NexusClientCli','NexusClientCLI','NexusClientCLI.csproj')

sln = VisualStudio2015Solution()
sln.LoadFromFile(target)
changed=False
if 'NexusClientCli' not in sln.projectsByName:
	proj = sln.AddProject('NexusClientCli',ProjectType.CSHARP_PROJECT,csprojfile)
	log.info('Added %s (%s) to %s', proj.name, proj.guid, target)
	changed=True
else:
	proj = sln.projectsByName['NexusClientCli']
	log.info('Project %s (%s) already exists in %s', proj.name, proj.guid, target)
	if proj.projectfile != csprojfile:
		oprojfile = proj.projectfile
		proj.projectfile = csprojfile
		log.info('Fixed CSPROJ location: %s -> %s',oprojfile,csprojfile)
		changed=True
if changed:
	sln.SaveToFile(target+'.patched')