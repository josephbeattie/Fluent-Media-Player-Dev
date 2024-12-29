> [!IMPORTANT]
> After a long period of no development, RiseMP is back!
> The app is currently undergoing major changes and while contributions are welcome please be aware of merge conflicts.

<p align="center">
    <a href='https://github.com/Rise-Software/Rise-Media-Player'>
      <img src='https://user-images.githubusercontent.com/74561130/156649276-8dc63e37-bf76-4321-ae7a-4e77f2022c37.png' alt='RiseMP' />
    </a>
    <a href='https://www.microsoft.com/store/r/9PCSZTMTT55Z'>
      <img src='https://github.com/Rise-Software/Rise-Media-Player/assets/74561130/3d7edcaf-26d8-4453-a751-29b851721abd'alt='Get it from Microsoft' />
    </a>
    <a href='https://github.com/Rise-Software/Rise-Media-Player/releases/latest'>
      <img src='https://github.com/Rise-Software/Rise-Media-Player/assets/74561130/60deb402-0c8e-4579-80e6-69cb7b19cd43'alt='Get it from GitHub' />
    </a>
</p>

> [!NOTE]
> The app can only be installed from the store if you are an Insider. To become an insider, [click here](http://bit.ly/rise-insider).

Introducing **Rise Media Player**, a powerful music platform that brings your media to a whole new level.
If it's videos, music, discs or even your favourite streaming services - you're sure to love our player: with stunning design and an amazing storage layer that gives you the freedom of having one library for all of your content, combined with almost infinite customisability with settings that are second to none.
Stream, browse and explore - RiseMP can do it all.

Created with **WinUI and the latest design ideologies**, **Rise Media Player** is modern while keeping all of the classic features people need. We use **WinUI 2.8 Preview** to keep our user interface, clean, modern and consistent with **Windows 11 UI and UX** - although, the app does work on **Windows 10** too!
Our own controls and icons give users a truly personalised experience, being able to choose their own icon packs and with features like compact mode, you can use it on any *Windows device!*

## Features

* **Music and video playback**: Play music and videos from any source across your device and the internet in high quality
* **Sorting for songs, albums and videos**: Sort and find any track
* **Now Playing bar**: Stunningly designed "now playing" experience
* **Fullscreen music experience**: Comes complete with a beautiful fullscreen listening interface
* **`last.fm` integration**: Find your favourite tracks from `last.fm` in Rise
* **Internet based artist images**: Carefully curated artist images
* **Playlists**: Sort all your music into playlists
* **Modern Settings UI**: Customise your experience in RiseMP with themes, change the layout and modify your connected services
* **OneDrive support**: Browse music from your favourite cloud provider
* **Properties window**: View metadata attached to your songs
* **Colourful icons setting**: Customise your experience with icons with a Windows 11 style
* **Casting to devices, repeat, shuffle**: Cast music with ease anywhere in your home
* **Insider exclusives**: Exclusive wallpapers and feature sneak peeks for those enlisted in the programme
* **Pick up where you left off support**: Support for history and remembering exactly where you were when you last opened the app

<!-- Insider features
- RiseMP Designed Wallpapers and themes for your desktop
- Feature sneak peeks
-->

## Downloads

**If you are an Insider already, just click the Download button and you'll be taken to a page to download 😁**
In order to learn how to build RiseMP from source, check out [the documentation](./BUILD.md).

[![Download](https://user-images.githubusercontent.com/74561130/137598555-649c77c7-1719-4aa3-8017-8b41283de730.png)](https://github.com/Rise-Software/Rise-Media-Player/releases)    ![divide](https://user-images.githubusercontent.com/74561130/137599566-866fef7d-967e-4ad1-91da-8014d1752b93.png)    [![JoinInsider](https://user-images.githubusercontent.com/74561130/137585885-7f98b4de-5067-41ee-bdb4-2a04fea4b90a.png)](http://www.bit.ly/risesoftinsider)

### Building from source

#### Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) with the following individual components:
  - Windows SDK
  - Windows app development workload
- Git for Windows

```sh
git clone https://github.com/Rise-Software/Rise-Media-Player.git
```

#### Prepare credentials

Create a file called `LastFM.cs` in `Rise.Common.Constants`, and paste the following contents:

```cs
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

#### Build the project

To build Rise Media Player for development, open the `RiseMP.sln` item in Visual Studio. Right-click on the `Rise.App` packaging project in solution explorer and select ‘Set as Startup item’. Then press <kbd>F5</kbd> or your selected build & deploy keybind to build the app and launch it.

## Contributing

Want to contribute to this project? Let us know with an [issue](https://github.com/Rise-Software/Rise-Media-Player/issues) that communicates your intent to create a [pull request](https://github.com/Rise-Software/Rise-Media-Player/pulls).
<!-- Add when actual project added to repo
Looking for a place to start? Check out the [task board](https://github.com/orgs/RiversideValley/projects/8/views/2), where you can sort tasks by size and priority.
-->
## Credits

* [**Joseph Beattie (@josephbeattie)**](https://github.com/josephbeattie): **Founder of Rise and lead developer**
* [**Omar Salas (@YourOrdinaryCat)**](https://github.com/yourordinarycat): **Former member**
* [**SimpleBear (@itsWindows11)**](https://github.com/itswindows11): **Former member**

Without this great development team we wouldn't be able to ship new releases so go help them out however you can!

---

<p align="center">
    <a href='https://github.com/Rise-Software/Rise-Media-Player'>
      <img src="https://github.com/Rise-Software/Rise-Media-Player/assets/74561130/67a1ea8f-1d0f-4c1d-9688-1912ec20f779" alt="RiseMP" />
    </a>
</p>
