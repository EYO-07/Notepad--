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

// Creating a alias shortcut
You can create a shortcut alias, just create a alias script or edit an existing one `~/.bash_aliases` on home folder. 

Add this to .bash_aliases.sh

    # -----------------------------------------------------
    PROJECT_DIR='PUT_THE_PROJECT_PATH_HERE'
    alias notepad__='"$PROJECT_DIR"/build/Notepad--LinuxQt'
    # -----------------------------------------------------

Then just execute in terminal `source .bash_aliases` to load the shortcut. Now you just need to type `notepad__` to open the application. You can add directly the alias on `~/.bashrc`, so it loads automatically on terminal (some distros load the `.bash_aliases` automatically through `.bashrc`, read the `.bashrc` to be sure).
