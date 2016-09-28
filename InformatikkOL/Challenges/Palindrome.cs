using System;
using System.IO;
using System.Linq;
using System.Text;
using Block = System.Tuple<int, int, int>;
using BlockList = System.Collections.Generic.List<System.Tuple<int, int, int>>;

namespace InformatikkOL.Challenges
{
    [Challenge(displayName: "Palindrom (blokkoppdatering)")]
    class Palindrome : Challenge
    {

        public override void Main()
        {
            int totChanges = 0;


            string input = InputLine();
            if (File.Exists(input))
                input = File.ReadAllText(input);

            input = input.ToLower().Replace(" ", "").Replace(",", "");

            string s1 = input.Substring(0, (input.Length + 1) / 2);
            string s2 = input.Remove(0, input.Length / 2).Reverse().Select(c => c.ToString()).Aggregate((sl, sr) => sl + sr);

            string f1, f2;

            TrimEqual(s1, s2, out f1, out f2);

            BlockList s1blocks;
            BlockList s2blocks;

            BlockifyStrings(f1, f2, out s1blocks, out s2blocks);

            PrintBlocks(f1, s1blocks);
            PrintBlocks(f2, s2blocks);

            totChanges = MinRequiredChanges(f1, f2, s1blocks, s2blocks);

            /*
            PalindromeMap sm = CreateMap(input);
            int compressions = 0;
            int changes = 0;
            int inserts = 0;
            int removes = 0;

            while (compressions != 0)
            {
                sm.Simplify(out compressions);
            }

            while ((changes + inserts + removes) != 0)
            {
                sm.Morph(out changes, out inserts, out removes);
                totChanges += changes + inserts + removes;
            }
            */

            OutputLine(totChanges);
        }


        private int MinRequiredChanges(string s1, string s2, BlockList s1blocks, BlockList s2blocks)
        {
            int min = s1.Length;

            int chang = short.MaxValue, insrt = short.MaxValue, remov = short.MaxValue;

            // Simulate insertion of an orphan block into the other
            if (min > 1)
                insrt = SimulateInsertion(s1, s2, s1blocks, s2blocks);
            min = Math.Min(min, insrt);

            // Simulate change of an orphan block
            if (min > 1)
                chang = SimulateChange(s1, s2, s1blocks, s2blocks);
            min = Math.Min(min, chang);

            // Simulate removal of an orphan block
            if (min > 1)
                remov = SimulateRemoval(s1, s2, s1blocks, s2blocks);
            min = Math.Min(min, remov);

            return min;
        }

        private int SimulateRemoval(string s1, string s2, BlockList s1blocks, BlockList s2blocks)
        {
            int min = s1.Length;

            BlockList s1orphans = s1blocks.Where(b => b.Item3 == -1).ToList();
            BlockList s2orphans = s2blocks.Where(b => b.Item3 == -1).ToList();

            if (s1orphans.Count == 0 && s2orphans.Count == 0)
            {
                return 0;
            }

            // Try removal of an s1 orphan
            foreach (Block s1orphan in s1orphans)
            {
                int changes = 0;
                BlockList newList = CloneList(s1blocks);

                // Remove the block we're at and adjust the indexes of the blocks after
                int indexOf = newList.IndexOf(s1orphan);
                newList.RemoveAt(indexOf);
                string mods1 = s1.Remove(s1orphan.Item1, s1orphan.Item2);

                for (int i = indexOf; i < newList.Count; i++)
                    newList[i] = new Tuple<int, int, int>(newList[i].Item1 - s1orphan.Item2, newList[i].Item2, newList[i].Item3);

                // Count changes and recurse
                changes += s1orphan.Item2;
                changes += MinRequiredChanges(mods1, s2, newList, s2blocks);

                min = Math.Min(min, changes);
            }

            // Try removal of an s2 orphan
            foreach (Block s2orphan in s2orphans)
            {
                int changes = 0;
                BlockList newList = CloneList(s2blocks);

                // Remove the block we're at and adjust the indexes of the blocks after
                int indexOf = newList.IndexOf(s2orphan);
                newList.RemoveAt(indexOf);
                string mods2 = s2.Remove(s2orphan.Item1, s2orphan.Item2);

                for (int i = indexOf; i < newList.Count; i++)
                    newList[i] = new Tuple<int, int, int>(newList[i].Item1 - s2orphan.Item2, newList[i].Item2, newList[i].Item3);

                // Count changes and recurse
                changes += s2orphan.Item2;
                changes += MinRequiredChanges(s1, mods2, s1blocks, newList);

                min = Math.Min(min, changes);
            }

            return min;
        }

