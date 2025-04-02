@echo off
SET SYNC_EXE="C:\Luxoft\Learnings\SyncFolders\bin\Debug\net7.0\SyncFolders.exe"
SET SOURCE_DIR="C:\cVeeam\Source"
SET REPLICA_DIR="C:\cVeeam\Replica"
SET LOG_FILE="C:\Veeam\Logs\sync.log"

:: Check if SyncFolders.exe exists
if not exist %SYNC_EXE% (
    echo [%DATE% %TIME%] ERROR: SyncFolders.exe not found! >> %LOG_FILE%
    echo ERROR: SyncFolders.exe not found! Check your path.
    pause
    exit /b
)

:: Ensure source and replica directories exist
if not exist %SOURCE_DIR% mkdir %SOURCE_DIR%
if not exist %REPLICA_DIR% mkdir %REPLICA_DIR%

:: Run SyncFolders.exe using the start command
echo [%DATE% %TIME%] Starting SyncFolders.exe... >> %LOG_FILE%
start "" "C:\Luxoft\Learnings\SyncFolders\bin\Debug\net7.0\SyncFolders.exe" "C:\cVeeam\Source" "C:\cVeeam\Replica" 30 >> %LOG_FILE% 2>&1
echo [%DATE% %TIME%] Sync completed. >> %LOG_FILE%

pause
