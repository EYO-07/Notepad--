#!/bin/bash

# Update package lists
sudo apt update
echo "--------------------------"
echo "'apt' Package List Updated"
echo "--------------------------"

# Install Essential Build Tools
sudo apt install -y build-essential cmake 
# Install Qt6 Core and Widgets
sudo apt install -y qt6-base-dev qt6-declarative-dev qt6-tools-dev-tools 
# Install QScintilla for Qt6
sudo apt install -y libqscintilla2-qt6-dev
echo "----------------------------------------------------"
echo "Installed Required Dependencies for Notepad--LinuxQt"
echo "----------------------------------------------------"

