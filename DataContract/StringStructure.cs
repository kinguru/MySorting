using System;
using System.Collections.Generic;
using System.Threading;

namespace Altium
{
    public class StringStructure : IDataStructure
    {
        string[] _predefinedStrings;

        public StringStructure() : this(null) { }

        public StringStructure(string[] predefinedStrings)
        {
            Comparer = new CustomComparer();
            _predefinedStrings = predefinedStrings;
        }

        public IComparer<string> Comparer { get; }

        public string GetRandomData()
        {
            var cityRandomizer = StaticRandom.Rand(100000);

            return
                StaticRandom.Rand(999) + ". " +
                (cityRandomizer < _predefinedStrings.Length
                ? _predefinedStrings[cityRandomizer]
                : Guid.NewGuid().ToString())
                + "\r\n";
            ;
        }
    }

    public interface IDataStructure
    {
        string GetRandomData();

        IComparer<string> Comparer { get; }
    }

    public static class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Rand(int maxValue)
        {
            return random.Value.Next(maxValue);
        }
    }

    public class CustomComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            var s1i = s1.IndexOf('.');
            var s1str = s1.Substring(s1i + 2);

            var s2i = s2.IndexOf('.');
            var s2str = s2.Substring(s2i + 2);

            var strCompare = String.Compare(s1str, s2str);
            if (strCompare != 0)
                return strCompare;

            var s1num = int.Parse(s1.Substring(0, s1i));
            var s2num = int.Parse(s2.Substring(0, s2i));

            return s1num.CompareTo(s2num);
        }
    }
}
