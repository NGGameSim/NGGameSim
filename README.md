# NGGameSim
Game Simulation for Northrop Grumman, 2017-2018 Senior Projects CU Boulder


### Links
* [Waffle Board](https://waffle.io/NGGameSim/NGGameSim)


## Building
The projects are built using Protobuild, allowing easy text-based cross-platform project definition files. In order to contribute, follow these steps:
1. Run `Protobuild.exe --generate <platform>`, where `<platform>` is one of: "Windows", "Linux", or "MacOS".
    * If you are on a non-Windows platform, you will need to add `mono` before the Protobuild.exe call.
2. Open the generated '.sln' file in your favorite IDE, and develop as necessary.
3. If you have added/removed files, or changed project settings, you will have to run `Protobuild.exe --sync` to update the project definition files with the changes you made.
    * Again, non-Windows users will need to add `mono` before the Protobuild.exe call.
    * If you forget to sync before deleting the solution file, your new files will remain on disk, but all file changes to the project will not be saved and will need to be redone.
4. If you ever want to clean up the generated files, run `Protobuild.exe --clean`