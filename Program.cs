using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace phrase_finder
{
    class Hit
    {
        public string String = null; // the string itself
        public int Count = 0; // how many times this pattern uniquely occurs
        public int Rank = 0; // 0 slice, 1 single split, 2 double split

        public Hit(string s)
        {
            String = s;
        }
    }

    struct Slice
    {
        public int Start;
        public int Length;

        public Slice(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }

    struct SingleSplit
    {
        public int StartA;
        public int LengthA;
        public int StartB;
        public int LengthB;

        public SingleSplit(int sA, int lA, int sB, int lB)
        {
            StartA = sA;
            LengthA = lA;
            StartB = sB;
            LengthB = lB;
        }
    }

    struct DoubleSplit
    {
        public int StartA;
        public int LengthA;
        public int StartB;
        public int LengthB;
        public int StartC;
        public int LengthC;

        public DoubleSplit(int sA, int lA, int sB, int lB, int sC, int lC)
        {
            StartA = sA;
            LengthA = lA;
            StartB = sB;
            LengthB = lB;
            StartC = sC;
            LengthC = lC;
        }
    }



    class Program
    {
        public static void FindSplits(string target, Dictionary<SingleSplit, string> map)
        {

            // You could post process this list to get the * values I think
            //a*c needs to start a least one character in and have a least one on the end

            // how to do a generate from one string is all you need
            // Hello -> H*llo -> H*lo -> H*o
            //       -> He*lo -> He*o
            //       -> Hel*o
            // Dictionary the allows for two slices
            // To ensure uniques
            // no problem - foreach string,
            // can do with a small test
            // for each character eat all characters to n-1
            //
            int start = 0;
            for(int openCursor = 1; openCursor < target.Length - 1; openCursor++)
            {
                for(int i = openCursor + 1; i < target.Length; i++)
                {

                    int sA = start;
                    int lA = openCursor;
                    int sB = i;
                    int lB = target.Length - i;
                    string s = (target.Substring(sA, lA) 
                        + "*"
                        + target.Substring(sB, lB) );
                    map[new SingleSplit(sA, lA, sB, lB)] = s;
                }
            }
        }

        public static void FindDoubleSplits(string target, Dictionary<DoubleSplit, string> map)
        {
                        // 1.
            // H*l*o World
            // H*l* World
            // H*l*World
            // H*l*orld
            // H*l*rld
            // H*l*ld
            // H*l*d
            // 2.
            // H*ll* World
            // H*ll*World
            // H*ll*orld
            // ...
            // For each of these the previous cur also needs to move through all
            // it's possible span

            // which is pretty good but now I need two
            int s1 = 0;

            for(int k = 1; k < target.Length - 3; k++)
            {
                int l1 = k;
                int s2 = s1 + l1 + 1;
                int e2 = target.Length - (s2 + 3);

                for(int j = 0; j < e2; j++)
                {
                    int l2 = 1 + j;

                    int end = target.Length - (s2 + l2 + 1);

                    for(int i = 1; i <= end; i++)
                    {

                        int s3 = s2 + l2 + i;
                        int l3 = target.Length - (s3);

                        string s =  target.Substring(s1, l1) 
                            + "*"
                            + target.Substring(s2, l2)
                            + "*"
                            + target.Substring(s3, l3);
                        map[new DoubleSplit(s1,l1,s2,l2,s3,l3)] = s;
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine($"Expected a single string argument");
                return;
            }
            Dictionary<Slice, string> subStrs = new Dictionary<Slice, string>();
            Dictionary<SingleSplit, string> subStrsSplit = new Dictionary<SingleSplit, string>();
            Dictionary<DoubleSplit, string> subStrsDoubleSplit = new Dictionary<DoubleSplit, string>();


            // I could seperate these out. Into HitsOne, HitsTwo, HitsThree
            Dictionary<string, Hit> hitRankOne = new Dictionary<string, Hit>();
            Dictionary<string, Hit> hitRankTwo = new Dictionary<string, Hit>();
            Dictionary<string, Hit> hitRankThree = new Dictionary<string, Hit>();

            // this will get all possible substrings

            string target = args[0];

            for(int i = 0; i < target.Length; i++)
            {
                for(int j = i; j < target.Length; j++)
                {
                    for(int k = 0; k < (target.Length - j); k++)
                    {
                        int start = j;
                        int length = target.Length - (j + k);
                        subStrs[new Slice(start, length)] = target.Substring(start, length);
                        // I can use the sub string args to filter unique slices in the string
                    }
                }
            }

            // For all possible substrings do check for splits

            foreach(string s in subStrs.Values)
            {
                FindSplits(s, subStrsSplit);
                FindDoubleSplits(s, subStrsDoubleSplit);

                if (!hitRankOne.ContainsKey(s)) 
                {  
                    hitRankOne[s] = new Hit(s);
                    hitRankOne[s].Rank = 1;
                }
                hitRankOne[s].Count++;
            }

            foreach(string item in subStrsSplit.Values)
            {
                if (!hitRankTwo.ContainsKey(item)) 
                {  
                    hitRankTwo[item] = new Hit(item);
                    hitRankTwo[item].Rank = 2;
                }
                hitRankTwo[item].Count++;
            }

            foreach(string item in subStrsDoubleSplit.Values)
            {
                if (!hitRankThree.ContainsKey(item)) 
                {  
                    hitRankThree[item] = new Hit(item);
                    hitRankThree[item].Rank = 3;
                }
                hitRankThree[item].Count++;
            }

            // Coallate
            Console.WriteLine($"{hitRankOne.Count + hitRankTwo.Count + hitRankThree.Count} values (R1: {hitRankOne.Count} R2:{hitRankTwo.Count} R3:{hitRankThree.Count})");
            var r0 = hitRankOne.Values.ToList().OrderBy(s => -s.Count).ToList();
            var r1 = hitRankTwo.Values.ToList().OrderBy(s => -s.Count).ToList();
            var r3 = hitRankThree.Values.ToList().OrderBy(s => -s.Count).ToList();


            Console.WriteLine("\n\nRank 1\n");
            int limit = 1;
            foreach(Hit item in r0)
            {
                Console.WriteLine(string.Format("{0} {1}", item.Count, item.String));
                limit++;
                if(limit >= 10)
                {
                    break;
                }
            }
            limit = 1;

            Console.WriteLine("\n\nRank 2\n");
            foreach(Hit item in r1)
            {
                Console.WriteLine(string.Format("{0} {1}", item.Count, item.String));
                limit++;
                if(limit >= 10)
                {
                    break;
                }
            }

            limit = 1;

            Console.WriteLine("\n\nRank 3\n");
            foreach(Hit item in hitRankThree.Values)
            {
                Console.WriteLine(string.Format("{0} {1} {2}", item.Rank, item.Count, item.String));
                                limit++;
                if(limit >= 10)
                {
                    break;
                }
            }
        }
    }
}
