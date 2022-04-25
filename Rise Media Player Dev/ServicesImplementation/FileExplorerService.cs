﻿using System;
using System.Threading.Tasks;
using Rise.App.Services;
using Windows.System;

namespace Rise.App.ServicesImplementation
{
    internal sealed class FileExplorerService : IFileExplorerService
    {
        public async Task<bool> OpenPathInFileExplorerAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            return await Launcher.LaunchFolderPathAsync(path);
        }
    }
}
