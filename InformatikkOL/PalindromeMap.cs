using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL.Challenges
{
    class PalindromeMap
    {
        List<PalindromeBlock> left;
        List<PalindromeBlock> right;
        List<Tuple<int, int>> mapping;

        internal PalindromeMap()
        {
            left = new List<PalindromeBlock>();
            right = new List<PalindromeBlock>();

            mapping = new List<Tuple<int, int>>();
        }

        internal PalindromeBlock GetBlock(int list, int index)
        {
            if (list == 0)
                return left[index];
            else if (list == 1)
                return right[index];

            throw new ArgumentOutOfRangeException("list", "'list' must be 0 or 1");
        }

        internal PalindromeBlock GetCorrespondingBlock(PalindromeBlock block)
        {
            Tuple<int, int> conn;
            if (block.List == 0)
            {
                conn = mapping.First(t => t.Item1 == block.Index);
                return right[conn.Item2];
            }
            else
            {
                conn = mapping.First(t => t.Item2 == block.Index);
                return left[conn.Item1];
            }
        }
    }
}
