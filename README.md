# Z-Toolbar Electron/Blazor
Z-Toolbar is a productivity toolbar written in Electron.NET (.NET5) and Blazor Web Assembly as a desktop app.

## Features:
- Global hook hotkey combination <kbd>Windows + \`</kbd> for showing and hiding the app.
- Tray icon with an option for exit (app not visible in the taskbar).
- Add files and/or folders by drag and drop functionality.
- Options include:
    - Run
    - Show info (for showing file info like name, path, and id (might add more info later))
    - Show in explorer
    - Remove
    - Open dev tools
    - About 
- Pinned files and folders are saved as JSON in the app folder.
- Show info for multiple items at once.
- Remove multiple items at once.
- Hold <kbd>Ctrl</kbd> to enter select mode.
- Select all with <kbd>Ctrl + A</kbd>.
- When showing file or folder info you can copy a specific row of info by pressing the row.

## Start the Application

To start the application make sure you have installed the "[ElectronNET.CLI](https://www.nuget.org/packages/ElectronNET.CLI/)" packages as a global tool:

```
dotnet tool install ElectronNET.CLI -g
```
Then, to start the application run the following command in the `Server` project folder:
```
electronize start
```

## Build
You can build this project to a packaged installer by using this command in the `Server` project folder:

```
electronize build /target "win10-x86" /electron-arch ia32 /PublishSingleFile false /PublishReadyToRun false
```
The output files can be found here:
`Server Project Folder\bin\Desktop`

## Demo
