using System;
using System.IO;
using System.Linq;
using System.Threading;

class SyncTestFolders
{
    private static string sourceFolder;
    private static string replicaFolder;
    private static int syncInterval;
    private static string logFilePath;

    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSync.exe <sourceFolder> <replicaFolder> <syncIntervalInSeconds> <logFilePath>");
            return;
        }


        if (args.Length < 2)
        {
            Console.WriteLine("Usage: SyncFolders.exe <source> <destination>");
            return;
        }

        sourceFolder = args[0];
        replicaFolder = args[1];

        // Ensure source directory exists (create if missing)
        if (!Directory.Exists(sourceFolder))
        {
            Console.WriteLine($"Source directory does not exist. Creating: {sourceFolder}");
            Directory.CreateDirectory(sourceFolder);
            CreateSourceFiles(sourceFolder);
        }

        // Ensure destination directory exists (create if missing)
        if (!Directory.Exists(replicaFolder))
        {
            Console.WriteLine($"Destination directory does not exist. Creating: {replicaFolder}");
            Directory.CreateDirectory(replicaFolder);
        }

        Console.WriteLine("Both directories are now valid. Proceeding with sync...");




    
        if (!int.TryParse(args[2], out syncInterval) || syncInterval <= 0)
        {
            Console.WriteLine("Error: Invalid synchronization interval. Please provide a positive integer.");
            return;
        }
        logFilePath = args[3];

        Log("Starting folder synchronization...");

        while (true)
        {
            try
            {
                SyncFolders();
            }
            catch (Exception ex)
            {
                Log("Critical error: " + ex.Message);
            }
            Thread.Sleep(syncInterval * 1000);
        }
    }


    static void CreateSourceFiles(string directoryPath)
    {
        Console.WriteLine("Creating source files...");

        File.WriteAllText(Path.Combine(directoryPath, "Source1.txt"), "Hello");
        File.WriteAllText(Path.Combine(directoryPath, "Source2.txt"), "Veeam");
        File.WriteAllText(Path.Combine(directoryPath, "Source3.txt"), "How are you doing!?");

        Console.WriteLine("Source files created successfully.");
    }

    static void SyncFolders()
    {
        try
        {
            if (!Directory.Exists(sourceFolder) || !Directory.Exists(replicaFolder))
            {
                Log("Error: One or both directories do not exist.");
                return;
            }

            Log("Synchronization started...");
            SyncFiles(sourceFolder, replicaFolder);
            RemoveExtraFiles(replicaFolder, sourceFolder);
            Log("Synchronization completed successfully.");
        }
        catch (Exception ex)
        {
            Log("Error during synchronization: " + ex.Message);
        }
    }

    static void SyncFiles(string sourceDir, string targetDir)
    {
        foreach (string sourceFile in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(sourceFile);
            string replicaFile = Path.Combine(targetDir, fileName);

            try
            {
                if (!File.Exists(replicaFile) || !File.ReadAllBytes(sourceFile).SequenceEqual(File.ReadAllBytes(replicaFile)))
                {
                    File.Copy(sourceFile, replicaFile, true);
                    Log($"Copied: {sourceFile} -> {replicaFile}");
                }
            }
            catch (Exception ex)
            {
                Log($"Error copying {sourceFile}: {ex.Message}");
            }
        }

        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(subDir);
            string replicaSubDir = Path.Combine(targetDir, dirName);

            try
            {
                if (!Directory.Exists(replicaSubDir))
                {
                    Directory.CreateDirectory(replicaSubDir);
                    Log($"Created directory: {replicaSubDir}");
                }

                SyncFiles(subDir, replicaSubDir);
            }
            catch (Exception ex)
            {
                Log($"Error processing directory {subDir}: {ex.Message}");
            }
        }
    }

    static void RemoveExtraFiles(string replicaDir, string sourceDir)
    {
        foreach (string replicaFile in Directory.GetFiles(replicaDir))
        {
            string fileName = Path.GetFileName(replicaFile);
            string sourceFile = Path.Combine(sourceDir, fileName);

            try
            {
                if (!File.Exists(sourceFile))
                {
                    File.Delete(replicaFile);
                    Log($"Deleted: {replicaFile}");
                }
            }
            catch (Exception ex)
            {
                Log($"Error deleting {replicaFile}: {ex.Message}");
            }
        }

        foreach (string subDir in Directory.GetDirectories(replicaDir))
        {
            string dirName = Path.GetFileName(subDir);
            string sourceSubDir = Path.Combine(sourceDir, dirName);

            try
            {
                if (!Directory.Exists(sourceSubDir))
                {
                    Directory.Delete(subDir, true);
                    Log($"Deleted directory: {subDir}");
                }
                else
                {
                    RemoveExtraFiles(subDir, sourceSubDir);
                }
            }
            catch (Exception ex)
            {
                Log($"Error deleting directory {subDir}: {ex.Message}");
            }
        }
    }

    static void Log(string message)
    {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        Console.WriteLine(logEntry);
        try
        {
            string logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logging error: {ex.Message}");
        }
    }
}