# JerusalemLauncher
This launcher fixes the problem of being unable to save while playing [Jerusalem](https://en.wikipedia.org/wiki/Jerusalem:_The_Three_Roads_to_the_Holy_Land) 
on modern systems.

## Usage

Place JerusalemLauncher.exe in the same folder of the game and use it to play, each time you go back to the main menu while playing a new save will be created.

## How it works

The game actually has an autosave system: each time you access the main menu, it saves the current game on `Data\Saves\SaveGame.ars`, 
so this launcher creates a new save, named `1_ArSa#.ars` (`#` is an autoincremented integer), in the same folder, when it happens.

The list of the saved games is located in `Data\saves\user.aba` and has this format:
- 4 bytes for the number of saved games;
- a list of saves in this format:
- - 4 bytes for the length of name shown on the load screen;
- - `n` bytes for name shown of the load screen;
- - 4 bytes for the length of the name shown on the load screen (again);
- - `n` bytes for the name shown on the load screen (again);
- - 4 bytes for the length of the save file without extension (`1_ArSa#`);
- - `n` bytes for the name of the save file without extension (`1_ArSa#`);
