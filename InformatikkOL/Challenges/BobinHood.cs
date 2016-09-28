using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL.Challenges
{
    [Challenge(displayName: "Bobin Hood")]
    class BobinHood : Challenge
    {

        public override void Main()
        {
            int price = ReadIntLine();
            int people = ReadIntLine();
            List<int> wealths = new List<int>();
            for (int i = 0; i < people; i++)
                wealths.Add(ReadIntLine());

            // Sort the wealths in ascending order
            wealths.Sort();

            List<Tuple<int, int>> results = new List<Tuple<int, int>>();

            // Start with stealing from the richest and move on to the poorer
            int start = people - 1, end = people - 1;

            while (start >= 0)
            {
                int sum = 0;
                for (int person = start; person <= end; person++)
                {
                    sum += wealths[person];
                }

                while (sum < price)
                {
                    // Not enough cash, steal some more
                    start--;
                    sum += wealths[start];
                }

                if (sum >= price)
                {
                    // Too much cash, try to remove the richest
                    while (sum - wealths[end] >= price)
                    {
                        sum -= wealths[end];
                        end--;
                    }

                    results.Add(new Tuple<int, int>(wealths[start], wealths[end]));
                    ProgressLine("{0} - {1} (dist {2})", wealths[start], wealths[end], wealths[end] - wealths[start]);
                }

                start--;
            }

            results.Sort((t1, t2) => (t1.Item2 - t1.Item1).CompareTo(t2.Item2 - t2.Item1));

            OutputLine("{0} - {1}", results[0].Item1, results[0].Item2);
        }
    }
}
