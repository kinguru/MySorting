using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Altium
{
    class Program
    {
        static void Main(string[] args)
        {
            IRandomString randomStringGenerator = new RandomString1();

            // create folder, delete previous files
            ArrangeDirectories();

            var timer = new Stopwatch();
            timer.Start();

            if (Config.GenerateFileParallelism > 1)
            {
                var files = new List<string>();
                Parallel.For(0, Config.GenerateFileParallelism, (i) =>
                {
                    var fileName = Config.WorkDirectory + @"\" + Guid.NewGuid().ToString();
                    GenerateFile((long)Config.FileSizeToGenerate / Config.GenerateFileParallelism, fileName, randomStringGenerator);
                    files.Add(fileName);
                });
                GC.Collect();
                Console.WriteLine($"Chunks generated: {timer.Elapsed}");
                ConcatenateFiles(Config.SourceFileName, files);
                Console.WriteLine($"Chunks concatenated: {timer.Elapsed}");
            } else
            {
                GenerateFile(Config.FileSizeToGenerate, Config.SourceFileName, randomStringGenerator);
            }

            // timer stop
            timer.Stop();
            Console.WriteLine($"Elapsed: {timer.Elapsed}");
            Console.ReadKey();
        }

        /// <summary>
        /// Concatenate many files to one
        /// </summary>
        /// <param name="outputFile">Result file path</param>
        /// <param name="inputFiles">Source files paths</param>
        static void ConcatenateFiles(string outputFile, List<string> inputFiles)
        {
            if (inputFiles.Count > 1)
            {
                using (Stream output = File.OpenWrite(outputFile))
                {
                    foreach (string inputFile in inputFiles)
                    {
                        using (Stream input = File.OpenRead(inputFile))
                        {
                            input.CopyTo(output);
                        }

                        File.Delete(inputFile);
                    }
                }
            } else
            {
                File.Move(inputFiles[0], outputFile);
            }

        }

        /// <summary>
        /// Arrange directories and files
        /// </summary>
        static void ArrangeDirectories()
        {
            var dirPath = (new FileInfo(Config.SourceFileName)).Directory.FullName;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            else
            {
                File.Delete(Config.SourceFileName);
            }
        }
     
        /// <summary>
        /// Generates a file of random strings
        /// </summary>
        /// <param name="size">Size in bytes</param>
        /// <param name="fileName">File path</param>
        static void GenerateFile(long size, string fileName, IRandomString randomStringGenerator)
        {
            long generatedFileSize = 0;
            using (var fileStream = new StreamWriter(fileName))
            {
                while (generatedFileSize < size)
                {
                    var rndDataString = randomStringGenerator.GetRandomString();

                    // write to file
                    generatedFileSize += rndDataString.Length;

                    fileStream.Write(rndDataString);
                }
            }
        }
    }

    
}
