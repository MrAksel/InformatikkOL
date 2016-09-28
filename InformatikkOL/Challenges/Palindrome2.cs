using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Block = System.Tuple<int, int, int>;
using BlockList = System.Collections.Generic.List<System.Tuple<int, int, int>>;

namespace InformatikkOL.Challenges
{
    [Challenge(displayName: "Palindrom (reanalysering)")]
    class Palindrome2 : Challenge
    {
        int minLim = 1;

        public override void Main()
        {
            MuteProgress(false);

            int totChanges = 0;


            string input = InputLine();
            if (File.Exists(input))
                input = File.ReadAllText(input);

            input = input.ToLower().Replace(" ", "").Replace(",", "");

            // Demo of blocks
            string s1, s2;
            SplitInput(input, out s1, out s2);

            BlockList s1blocks;
            BlockList s2blocks;

            BlockifyStrings(s1, s2, out s1blocks, out s2blocks);

            PrintBlocks(s1, s1blocks);
            PrintBlocks(s2, s2blocks);
            ProgressLine("");
            PrintBlocks(s1, s2, s1blocks, s2blocks);

            // Actual calculation
            DecreaseIndent();
            totChanges = MinRequiredChanges(s1, s2);

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

        /// <summary>
        /// Counts the minimum number of required changes to make i1 and i2 equal. Does some optimizing checks and processes input before passing on to MinRequiredChanges2.
        /// </summary>
        private int MinRequiredChanges(string i1, string i2)
        {
            IncreaseIndent();
            ProgressLine("Count changes required for '{0}'-'{1}'", i1, Reverse(i2));

            int min = short.MaxValue;

            string s1, s2;
            TrimEqual(i1, i2, out s1, out s2);

            ProgressLine("Trimmed to '{0}'-'{1}'", s1, Reverse(s2));

            if (s1.Length == 1 && s2.Length == 1)
                min = 1;
            if (s1 == s2)
                min = 0;

            if (min > 1)
                min = MinRequiredChanges2(s1, s2);

            ProgressLine("ret {0}", min);
            DecreaseIndent();

            return min;
        }

        /// <summary>
        /// Transforms i1 and i2 to be equal and counts the number of required changes to do so. Tries to do this by either changing (replacing), removing or inserting a block.
        /// </summary>
        private int MinRequiredChanges2(string i1, string i2)
        {
            int min = short.MaxValue;

            string s1, s2;
            //TrimEqual(i1, i2, out s1, out s2);
            s1 = i1;
            s2 = i2;

            int chang = short.MaxValue, insrt = short.MaxValue, remov = short.MaxValue;

            BlockList s1blocks;
            BlockList s2blocks;

            BlockifyStrings(s1, s2, out s1blocks, out s2blocks);

            // Simulate change of an orphan block
            if (min > 1)
            {
                IncreaseIndent();
                chang = SimulateChange(s1, s2, s1blocks, s2blocks);
                min = Math.Min(min, chang);
                DecreaseIndent();
                ProgressLine("- By changes: {0}", chang);
            }

            // Simulate removal of an orphan block
            if (min > 1)
            {
                IncreaseIndent();
                remov = SimulateRemoval(s1, s2, s1blocks, s2blocks);
                min = Math.Min(min, remov);
                DecreaseIndent();
                ProgressLine("- By removal: {0}", remov);
            }

            // Simulate insertion of an orphan block into the other
            if (min > 1)
            {
                IncreaseIndent();
                insrt = SimulateInsertion(s1, s2, s1blocks, s2blocks);
                min = Math.Min(min, insrt);
                DecreaseIndent();
                ProgressLine("- By inserts: {0}", insrt);
            }

            return min;
        }

        /// <summary>
        /// Counts the number of changes required after removing one block from s1 or s2.
        /// </summary>
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
            for (int orphanIndex = 0; orphanIndex < s1orphans.Count && min > minLim; orphanIndex++)
            {
                Block s1orphan = s1orphans[orphanIndex];
                int changes = 0;

                // Remove the block in question from the string
                string mods1 = s1.Remove(s1orphan.Item1, s1orphan.Item2);

                // Count changes and recurse
                changes += s1orphan.Item2;
                ProgressLine("Trying removal ({0}): {1} -> {2}", changes, s1, mods1);
                changes += MinRequiredChanges(mods1, s2);

                min = Math.Min(min, changes);
            }

            // Try removal of an s2 orphan
            for (int orphanIndex = 0; orphanIndex < s2orphans.Count && min > minLim; orphanIndex++)
            {
                Block s2orphan = s2orphans[orphanIndex];
                int changes = 0;

                // Remove the block in question from the string
                string mods2 = s2.Remove(s2orphan.Item1, s2orphan.Item2);

                // Count changes and recurse
                changes += s2orphan.Item2;
                ProgressLine("Trying removal ({0}): {1} -> {2}", changes, s2, mods2);
                changes += MinRequiredChanges(s1, mods2);

                min = Math.Min(min, changes);
            }

            return min;
        }

