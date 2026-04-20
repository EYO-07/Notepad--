# profileSelfDevelopment.sh 
# Profile Template for Notepad--LinuxQt
# ... this template will open all the files from this project

# ./ current folder 
# & means that it will detach from terminal
# use \ to break a command-line line 
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
