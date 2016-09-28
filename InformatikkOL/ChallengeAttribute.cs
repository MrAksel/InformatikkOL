using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL
{
    class ChallengeAttribute : Attribute
    {
        public string DisplayName { get; private set; }

        public ChallengeAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