        /// <summary>
        /// Counts the number of changes required after replacing one block from s1 or s2 into the other.
        /// </summary>
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
            for (int orphanIndex = 0; orphanIndex < s1orphans.Count && min > minLim; orphanIndex++)
            {
                Block s1orphan = s1orphans[orphanIndex];
                int changes = 0;

                // Replace the content of the block we're at with the content of the corresponding block
                int indexOf = s1blocks.IndexOf(s1orphan);
                int corrIndex = 0;
                if (indexOf > 0)
                {
                    corrIndex = s1blocks[indexOf - 1].Item3 + 1;
                }
                if (corrIndex < s2blocks.Count)
                {
                    Block s2corr = s2blocks[corrIndex];

                    string mods1 = s1.Remove(s1orphan.Item1, s1orphan.Item2);
                    string insVal = s2.Substring(s2corr.Item1, s2corr.Item2);
                    mods1 = mods1.Insert(s1orphan.Item1, insVal);

                    int charDiff = insVal.Length - s1orphan.Item2;

                    // Count changes and recurse
                    changes += (charDiff > 0 ? insVal.Length : s1orphan.Item2);
                    ProgressLine("Trying change ({0}): {1} -> {2}", changes, s1, mods1);

                    changes += MinRequiredChanges(mods1, s2);

                    min = Math.Min(min, changes);
                }
            }

            // Try change of an s2 orphan
            for (int orphanIndex = 0; orphanIndex < s2orphans.Count && min > minLim; orphanIndex++)
            {
                Block s2orphan = s2orphans[orphanIndex];
                int changes = 0;

                // Replace the content of the block we're at with the content of the corresponding block
                int indexOf = s2blocks.IndexOf(s2orphan);
                int corrIndex = 0;
                if (indexOf > 0)
                {
                    corrIndex = s2blocks[indexOf - 1].Item3 + 1;
                }
                if (corrIndex < s1blocks.Count)
                {
                    Block s1corr = s1blocks[corrIndex];

                    string mods2 = s2.Remove(s2orphan.Item1, s2orphan.Item2);
                    string insVal = s1.Substring(s1corr.Item1, s1corr.Item2);
                    mods2 = mods2.Insert(s2orphan.Item1, insVal);

                    int charDiff = insVal.Length - s2orphan.Item2;

                    // Count changes and recurse
                    changes += (charDiff > 0 ? insVal.Length : s2orphan.Item2);
                    ProgressLine("Trying change ({0}): {1} -> {2}", changes, s2, mods2);

                    changes += MinRequiredChanges(s1, mods2);

                    min = Math.Min(min, changes);
                }
            }

            return min;
        }

        /// <summary>
        /// Counts the number of changes required after inserting one block from s1 or s2 into the other.
        /// </summary>
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
            for (int orphanIndex = 0; orphanIndex < s1orphans.Count && min > minLim; orphanIndex++)
            {
                Block s1orphan = s1orphans[orphanIndex];
                int changes = 0;

                // Insert the content of the block we're at in the other string
                int indexOf = s1blocks.IndexOf(s1orphan);
                int afterBlock = -1;
                if (indexOf > 0)
                {
                    afterBlock = s1blocks[indexOf - 1].Item3;
                }
                if (afterBlock <= s2blocks.Count)
                {
                    // Calculate the index in the other string to insert the value
                    int s2indx = 0;
                    for (int s2b = 0; s2b <= afterBlock; s2b++)
                    {
                        s2indx += s2blocks[s2b].Item2;
                    }

                    string insVal = s1.Substring(s1orphan.Item1, s1orphan.Item2);
                    string mods2 = s2.Insert(s2indx, insVal);

                    // Count changes and recurse
                    changes += insVal.Length;
                    ProgressLine("Trying insertion ({0}): {1} -> {2}", changes, s2, mods2);
                    changes += MinRequiredChanges(s1, mods2);

                    min = Math.Min(min, changes);
                }
            }

