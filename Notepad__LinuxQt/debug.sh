#!/bin/bash

# Define a helper for error messages
fail() {
    echo "----------------------------------"
    echo "ERROR: $1"
    echo "----------------------------------"
    exit 1
}

# 1. Create directory
mkdir -p ./debug || fail "Failed to create build directory."

# 2. Configure project
echo "Configuring project..."
cmake -B debug || fail "CMake configuration failed. Check your CMakeLists.txt or dependencies."

# 3. Build project
echo "Building project..."
cmake --build debug || fail "Compilation failed. Check the error logs above."

# 4. Success
echo "-------------------------------------------"
echo "Notepad--LinuxQt (DEBUG) Successfully built"
echo "-------------------------------------------"
echo "To start: ./debug/Notepad--LinuxQt"