using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL.Challenges
{
    [Challenge(displayName: "Intervaller")]
    class RunnersIntervals : Challenge
    {

        public override void Main()
        {
            int time0 = ReadIntLine();
            int runnersLeft = ReadIntLine();

            int placement = 1;

            while(runnersLeft-- > 0)
            {
                int time = ReadIntLine();
                if (time < time0)
                    placement++;

                OutputLine(placement);
            }
        }
    }
}
