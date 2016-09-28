using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL.Challenges
{
    abstract class Challenge
    {
        private readonly string indentString = "|   ";
        private int indentCount = 0;
        private bool progressMuted = false;


        private StringBuilder totalOutput = new StringBuilder();
        public string TotalOutput { get { return totalOutput.ToString(); } }


        public abstract void Main();


        protected string InputLine()
        {
            return Console.ReadLine();
        }

        protected int ReadIntLine()
        {
            return int.Parse(Console.ReadLine());
        }
        protected int ReadIntSpace()
        {
            StringBuilder sb = new StringBuilder();
            char c;
            while ((c = Console.ReadKey().KeyChar) != ' ')
            {
                sb.Append(c);
            }
            return int.Parse(sb.ToString());
        }


        protected void Output(object arg)
        {
            Output("{0}", arg);
        }
        protected void Output(string fmt, params object[] args)
        {
            string output = string.Format(fmt, args);
            totalOutput.Append(output);
            Console.Write(output);
        }

        protected void OutputLine(object arg)
        {
            OutputLine("{0}", arg);
        }
        protected void OutputLine(string fmt, params object[] args)
        {
            string output = string.Format(fmt, args);
            totalOutput.AppendLine(output);
            Console.WriteLine(output);
        }

        protected void ProgressLine(object arg)
        {
            if (!progressMuted)
                ProgressLine("{0}", arg);
        }
        protected void ProgressLine(string fmt, params object[] args)
        {
            if (!progressMuted)
            {
                string output = indentString.Repeat(indentCount) + string.Format(fmt, args);
                Console.Error.WriteLine(output);
            }
        }

        protected void MuteProgress(bool mute)
        {
            progressMuted = mute;
        }

        protected void IncreaseIndent()
        {
            indentCount++;
        }
        protected void DecreaseIndent()
        {
            indentCount--;
        }
    }
}
