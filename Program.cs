using System.IO;
using static System.Console;

namespace LearnNet.DataProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Parsing command line options");

            var command = args[0];

            if(command == "--file")
            {
                var filePath = args[1];
                WriteLine($"Single file {filePath} selected");
                if(!Path.IsPathFullyQualified(filePath))
                {
                    WriteLine($"ERROR: path '{filePath}' must be fully qualified.");
                    ReadKey();
                    return;
                }

                ProcessSingleFile(filePath);
            }
            else if(command == "--dir")
            {
                var directoryPath = args[1];
                var fileType = args[2];
                WriteLine($"Directory {directoryPath} selected for {fileType} files");
                ProcessDirectory(directoryPath, fileType);
            }
            else
            {
                WriteLine("Invalid command line options");
            }

            WriteLine("Press enter to quit.");
            ReadLine();
        }

        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }

        private static void ProcessDirectory(string directoryPath, string fileType)
        {
            switch(fileType)
            {
                case "TEXT":
                    string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach (var textFilePath in textFiles)
                    {
                        var processor = new FileProcessor(textFilePath);
                        processor.Process();
                    }
                    break;
                default:
                    WriteLine($"ERROR: {fileType} is not supported");
                    return;
            }
        }
    }
}
