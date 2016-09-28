using InformatikkOL.Challenges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL
{
    class Program
    {
        static void Main(string[] args)
        {
            string task = "";

            Type challengeClass = null;
            Type[] allChallengeTypes = typeof(Challenge).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Challenge))).ToArray();
            string[] displayNames = allChallengeTypes.Select(t => t.GetCustomAttribute<ChallengeAttribute>().DisplayName).ToArray();

            // List all challenges
            if (challengeClass == null)
            {
                for (int i = 0; i < allChallengeTypes.Length; i++)
                {
                    Console.WriteLine("{0}: {1}", i, displayNames[i]);
                }
            }

            // Select one challenge
            int taskNum = -1;
            while (challengeClass == null)
            {
                task = Console.ReadLine();
                if (int.TryParse(task, out taskNum) && taskNum >= 0 && taskNum < allChallengeTypes.Length)
                {
                    challengeClass = allChallengeTypes[taskNum];
                }
                else
                {
                    Console.WriteLine("Invalid index");
                }
            }

            while (true)
            {
                // Execute
                Console.WriteLine("Executing {0}", displayNames[taskNum]);
                Challenge executer = Activator.CreateInstance(challengeClass) as Challenge;
                executer.Main();
                Console.WriteLine("Finished");
                Console.WriteLine();
            }
        }
    }
}
