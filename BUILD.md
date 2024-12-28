# Building the project
Start by cloning the repository:

```
git clone https://github.com/Rise-Software/Rise-Media-Player.git
```

Rise Media Player uses a single solution, where all the projects (including the app itself) live. To open it, you'll need Visual Studio 2019 or later with the UWP workload, and the Windows 11 SDK (10.0.22000.0).

## Pre-build steps
Before being able to build the app, you need to take care of a few things.

If you haven't set up the last.fm constants yet, you need to in order to build the app. See below for instructions.

### Adding last.fm constants
To build the app, you'll need to add a few last.fm related constants. On the `Rise.Common` project, there's a folder named `Constants`. Add a new file called `LastFM.cs` and paste the following:

```C#
namespace Rise.Common.Constants
{
    /// <summary>
    /// Contains last.fm related constants.
    /// </summary>
    public class LastFM
    {
        public const string Key = "YourAPIKey";
        public const string Secret = "YourSecret";

        public const string VaultResource = "RiseMP - LastFM account";
    }
}
```

This will be enough to build the app, but if you want last.fm support, you should [get a last.fm API key](https://www.last.fm/api#getting-started). After doing this, replace the value of the `Key` and `Secret` constants with your own API key and secret key respectively. After doing this, last.fm functionality should be enabled.

## Deploying
If `Rise.App` isn't the startup project already, set it as that. You can then deploy the app by pressing the "Start Debugging" button.
