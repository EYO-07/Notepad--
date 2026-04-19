#!/bin/bash

# Define a helper for error messages
fail() {
    echo "----------------------------------"
    echo "ERROR: $1"
    echo "----------------------------------"
    exit 1
}

# 1. Create directory
mkdir -p ./build || fail "Failed to create build directory."

# 2. Configure project
echo "Configuring project..."
cmake -B build || fail "CMake configuration failed. Check your CMakeLists.txt or dependencies."

# 3. Build project
echo "Building project..."
cmake --build build || fail "Compilation failed. Check the error logs above."

# 4. Success
echo "-----------------------------------"
echo "Notepad--LinuxQt Successfully built"
echo "-----------------------------------"
echo "To start: ./build/Notepad--LinuxQt"
echo "Or you can start a profile example: bash profileSelfDevelopment.sh"