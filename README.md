<h1 align="center">
  <br>
  <img src="https://raw.githubusercontent.com/Astropilot/PBPModSwitch/master/images/bpb.jpg" alt="Penumbra Black Plague" width="400">
</h1>

<h4 align="center">
Penumbra Black Plague - Mod Switch</h4>

<p align="center">
  <a href="https://github.com/Astropilot/PBPModSwitch/releases/latest"><img src="https://img.shields.io/github/release/Astropilot/PBPModSwitch.svg" alt="Version"></a>
  <a href="https://github.com/Astropilot/PBPModSwitch/issues">
    <img src="https://img.shields.io/github/issues/Astropilot/PBPModSwitch"
         alt="Issues">
  </a>
  <a href="https://github.com/Astropilot/PBPModSwitch/pulls">
    <img src="https://img.shields.io/github/issues-pr-raw/Astropilot/PBPModSwitch"
         alt="Issues">
  </a>
  <img src="https://img.shields.io/badge/Made%20with-%E2%9D%A4%EF%B8%8F-yellow.svg">
</p>

<p align="center">
  <a href="#about">About</a> •
  <a href="#usage">Usage</a> •
  <a href="#contributing">Contributing</a> •
  <a href="#authors">Authors</a> •
  <a href="#license">License</a>
</p>

## About

This repository contains the C# project of this utility.

This utility was developed with OOB maps in mind. The goal is to be able to switch from official maps to OOB maps without manipulating folders (including the game cache) and without having to restart the game.

This tool supports the following versions of Penumbra Black Plague:
* The Steam version
* The GOG version with the 1.1 patch

## Usage

* First of all download the latest version (compressed archive) you can find [here](https://github.com/Astropilot/PBPModSwitch/releases/latest).
* Then move the files and the `mods` folder to your game folder in the same place as Penumbra.exe (in the `redist` folder). The game path can be found via Steam: Properties -> Local files -> Browse local files...<br>
The path generally used by Steam is `C:\Program Files (x86)\Steam\steamapps\common\Penumbra Black Plague\redist`.<br>
* You can run PBPModSwitch.exe then Penumbra but it also works the other way around!
* A few seconds after the game is initialized the utility should load the maps
* Once the interface with the list of modded maps appears, you can activate all the modded maps with the main button or select the ones you want from the list below. Unchecking them will return the normal maps.
* **Note**: This program will create a secondary cache for the game so as not to pollute the real game cache with files from modded maps. This cache is automatically deleted when you close the utility and the modded maps are automatically disabled.

## Contributing

I would be happy to see you contribute to the development of this tool!

You can open a issue in case of a problem or fork this repository to propose a modification via a pull request.

## Authors

|               | Github profile                                        | Discord                                             |
|---------------|:-----------------------------------------------------:|:---------------------------------------------------:|
| Astropilot    | [Astropilot](https://github.com/Astropilot)           | [Anos]#2347                                     |

## License

MIT - See LICENSE file
