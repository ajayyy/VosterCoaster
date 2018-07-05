#! /bin/sh

project="Voster Coaster"

echo "Attempting to build $project for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity 
  -batchmode 
  -nographics 
  -silent-crashes 
  -logFile $(pwd)/unity.log 
  -projectPath $(pwd) 
  -buildWindowsPlayer "$(pwd)/Build/windows/$project.exe" 
  -quit
  -quit

echo 'Logs from build'
cat $(pwd)/unity.log

zip -r $(pwd)/Build/windows.zip $(pwd)/Build/windows/