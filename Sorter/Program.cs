using Altium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altium
{
    public static class Program
    {
        static long bytesRead = 0;
        static long bytesWrote = 0;

        public static void Main(string[] args)
        {
            // init data comparer here
            // TODO: move to DI injector if need
            IDataStructure stringStructure = new StringStructure();

            var sw = new Stopwatch();
            sw.Start();

            File.WriteAllLines(Config.DestinationFileName,
                File.ReadLines(Config.SourceFileName)
                    .Select(ReadProgress())
                    .Sorted((int)Config.ChunkFileSize, stringStructure.Comparer)
                    .Select(WriteProgress()));

            sw.Stop();
            Console.WriteLine("Elapsed: " + sw.Elapsed);
            Console.ReadKey();
        }

        private static Func<string, int, string> ReadProgress()
        {
            return (x, i) =>
            {
                bytesRead += x.Length;
                if (i % 1000000 == 0) Console.WriteLine("Read: {0:n0} bytes", bytesRead);
                return x;
            };
        }

        private static Func<string, int, string> WriteProgress()
        {
            return (x, i) =>
            {
                bytesWrote += x.Length;
                if (i % 1000000 == 0) Console.WriteLine("Write sorted: {0:n0} bytes", bytesWrote);
                return x;
            };
        }

        /// <summary>
        /// Merge chunk files
        /// </summary>
        /// <param name="lines">Generated file</param>
        /// <param name="size">Chunk size</param>
        /// <param name="comparer">Custom string comparer</param>
        /// <returns>Sorted merged IEnumerable</returns>
        public static IEnumerable<string> Sorted(this IEnumerable<string> lines, int size, IComparer<string> comparer)
        {
            // select non-empty files
            var files = lines.Partition(size, comparer)
                .Select(x => new { File = x, Lines = File.ReadLines(x).GetEnumerator() })
                .Where(x => x.Lines.MoveNext())
                .ToList();

            // go through chunk files
            while (files.Any())
            {
                string current = null;
                var currentFileIndex = 0;
                for (int i = 0; i < files.Count; i++)
                {
                    if (current == null || comparer.Compare(current,files[i].Lines.Current) > 0)
                    {
                        current = files[i].Lines.Current;
                        currentFileIndex = i;
                    }
                }

                if (!files[currentFileIndex].Lines.MoveNext())
                {
                    File.Delete(files[currentFileIndex].File);
                    files.RemoveAt(currentFileIndex);
                }

                yield return current;
            }
        }

        /// <summary>
        /// Split generated file to smaller files
        /// </summary>
        /// <param name="lines">Generated file lines</param>
        /// <param name="size">Chunk target size</param>
        /// <param name="comparer">String comparer</param>
        /// <returns></returns>
        public static IEnumerable<string> Partition(this IEnumerable<string> lines, int size, IComparer<string> comparer)
        {
            return lines.Batch(size)
                .AsParallel()
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .Select(batch =>
                {
                    var temp = Config.WorkDirectory + @"\_" + Guid.NewGuid() + ".tmp";
                    File.WriteAllLines(temp, batch.OrderBy(x => x, comparer));
                    Console.WriteLine("Wrote file {0}", temp);
                    return temp;
                });
        }

        /// <summary>
        /// Create a chunk from
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Generated file</param>
        /// <param name="size">Chunk target size</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> collection, long size)
        {
            var batch = new List<T>();
            var batchSizeBytes = 0;
            foreach (T item in collection)
            {
                batch.Add(item);
                batchSizeBytes += (item as string).Length + 2;
                if (batchSizeBytes >= size)
                {
                    yield return batch;
                    batch = new List<T>();
                    batchSizeBytes = 0;
                }
            }

            if (batch.Any())
                yield return batch;
        }

    }
    
}
