using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace OperationOrganise
{
    public class Organiser
    {
        private readonly string _pathToOrganise;
        private readonly int _maximumPerOrganise;
        public enum Mode
        {
            Images,
            Videos
        }; 

        public Organiser(string pathToOrganise, int maximumPerOrganise)
        {
            _pathToOrganise = pathToOrganise;
            _maximumPerOrganise = maximumPerOrganise;
        }

        public void Process(Mode mode, IEnumerable<string> validExtensions, Dictionary<string, string> transforms = null)
        {
            mode.ToString().ConsoleLogMessage();

            MoveValidFilesToRoot(mode, validExtensions);

            if (transforms != null)
                TransformFileNames(transforms);

            OrganiseFilesToFolders(mode, validExtensions);
        }

        private void MoveValidFilesToRoot(Mode mode, IEnumerable<string> validExtensions)
        {
            if (!Directory.Exists(_pathToOrganise))
                return;

            "MOVE TO SOURCEPATH".ConsoleLogMessage();

            var files = new DirectoryInfo(_pathToOrganise)
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Where(f => validExtensions.Contains(f.Extension.ToLower()))
                .Select(f => f.FullName)
                .ToList();

            if (!files.Any())
                return;

            files.RemoveAll(f => f == $"{_pathToOrganise}\\{Path.GetFileName(f)}");

            $"{files.Count()} FILES FOUND".ConsoleLogMessage();

            foreach (var f in files)
            {
                var fileName = Path.GetFileName(f);

                // if file exists
                if (File.Exists($"{_pathToOrganise}\\{fileName}"))
                {
                    // check if existing file matches 
                    switch (mode)
                    {
                        case Mode.Images:
                            fileName = Path.GetFileName(CompareImages(f, $"{_pathToOrganise}\\{fileName}"));
                            break;
                        case Mode.Videos:
                            fileName = Path.GetFileName(CompareVideos($"{_pathToOrganise}\\{fileName}"));
                            break;
                    }

                    // if filename the same, the file can be deleted. 
                    if (File.Exists($"{_pathToOrganise}\\{fileName}"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(f);
                        File.Delete(f);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                    }
                }

                try
                {
                    File.Move(f, $"{_pathToOrganise}\\{fileName}");
                    Console.WriteLine($"{_pathToOrganise}\\{fileName}");
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{_pathToOrganise}\\{fileName} locked");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

        private void TransformFileNames(IDictionary<string, string> transforms)
        {
            "TRANSFORMERS".ConsoleLogMessage();

            var files = new DirectoryInfo(_pathToOrganise)
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Where(f => transforms.ContainsKey(f.Extension.ToLower()))
                .OrderBy(f => f.LastWriteTime)
                .Select(f => f.FullName)
                .ToList();

            if (!files.Any())
                return;

            $"{files.Count()} FILES FOUND".ConsoleLogMessage();

            foreach (var f in files)
            {
                var key = Path.GetExtension(f);

                if (key != null && !transforms.ContainsKey(key))
                    continue;

                var transform = transforms.First(x => x.Key == key);
                var newName = f.Replace(transform.Key, transform.Value).GetNewFileName();
                
                try
                {
                    File.Move(f, newName);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{newName} locked");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }
  
        private void OrganiseFilesToFolders(Mode mode, IEnumerable<string> validExtensions)
        {
            "ORGANISE".ConsoleLogMessage();

            var files = new DirectoryInfo(_pathToOrganise)
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Where(f => validExtensions.Contains(f.Extension.ToLower()))
                .OrderBy(f => f.LastWriteTime)
                .Select(f => f.FullName)
                .ToList();

            if (!files.Any())
                return;

            $"{files.Count()} FILES FOUND".ConsoleLogMessage();

            var counter = 0;
            string currentPath;

            files.ForEach(f =>
            {
                var iteration = counter/_maximumPerOrganise;
                currentPath = $"{_pathToOrganise}\\{mode.ToString()}\\{iteration + 1}";

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                    $"CREATE FOLDER: {currentPath}".ConsoleLogMessage();
                }

                if (counter%_maximumPerOrganise != -1)
                {
                    var newFile = ($"{currentPath}\\{Path.GetFileName(f)}").GetNewFileName();

                    try
                    {
                        File.Move(f, newFile);
                        Console.WriteLine($"{counter} - {newFile}");
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{newFile} locked");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                counter++;
            });

        }

        private string CompareImages(string sourceFile, string destinationFile)
        {
            var i = 0;
            var source = new Bitmap(sourceFile);
            var destinaton = new Bitmap(destinationFile);

            if (ComparingImages.Compare(source, destinaton) != ComparingImages.CompareResult.CompareOk)
                while (File.Exists(destinationFile))
                {
                    i++;
                    destinationFile =
                        $"{Path.GetFileNameWithoutExtension(destinationFile)}_{i}{Path.GetExtension(destinationFile)}";
                }
            source.Dispose();
            destinaton.Dispose();

            return destinationFile;
        }

        private string CompareVideos(string destinationFile)
        {
            var i = 0;
            while (File.Exists(destinationFile))
            {
                i++;
                destinationFile =
                    $"{Path.GetFileNameWithoutExtension(destinationFile)}_{i}{Path.GetExtension(destinationFile)}";
            }
            return destinationFile;
        }
    }
}