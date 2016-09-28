using System;

namespace InformatikkOL.Challenges
{
    class PalindromeBlock
    {
        Tuple<int, int> index;
        internal byte[] data;


        internal int List { get { return index.Item1; } }
        internal int Index { get { return index.Item2; } }
        internal byte[] Data { get { return data; } }

        internal PalindromeBlock(int list, int index, byte[] data)
        {
            this.index = new Tuple<int, int>(list, index);
            this.data = data;
        }
    }
}
