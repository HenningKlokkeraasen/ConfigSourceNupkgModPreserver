Visual Studio extension for preserving modifications to a generated web.config file done by NuGet packages, when you have a source web.config file that is transformed.

Solves the following scenario:
* You have a project set up with transformation of a web.config file from e.g. \Configurations\web.config to \web.config
* You install/update/uninstall a NuGet package
* The NuGet package modifies \web.config
* You never see the modifications, because you either have \web.config ignored or you build the solution before seeing the diff causing \web.config to be overwritten

The extension hooks into NuGet events, and prompts you if you want to run the preservation.
The preservation is done by calling _git merge-file_ with \Configurations\web.config as the current-file and \web.config as the other-file, causing changes in \web.config to be merged into \Configurations\web.config.
You must handle the resulting merge manually.

Prerequisites:
* git must be installed
* \Configurations\web.config and \web.config must be similar (have several runs of the same lines), in order for git to be able to perform the merge-file algorithm - which should be fine as the latter is a result of the former