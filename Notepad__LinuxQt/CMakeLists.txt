# -- BEGIN

cmake_minimum_required(VERSION 3.16)
project(Notepad--LinuxQt VERSION 1.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Enable Qt features
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTOUIC ON) # Added in case you use .ui files
set(CMAKE_AUTORCC ON) # Added in case you use .qrc files

# Find Qt
find_package(Qt6 REQUIRED COMPONENTS Widgets)

# Find QScintilla
# --- Robust QScintilla Find Logic ---
# 1. Search for the directory containing the 'Qsci' folder
find_path(QSCINTILLA_INCLUDE_DIR 
    NAMES Qsci/qsciscintilla.h
    PATHS 
        /usr/include/qt6 
        /usr/local/include/qt6
        /usr/include/${CMAKE_LIBRARY_ARCHITECTURE}/qt6 # For Debian/Ubuntu multiarch
    DOC "Path to QScintilla include directory"
)
# 2. Search for the library (checking common Qt6 naming conventions)
find_library(QSCINTILLA_LIB 
    NAMES qscintilla2_qt6 qscintilla2-qt6 libqscintilla2_qt6
    PATHS 
        /usr/lib 
        /usr/lib/${CMAKE_LIBRARY_ARCHITECTURE}
        /usr/local/lib
    DOC "Path to QScintilla library"
)
# --- Validation ---
if(NOT QSCINTILLA_LIB OR NOT QSCINTILLA_INCLUDE_DIR)
    message(FATAL_ERROR "QScintilla NOT found!
        Include Dir: ${QSCINTILLA_INCLUDE_DIR}
        Library:     ${QSCINTILLA_LIB}
        If you are on Ubuntu/Debian, run: sudo apt install libqscintilla2-qt6-dev
        If you are on Arch, run: sudo pacman -S qscintilla-qt6
    ")
endif()

# --
add_executable(Notepad--LinuxQt main.cpp CODEX_DarkQt.cpp CODEX_DarkQtScintilla.cpp)
target_compile_definitions(Notepad--LinuxQt PRIVATE 
    QT_VERSION_MAX_ALLOWED=0x060000
    QT_DISABLE_DEPRECATED_BEFORE=0x060000
)        
target_include_directories(Notepad--LinuxQt PRIVATE ${QSCINTILLA_INCLUDE_DIR})
target_link_libraries(Notepad--LinuxQt PRIVATE Qt6::Widgets ${QSCINTILLA_LIB})



# -- END 