        private int SimulateChange(string s1, string s2, BlockList s1blocks, BlockList s2blocks)
        {
            int min = s1.Length;

            BlockList s1orphans = s1blocks.Where(b => b.Item3 == -1).ToList();
            BlockList s2orphans = s2blocks.Where(b => b.Item3 == -1).ToList();

            if (s1orphans.Count == 0 && s2orphans.Count == 0)
            {
                return 0;
            }

            // Try change of an s1 orphan
            foreach (Block s1orphan in s1orphans)
            {
                int changes = 0;

                // Replace the content of the block we're at with the content of the corresponding block
                int indexOf = s1blocks.IndexOf(s1orphan);
                if (indexOf < s2blocks.Count)
                {
                    BlockList news1List = CloneList(s1blocks);
                    BlockList news2List = CloneList(s2blocks);

                    Block s2corr = s2blocks[indexOf];

                    string mods1 = s1.Remove(s1orphan.Item1, s1orphan.Item2);
                    string insVal = s2.Substring(s2corr.Item1, s2corr.Item2);
                    mods1 = mods1.Insert(s1orphan.Item1, insVal);
                    int charDiff = insVal.Length - s1orphan.Item2;

                    // Update corresponding block indexes
                    news1List[indexOf] = new Block(s1orphan.Item1, s2corr.Item2, indexOf);
                    news2List[indexOf] = new Block(s1orphan.Item1, s2corr.Item2, indexOf);
                    // Update character indexes in s1blocks after the changed block
                    for (int s1b = indexOf + 1; s1b < news1List.Count; s1b++)
                    {
                        news1List[s1b] = new Block(news1List[s1b].Item1 + charDiff, news1List[s1b].Item2, news1List[s1b].Item3);
                    }

                    // Count changes and recurse
                    changes += (charDiff > 0 ? insVal.Length : s1orphan.Item2); 

                    // Count changes and recurse
                    changes += insVal.Length;
                    changes += MinRequiredChanges(mods1, s2, news1List, news2List);

                    min = Math.Min(min, changes);
                }
            }

            // Try change of an s2 orphan
            foreach (Block s2orphan in s2orphans)
            {
                int changes = 0;

                // Replace the content of the block we're at with the content of the corresponding block
                int indexOf = s2blocks.IndexOf(s2orphan);
                if (indexOf < s1blocks.Count)
                {
                    BlockList news1List = CloneList(s1blocks);
                    BlockList news2List = CloneList(s2blocks);

                    Block s1corr = s1blocks[indexOf];

                    string mods2 = s2.Remove(s2orphan.Item1, s2orphan.Item2);
                    string insVal = s1.Substring(s1corr.Item1, s1corr.Item2);
                    mods2 = mods2.Insert(s2orphan.Item1, insVal);
                    int charDiff = insVal.Length - s2orphan.Item2;

                    // Update corresponding block indexes
                    news1List[indexOf] = new Block(s2orphan.Item1, s1corr.Item2, indexOf);
                    news2List[indexOf] = new Block(s2orphan.Item1, s1corr.Item2, indexOf);
                    // Update character indexes in s2blocks after the changed block
                    for (int s2b = indexOf + 1; s2b < news2List.Count; s2b++)
                    {
                        news2List[s2b] = new Block(news2List[s2b].Item1 + charDiff, news2List[s2b].Item2, news2List[s2b].Item3);
                    }

                    // Count changes and recurse
                    changes += (charDiff > 0 ? insVal.Length : s2orphan.Item2); 
                    changes += MinRequiredChanges(s1, mods2, news1List, news2List);

                    min = Math.Min(min, changes);
                }
            }

            return min;
        }

