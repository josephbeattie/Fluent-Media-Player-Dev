﻿using Microsoft.Toolkit.Mvvm.Input;

namespace Rise.App.ViewModels.FileBrowser
{
    public sealed class FileBrowserBreadcrumbItemViewModel : BreadcrumbItemViewModel
    {
        public string? Path { get; }

        public FileBrowserBreadcrumbItemViewModel(string sectionName, IRelayCommand? itemClickedCommand, string? path)
            : base(sectionName, itemClickedCommand)
        {
            this.Path = path;
        }
    }
}