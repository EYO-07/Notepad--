#!/bin/bash

# Update package lists
sudo pacman -Syu
echo "-----------------------------"
echo "'pacman' Package List Updated"
echo "-----------------------------"

# Install Essential Build Tools | Install Qt6 Core and Widgets
sudo pacman -S base-devel cmake 
sudo pacman -S qt6-base qt6-tools qt6-declarative
# Install QScintilla for Qt6
sudo pacman -S qscintilla-qt6
echo "----------------------------------------------------"
echo "Installed Required Dependencies for Notepad--LinuxQt"
echo "----------------------------------------------------"

