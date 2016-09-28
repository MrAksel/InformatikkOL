using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL.Challenges
{
    class Palindrome3Map
    {
        List<Palindrome3Block> left;
        List<Palindrome3Block> right;
        List<Tuple<int, int>> mapping;


        internal Palindrome3Map()
        {
            left = new List<Palindrome3Block>();
            right = new List<Palindrome3Block>();

            mapping = new List<Tuple<int, int>>();
        }

        
        internal void AddValue(int list, string value)
        {
            InsertValue( list, -1, value);
        }
        internal void InsertValue(int list, int index, string value)
        {
            if (list == 0)
            {
                if (index < 0)
                    index = left.Count + index;

                Palindrome3Block block = new Palindrome3Block(list, index, value);
                left.Insert(index, block);
            }
            else if (list == 1)
            {
                if (index < 0)
                    index = right.Count + index;

                Palindrome3Block block = new Palindrome3Block(list, index, value);
                right.Insert(index, block);
            }
        }


        internal Palindrome3Block GetBlock(int list, int index)
        {
            if (list == 0)
                return left[index];
            else if (list == 1)
                return right[index];

            throw new ArgumentOutOfRangeException("list", "'list' must be 0 or 1");
        }
        internal Palindrome3Block GetCorrespondingBlock(Palindrome3Block block)
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


        // aaabbbcccdddeeefff
        // grklaaaabbbcccdddeeefff

        internal void Simplify(out int ops)
        {
            ops = 0;
        }

        internal void Morph(out int changes, out int inserts, out int removes)
        {
            changes = inserts = removes = 0;
        }
    }
}
