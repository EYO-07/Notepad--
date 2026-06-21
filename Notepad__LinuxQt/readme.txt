// GUIDE - Notepad--LinuxQt 

// Installing Dependencies (qt6, qsciscintilla)
1. For Debian-Linux or Debian based using apt package manager:
Execute in terminal `bash DebianBased_Install_Dependencies.sh`
    or activate the script and execute './DebianBased_Install_Dependencies.sh' 
2. For Arch-Linux or Arch based distributions using pacman package manager:
Execute in terminal `bash ArchBased_Install_Dependencies.sh`
    or activate the script and execute './ArchBased_Install_Dependencies.sh'

// Building the Project using cmake 
Execute `bash build.sh` to build the project

// Using Notepad-- 
Execute `./build/Notepad--LinuxQt` to open the Notepad-- 

// Creating an alias shortcut
You can create a shortcut alias, just create a alias script or edit an existing one `~/.bash_aliases` on home folder. 

Add this to .bash_aliases.sh

    # -----------------------------------------------------
    PROJECT_DIR='PUT_THE_PROJECT_PATH_HERE'
    alias notepad__='"$PROJECT_DIR"/build/Notepad--LinuxQt'
    # -----------------------------------------------------

Then just execute in terminal `source .bash_aliases` to load the shortcut. Now you just need to type `notepad__` to open the application. You can add directly the alias on `~/.bashrc`, so it loads automatically on terminal (some distros load the `.bash_aliases` automatically through `.bashrc`, read the `.bashrc` to be sure).

// Creating an application executable script 
You can create an application executable script, just create an empty file `notepad--`

    # -----------------------------------------------------
    #!/bin/sh
    exec PATH_TO/build/Notepad--LinuxQt "$@"
    # -----------------------------------------------------

Change PATH_TO with the actual path to the binary. Move this file to somewhere registered to the PATH variable. You can check the path variable using `echo $PATH`. In general you can move this file to the folder `~/.local/bin/`, check the `cat ~/.bashrc` or `cat ~/.profile` to see if this folder is inserted to the path. If the directory `~/.local/bin/` dont exist, create it and restart the system. Activate the script with `sudo chmod +x ~/.local/bin/notepad--`. Check if the terminal recognize the command `notepad--` or whatever the name you set to the file.
