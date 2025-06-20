﻿global using System;
global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;

namespace CopyFileContents;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "Copy File Contents", "General", 0, 0, true)]
[ProvideProfile(typeof(OptionsProvider.GeneralOptions), "Copy File Contents", "General", 0, 0, true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.guidCopyFileContentsString)]
public sealed class CopyFileContentsPackage : ToolkitPackage {

	protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
		await this.RegisterCommandsAsync();
	}
}