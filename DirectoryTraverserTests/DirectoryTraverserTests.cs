using DelegatesTraverser.Extensions;
using Xunit;

namespace DelegatesTraverser.Tests
{
    public class DirectoryTraverserTests : IDisposable
    {
        private string _testFolder = Path.Combine(Directory.GetCurrentDirectory(), "TestFolder");
        private string _largestFileName = "test_largest.txt";

        public DirectoryTraverserTests()
        {
            //создаем тестовую папку
            for (int i = 0; i < 9; i++)
            {
                Directory.CreateDirectory(_testFolder);
                File.WriteAllText(Path.Combine(_testFolder, $"test{i}.txt"), new string('a', new Random().Next(1, 1000)));
            }
            //создаем самый большой файл
            File.WriteAllText(Path.Combine(_testFolder, _largestFileName), new string('a', new Random().Next(1, 100000)));
        }

        [Fact]
        public void Traverse_WhenCalled_ReturnLargestFile()
        {
            // Arrange
            DirectoryTraverser traverser = new DirectoryTraverser();

            // Act
            traverser.Traverse(_testFolder);

            var largestFileLenght = traverser.Files.GetMax(file => file.Length);
            FileInfo largestFile = new(Path.Combine(_testFolder, _largestFileName));

            // Assert
            Assert.NotNull(largestFileLenght);
            Assert.Equal(largestFileLenght.Length, largestFile.Length);
        }


        [Fact]
        public void Traverse_WhenCalled_RaiseFileFoundEvent()
        {
            // Arrange
            DirectoryTraverser traverser = new DirectoryTraverser();
            List<string> filesFound = new List<string>();
            traverser.FileFound += (sender, args) => filesFound.Add(args.FileName);

            // Act
            traverser.Traverse(_testFolder);

            // Assert
            Assert.Equal(10, filesFound.Count);
        }

        [Fact]
        public void Traverse_ShouldTriggerFileFoundEvent_WithCorrectArguments()
        {
            // Arrange
            var traverser = new DirectoryTraverser();
            var wasCalled = false;
            FileArgs receivedArgs = null;

            traverser.FileFound += (sender, e) =>
            {
                wasCalled = true;
                receivedArgs = e;
            };

            // Act
            traverser.Traverse(_testFolder);

            // Assert
            Assert.True(wasCalled, "The FileFound event was not triggered.");
            Assert.NotNull(receivedArgs);
            Assert.True(File.Exists(Path.Combine(_testFolder, receivedArgs.FileName)), "The file reported in the event does not exist.");
        }

        [Fact]
        public void Traverse_WhenCancelRequested_StopTraversing()
        {
            // Arrange
            var counter = 0;
            DirectoryTraverser traverser = new DirectoryTraverser();
            traverser.FileFound += (sender, args) =>
            {
                if (counter >= 5)
                {
                    traverser.RequestCancel();
                }
                else
                {
                    counter++;
                }
            };

            // Act
            traverser.Traverse(_testFolder);

            // Assert
            Assert.NotEmpty(traverser.Files);
            Assert.NotEqual(10, counter);
        }

        [Fact]
        public void Traverse_ShouldPrintCorrectMessages()
        {
            // Arrange
            var traverser = new DirectoryTraverser();
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            traverser.FileFound += (sender, e) => Console.WriteLine($"Found file: {e.FileName}");
            traverser.Traverse(_testFolder);

            var largestFile = traverser.Files.GetMax(file => file.Length);
            if (traverser.Files.GetMax(file => file.Length) is { } largestFileLenght)
            {
                Console.WriteLine($"The largest file is: {largestFile.Name}");
            }

            string consoleOutput = output.ToString();
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            output.Dispose();

            // Assert
            Assert.Contains("Found file:", consoleOutput); // Проверка на вывод всех найденных файлов
            Assert.Contains("The largest file is:", consoleOutput); // Проерка на вывод самого большого файла
        }

        private void Traverser_FileFound(object? sender, FileArgs e)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //удаляем тестовую папку
            Directory.Delete(_testFolder, true);
        }
    }
}