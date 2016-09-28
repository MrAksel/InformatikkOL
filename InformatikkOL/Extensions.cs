using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL
{
    static class Extensions
    {

        public static string Repeat(this string s, int count)
        {
            StringBuilder sb = new StringBuilder(s.Length * count);
            for (int i = 0; i < count; i++)
                sb.Append(s);
            return sb.ToString();
        }
    }
}
