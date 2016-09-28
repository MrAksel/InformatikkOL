using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL.Challenges
{
    class Palindrom : Challenge
    {
        public override void Main()
        {
            string input = InputLine();

            // Map input chars (A-Z) to bytes
            byte[] data = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                data[i] = (byte)(input[i] - 'A');
            }

            PalindromeMap sm = CreateMap(data);
        }


        static PalindromeMap CreateMap(byte[] data)
        {
            PalindromeMap m = new PalindromeMap();

            int endInc = (data.Length - 1) / 2;
            int startInc = data.Length / 2;

            return m;
        }

        private int FindNumOps(PalindromeMap map, int maxNumOps)
        {

        }
    }
}
