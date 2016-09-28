using System;

namespace InformatikkOL.Challenges
{
    class Palindrome3Block
    {
        Tuple<int, int> index;
        internal string data;


        internal int List { get { return index.Item1; } }
        internal int Index { get { return index.Item2; } }
        internal string Data { get { return data; } }

        internal Palindrome3Block(int list, int index, string data)
        {
            this.index = new Tuple<int, int>(list, index);
            this.data = data;
        }
    }
}
