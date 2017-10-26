# NGGameSim
2017-2018 Senior Projects CU Boulder

This project is an attempt to create a simulation environment for testing C4ISR algorithms. The simulation pits two teams against each other, both with algorithms written to run completely autonomously, with a tank and a drone. The goal is to use the drone to search out the enemy tank, and communicate with the friendly tank to destroy the enemy tank.

This project is sponsered by Northrop Grumman, with the intent to use it in a competition in the Engineering Week Challenge. The project is developed by a team of 5 students at the University of Colorado at Boulder, referred to as the "NGGameSim Team" within the code and licensing.


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

## Contributing
When contributing to this repo, please follow the conventions used throughout the code. Development is done using the fork/pull model. When using this model with Protobuild, please remember to run the `clean` command *before* switching branches, or else the changes to the project definitions could have unintended consequences.

## Licensing
The files contained within this repo are licensed under two licenses. The full text of these licenses can be found in the LICENSE-CODE and LICENSE-ASSETS files within this repository. These licenses are:
* [GNU GPL v3](https://choosealicense.com/licenses/gpl-3.0/) - For all code files
* [CC-BY-SA-4.0](https://choosealicense.com/licenses/cc-by-sa-4.0/) - For all non-code files, such as 3D models, textures, ect...

Unless otherwise stated, all files contained within this repository implictly fall under one of these two licenses.