        private int SimulateInsertion(string s1, string s2, BlockList s1blocks, BlockList s2blocks)
        {
            int min = s1.Length;

            BlockList s1orphans = s1blocks.Where(b => b.Item3 == -1).ToList();
            BlockList s2orphans = s2blocks.Where(b => b.Item3 == -1).ToList();

            if (s1orphans.Count == 0 && s2orphans.Count == 0)
            {
                return 0;
            }

            // Try change of an s1 orphan
            foreach (Block s1orphan in s1orphans)
            {
                int changes = 0;

                // Insert the content of the block we're at in the other string
                int indexOf = s1blocks.IndexOf(s1orphan);
                if (indexOf < s2blocks.Count)
                {
                    BlockList news1List = CloneList(s1blocks);
                    BlockList news2List = CloneList(s2blocks);

                    // Calculate the index in the other string to insert the value
                    int s2indx = 0;
                    for (int s2b = 0; s2b < indexOf; s2b++)
                    {
                        s2indx += s2blocks[s2b].Item2;
                    }

                    string insVal = s1.Substring(s1orphan.Item1, s1orphan.Item2);
                    string mods2 = s2.Insert(s2indx, insVal);

                    // Create the corresponding block in s2 and insert it to the list
                    Block s2corr = new Block(s2indx, s1orphan.Item2, indexOf);
                    news2List.Insert(indexOf, s2corr);

                    // Update all the following blocks in s1 to have updated block references
                    for (int s1b = indexOf + 1; s1b < news1List.Count; s1b++)
                    {
                        news1List[s1b] = new Block(news1List[s1b].Item1, news1List[s1b].Item2, news1List[s1b].Item3 + 1);
                    }

                    // Update all the following blocks in s2 to have updated string indexes
                    for (int s2b = indexOf + 1; s2b < news2List.Count; s2b++)
                    {
                        news2List[s2b] = new Block(news2List[s2b].Item1 + insVal.Length, news2List[s2b].Item2, news2List[s2b].Item3);
                    }

                    // Update this orphan block to reference the other
                    news1List[indexOf] = new Block(s1orphan.Item1, s1orphan.Item2, indexOf);



                    // Count changes and recurse
                    changes += insVal.Length;
                    changes += MinRequiredChanges(s1, mods2, news1List, news2List);

                    min = Math.Min(min, changes);
                }
            }

            // Try change of an s2 orphan
            foreach (Block s2orphan in s2orphans)
            {
                int changes = 0;

                // Insert the content of the block we're at in the other string
                int indexOf = s2blocks.IndexOf(s2orphan);
                if (indexOf < s1blocks.Count)
                {
                    BlockList news1List = CloneList(s1blocks);
                    BlockList news2List = CloneList(s2blocks);

                    // Calculate the index in the other string to insert the value
                    int s1indx = 0;
                    for (int s1b = 0; s1b < indexOf; s1b++)
                    {
                        s1indx += s1blocks[s1b].Item2;
                    }

                    string insVal = s2.Substring(s2orphan.Item1, s2orphan.Item2);
                    string mods1 = s1.Insert(s1indx, insVal);

                    // Create the corresponding block in s1 and insert it to the list
                    Block s1corr = new Block(s1indx, s2orphan.Item2, indexOf);
                    news1List.Insert(indexOf, s1corr);

                    // Update all the following blocks in s2 to have updated block references
                    for (int s2b = indexOf + 1; s2b < news2List.Count; s2b++)
                    {
                        news2List[s2b] = new Block(news2List[s2b].Item1, news2List[s2b].Item2, news2List[s2b].Item3 + 1);
                    }

                    // Update all the following blocks in s1 to have updated string indexes
                    for (int s1b = indexOf + 1; s1b < news1List.Count; s1b++)
                    {
                        news1List[s1b] = new Block(news1List[s1b].Item1 + insVal.Length, news1List[s1b].Item2, news1List[s1b].Item3);
                    }

                    // Update this orphan block to reference the other
                    news2List[indexOf] = new Block(s2orphan.Item1, s2orphan.Item2, indexOf);



                    // Count changes and recurse
                    changes += insVal.Length;
                    changes += MinRequiredChanges(mods1, s2, news1List, news2List);

                    min = Math.Min(min, changes);
                }
            }

            return min;
        }



        private Palindrome3Map CreateMap(string data)
        {
            Palindrome3Map m = new Palindrome3Map();

            int endInc = (data.Length - 1) / 2;
            int startInc = data.Length / 2;

            for (int i = 0; i <= endInc; i++)
            {
                m.AddValue(0, data[i].ToString());
            }
            // Add the second half in reverse order
            for (int i = data.Length - 1; i >= startInc; i--)
            {
                m.AddValue(1, data[i].ToString());
            }

            return m;
        }


