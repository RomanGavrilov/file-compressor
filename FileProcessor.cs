using System;
using System.IO;
using static System.Console;

namespace LearnNet.DataProcessor
{
    public class FileProcessor
    {

        private const string BackupDirectoryName = "backup";
        private const string InProgressDirectoryName = "processing";
        private const string CompletedDirectoryName = "complete";

        public string InputFilePath { get; }

        public FileProcessor(string filePath) => InputFilePath = filePath;
    
        public void Process()
        {
            WriteLine($"Begin process of {InputFilePath}");

            if (!File.Exists(InputFilePath))
            {
                WriteLine($"Error: file {InputFilePath} does not exist.");
                return;
            }

            string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
            WriteLine($"Root data path is {rootDirectoryPath}");

            // checking backup dir
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);
            WriteLine($"Checking backup directory {backupDirectoryPath}");
            Directory.CreateDirectory(backupDirectoryPath);

            // copy file to backup dir
            string inputFileName = Path.GetFileName(InputFilePath);
            string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
            WriteLine($"Copying {InputFilePath} to {backupFilePath}");
            File.Copy(InputFilePath, backupFilePath, overwrite: true);

            // move file to in progress dir
            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
            string inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

            if(File.Exists(inProgressFilePath))
            {
                WriteLine($"ERROR: a file with the name {inProgressFilePath} is already being processed");
                return;
            }

            WriteLine($"Moving {InputFilePath} to {InProgressDirectoryName}");
            File.Move(InputFilePath, inProgressFilePath);

            // determine type of file
            string extension = Path.GetExtension(InputFilePath);

            switch(extension)
            {
                case ".txt":
                    ProcessTextFile(inProgressFilePath);
                    break;
                default:
                    WriteLine($"ERROR: {extension} is an unsupported file type");
                    break;
            }

            string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompletedDirectoryName);
            Directory.CreateDirectory(completedDirectoryPath);
            WriteLine($"Moving {inProgressFilePath} to {completedDirectoryPath}");
            string completedFileName = $"{Path.GetFileNameWithoutExtension(InputFilePath)}-{Guid.NewGuid()}{extension}";

            completedFileName = Path.ChangeExtension(completedFileName, ".complete");

            var completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);
            File.Move(inProgressFilePath, completedFilePath);

            string inProgressDirectoryPath = Path.GetDirectoryName(inProgressFilePath);
            Directory.Delete(inProgressDirectoryPath, recursive: true);
        }

        private void ProcessTextFile(string inProgressFilePath)
        {
            WriteLine($"Processing text file {inProgressFilePath}");
            // read in and process
        }
    }
}
