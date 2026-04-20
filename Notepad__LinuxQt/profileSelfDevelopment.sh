# profileSelfDevelopment.sh 
# Profile Template for Notepad--LinuxQt
# ... this template will open files from the project itself.
# ... be careful running arbitrary bash files, profiles should be build and managed by user, don't run profiles from others.

# ./ current folder 
# & means that it will detach from terminal
# use \ to break a command-line line 
# --left argument will open files on the left view
# --right argument will open files on the right view
./build/Notepad--LinuxQt \
    --left \
        ./profileSelfDevelopment.sh \
        ./main.cpp \
        ./CODEX_DarkQt.cpp \
        ./CODEX_DarkQtScintilla.cpp \
        ./CMakeLists.txt \
    --right \
        ./_grimoireNotepad__LinuxQt.h \
        ./CODEX_DarkQt.h \
        ./CODEX_DarkQtScintilla.h \
    &
