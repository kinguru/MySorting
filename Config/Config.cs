using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altium
{
    public static class Config
    {
        // specify Working Directory and optionally source and destination file names
        // TODO: move configuratiln fields to app.config or application command line arguments
        public const string WorkDirectory = @"D:\Altium.Sort";
        private const string _sourceFileName = @"datafileGenerated.txt";
        private const string _destinationFileName = @"dataFileSorted.txt";

        public static string SourceFileName = WorkDirectory + @"\" + _sourceFileName;
        public static string DestinationFileName = WorkDirectory + @"\" + _destinationFileName;

        // pseudo-random values
        public static readonly string[] predefinedStrings = new[] { "Kiev", "Kharkiv", "Lviv", "New York", "Sidney" };

        // file size to be generated, bytes
        public const long FileSizeToGenerate = 1000L * 1000000;

        // chunk file size to be able to fit in memory, bytes
        public static long ChunkFileSize = 100L * 1000000;

        // number of files to be written to disk simultaneously
        // if you have multicore CPU, then increase it from 1 until your CPU load OR Disk load is 100%
        // Generally set it to logical CPU count
        public static byte GenerateFileParallelism = 4;
    }
}
