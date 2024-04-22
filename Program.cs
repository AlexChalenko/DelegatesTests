using DelegatesTraverser;
using DelegatesTraverser.Extensions;

namespace DelegatesTests
{
    internal class Program
    {
        static void Main()
        {
            DirectoryTraverser traverser = new();
            List<FileInfo> allFiles = [];

            traverser.FileFound += (sender, e) =>
            {
                FileInfo fileInfo = new FileInfo(e.FileName);
                allFiles.Add(fileInfo);
                Console.WriteLine("Found file: " + e.FileName);

                if (fileInfo.Name.Contains("User.cs"))  // условие для демонстрации отмены
                {
                    Console.WriteLine("Stopping traversal.");
                    ((DirectoryTraverser)sender).RequestCancel();
                }
            };

            string pathToTraverse = @"C:\Users\Alexey\source\repos\AllRoadsLeadToRome\src\services\auth\Core\Entities";
            traverser.Traverse(pathToTraverse);

            if (allFiles.Count > 0)
            {
                FileInfo largestFile = allFiles.GetMax(file => file.Length);
                Console.WriteLine($"The largest file is: {largestFile.Name} with size {largestFile.Length} bytes");
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }
    }
}