        private void TrimEqual(string s1, string s2, out string o1, out string o2)
        {
            int maxLen = Math.Min(s1.Length, s2.Length);

            int trimStartLen = 0;
            for (trimStartLen = 0; trimStartLen < maxLen; trimStartLen++)
            {
                if (s1.Substring(0, trimStartLen + 1) != s2.Substring(0, trimStartLen + 1))
                {
                    break;
                }
            }

            o1 = s1 = s1.Remove(0, trimStartLen);
            o2 = s2 = s2.Remove(0, trimStartLen);

            if (s1.Length == 0 || s2.Length == 0)
                return;

            maxLen = Math.Min(s1.Length, s2.Length);

            int trimEndLen = 0;
            for (trimEndLen = 0; trimEndLen < maxLen; trimEndLen++)
            {
                if (s1.Substring(s1.Length - (trimEndLen + 1)) != s2.Substring(s2.Length - (trimEndLen + 1)))
                {
                    break;
                }
            }


            o1 = s1.Substring(0, s1.Length - trimEndLen);
            o2 = s2.Substring(0, s2.Length - trimEndLen);
        }

        private void BlockifyStrings(string s1, string s2, out BlockList s1Blocks, out BlockList s2Blocks)
        {
            s1Blocks = new BlockList();
            s2Blocks = new BlockList();

            Block lasts1block = new Block(0, 0, 0);
            Block lasts2block = new Block(0, 0, 0);
            int lasts1BlockEnd;
            int lasts2BlockEnd;

            int s1i = 0;
            int subLen = 0;
            int subInd = -1;
            int mins2Ind = 0;
            while (s1i < s1.Length)
            {
                // Find the largest matching block som s1i to subLen in s2
                for (int testLen = 1; testLen <= s1.Length - s1i; testLen++)
                {
                    string sub = s1.Substring(s1i, testLen);
                    int idx = s2.IndexOf(sub, mins2Ind);

                    // If it existed, set the current subInd to idx
                    if (idx > -1)
                    {
                        // We found a match for this length too, try one more character
                        // TODO be careful, maybe we skip a nice block and find one waaay back in s2. That kinda ruins things I think
                        subInd = idx;
                        subLen = testLen;
                    }
                    // If not, exit the loop
                    else
                    {
                        break;
                    }
                }

                // Check if we found any blocks
                if (subInd > -1)
                {
                    // We found some matching blocks!
                    // First, take care of any characters before those we found

                    lasts1BlockEnd = lasts1block.Item1 + lasts1block.Item2;
                    if (s1i > lasts1BlockEnd)
                    {
                        var restBlock = new Block(lasts1BlockEnd, s1i - lasts1BlockEnd, -1);
                        s1Blocks.Add(restBlock);
                    }
                    lasts2BlockEnd = lasts2block.Item1 + lasts2block.Item2;
                    if (subInd > lasts2BlockEnd)
                    {
                        var restBlock = new Block(lasts2BlockEnd, subInd - lasts2BlockEnd, -1);
                        s2Blocks.Add(restBlock);
                    }

                    var s1b = new Block(s1i, subLen, s2Blocks.Count);
                    var s2b = new Block(subInd, subLen, s1Blocks.Count);
                    s1Blocks.Add(s1b);
                    s2Blocks.Add(s2b);

                    lasts1block = s1b;
                    lasts2block = s2b;
                    mins2Ind = lasts2block.Item1 + lasts2block.Item2;

                    s1i = lasts1block.Item1 + lasts1block.Item2;

                    subLen = 1;
                    subInd = -1;
                }
                else
                {
                    // No matches for this index, increment i and start over.
                    s1i++;
                }
            }

            // Add the final rest blocks
            lasts1BlockEnd = lasts1block.Item1 + lasts1block.Item2;
            if (lasts1BlockEnd < s1.Length)
            {
                var restBlock = new Block(lasts1BlockEnd, s1.Length - lasts1BlockEnd, -1);
                s1Blocks.Add(restBlock);
            }
            lasts2BlockEnd = lasts2block.Item1 + lasts2block.Item2;
            if (lasts2BlockEnd < s2.Length)
            {
                var restBlock = new Block(lasts2BlockEnd, s2.Length - lasts2BlockEnd, -1);
                s2Blocks.Add(restBlock);
            }
        }

        private void PrintBlocks(string s, BlockList blockDef)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Block bd in blockDef)
            {
                string segment = s.Substring(bd.Item1, bd.Item2);
                sb.AppendFormat("{0}|{1}| ", bd.Item3, segment);
            }
            OutputLine(sb.ToString());
        }


        private BlockList CloneList(BlockList list)
        {
            BlockList n = new BlockList();
            n.AddRange(list);
            return n;
        }
    }
}
