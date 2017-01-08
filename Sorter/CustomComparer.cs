using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altium
{
    /// <summary>
    /// Strings sorting logic   
    /// </summary>
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