            // Try change of an s2 orphan
            for (int orphanIndex = 0; orphanIndex < s2orphans.Count && min > minLim; orphanIndex++)
            {
                Block s2orphan = s2orphans[orphanIndex];
                int changes = 0;

                // Insert the content of the block we're at in the other string
                int indexOf = s2blocks.IndexOf(s2orphan);
                int afterBlock = -1;
                if (indexOf > 0)
                {
                    afterBlock = s2blocks[indexOf - 1].Item3;
                }
                if (afterBlock <= s1blocks.Count)
                {
                    // Calculate the index in the other string to insert the value
                    int s1indx = 0;
                    for (int s1b = 0; s1b <= afterBlock; s1b++)
                    {
                        s1indx += s1blocks[s1b].Item2;
                    }

                    string insVal = s2.Substring(s2orphan.Item1, s2orphan.Item2);
                    string mods1 = s1.Insert(s1indx, insVal);

                    // Count changes and recurse
                    changes += insVal.Length;
                    ProgressLine("Trying change ({0}): {1} -> {2}", changes, s1, mods1);
                    changes += MinRequiredChanges(mods1, s2);

                    min = Math.Min(min, changes);
                }
            }

            return min;
        }


        private string Reverse(string input)
        {
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// Splits one string into two halves.
        /// </summary>
        private void SplitInput(string input, out string s1, out string s2)
        {
            int bothGetsMiddle = 0;

            s1 = input.Substring(0, (input.Length + bothGetsMiddle) / 2);
            s2 = Reverse(input.Remove(0, input.Length / 2));
        }

        /// <summary>
        /// Removes common characters from the beginning of both strings
        /// </summary>
        private void TrimEqual(string s1, string s2, out string o1, out string o2)
        {
            bool trimEnd = false;

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

            if (trimEnd)
            {
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
        }

        /// <summary>
        /// Structures the two strings into blocks. Find common substrings of s1 in s2, and mark others as orphans.
        /// </summary>
        /// <param name="s1Blocks">
        /// A list of blocks in s1. Blocks are 3-tuples where Item1 is the index in s1, Item2 is the length of the substring, and Item3 is the index in s2Blocks of the corresponding block (or -1 if it's an orphan).
        /// </param>
        /// <param name="s2Blocks">
        /// A list of blocks in s2. Blocks are 3-tuples where Item1 is the index in s1, Item2 is the length of the substring, and Item3 is the index in s1Blocks of the corresponding block (or -1 if it's an orphan).
        /// </param>
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

            // Start trying to find substrings at s1i = 0 and continue up to the whole string
            while (s1i < s1.Length)
            {
                // The maximum length of substrings from this index (number of chars remaining from this index to the end)
                int maxSubLen = s1.Length - s1i;
                // Try larger and larger substrings from s1i until we no longer find the substring in the other string
                for (int testLen = 1; testLen <= maxSubLen; testLen++)
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

                // Check if we found a common block in both strings
                if (subInd > -1)
                {
                    // We found two matching blocks!
                    // First, take care of any characters left over before those we found (those that aren't in a block)

                    // Indexes where the last block in s1 ended.
                    lasts1BlockEnd = lasts1block.Item1 + lasts1block.Item2;
                    if (s1i > lasts1BlockEnd)
                    {
                        // There are characters in between these two blocks, add it as an orphan (a block with no corresponding block in s2)
                        var restBlock = new Block(lasts1BlockEnd, s1i - lasts1BlockEnd, -1);
                        s1Blocks.Add(restBlock);
                    }
                    lasts2BlockEnd = lasts2block.Item1 + lasts2block.Item2;
                    if (subInd > lasts2BlockEnd)
                    {
                        var restBlock = new Block(lasts2BlockEnd, subInd - lasts2BlockEnd, -1);
                        s2Blocks.Add(restBlock);
                    }

                    // Add the common blocks we found to the list of blocks, and make them reference each other (that is, they're not orphans)
                    var s1b = new Block(s1i, subLen, s2Blocks.Count);
                    var s2b = new Block(subInd, subLen, s1Blocks.Count);
                    s1Blocks.Add(s1b);
                    s2Blocks.Add(s2b);

                    lasts1block = s1b;
                    lasts2block = s2b;
                    mins2Ind = lasts2block.Item1 + lasts2block.Item2;

                    // Jump to the end of the block and continue searching from there
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

            // If there are characters left over after the last blocks, add these as orphans as well
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

        /// <summary>
        /// Prints out the string split into the blocks in blockDef
        /// </summary>
        private void PrintBlocks(string s, BlockList blockDef)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Block bd in blockDef)
            {
                string segment = s.Substring(bd.Item1, bd.Item2);
                sb.AppendFormat("{0}|{1}| ", bd.Item3, segment);
            }
            ProgressLine(sb);
        }

        private void PrintBlocks(string s1, string s2, BlockList s1blocks, BlockList s2blocks)
        {
            StringBuilder sShort = new StringBuilder();
            StringBuilder sLong = new StringBuilder();
            BlockList shortestList, longestList;
            string shortString, longString;

            if (s1blocks.Count <= s2blocks.Count)
            {
                shortestList = s1blocks;
                shortString = s1;
                longestList = s2blocks;
                longString = s2;
            }
            else
            {
                shortestList = s2blocks;
                shortString = s2;
                longestList = s1blocks;
                longString = s1;
            }

            int currStringIndex = 0;
            int margins = 4;
            int bsoff = 0;
            int bloff = 0;
            for (int bi = 0; bi < shortestList.Count; bi++)
            {
                Block b1 = shortestList[bi + bsoff];
                Block b2 = longestList[bi + bloff];

                string sub1 = shortString.Substring(b1.Item1, b1.Item2);
                string sub2 = longString.Substring(b2.Item1, b2.Item2);

                if (b1.Item3 == -1 && b2.Item3 == -1)
                {
                    // Both are orphans
                    sShort.AppendFormat("|{0}|", sub1);
                    sLong.AppendFormat("|{0}|", sub2);
                    currStringIndex += Math.Max(sub1.Length, sub2.Length);
                }
                else if (b1.Item3 == -1)
                {
                    // b1 is an orphan, b2 is not. Process b2 later
                    sShort.AppendFormat("|{0}|", sub1);
                    bloff--;
                    currStringIndex += sub1.Length;
                }
                else if (b2.Item3 == -1)
                {
                    // b2 is an orphan, b1 is not. Process b1 later
                    sLong.AppendFormat("|{0}|", sub2);
                    bsoff--;
                    currStringIndex += sub2.Length;
                }
                else
                {
                    // Neither are orphans, check that they are referencing each other
                    Debug.Assert(b1.Item3 == bi + bloff);
                    Debug.Assert(b2.Item3 == bi + bsoff);

                    sShort.AppendFormat("|{0}|", sub1);
                    sLong.AppendFormat("|{0}|", sub2);
                    currStringIndex += Math.Max(sub1.Length, sub2.Length);
                }


                currStringIndex += margins;
                sShort.Append(' ', currStringIndex - sShort.Length);
                sLong.Append(' ', currStringIndex - sLong.Length);
            }

            for (int bi = shortestList.Count + bloff; bi < longestList.Count; bi++)
            {
                Block b2 = longestList[bi + bloff];
                string sub2 = longString.Substring(b2.Item1, b2.Item2);

                sLong.AppendFormat("|{0}|", sub2);

                currStringIndex += sub2.Length + margins;
                sLong.Append(' ', margins);
            }

            if (s1 == shortString)
            {
                ProgressLine(sShort);
                ProgressLine(sLong);
            }
            else
            {
                ProgressLine(sLong);
                ProgressLine(sShort);
            }
        }

        private BlockList CloneList(BlockList list)
        {
            BlockList n = new BlockList();
            n.AddRange(list);
            return n;
        }
    }
}
