using System;
using System.Collections;
using System.Collections.Generic;

namespace phrase_finder
{
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

            // this will get all possible substrings


            string target = args[0];

            // for(int i = 0; i < target.Length; i++)
            // {
            //     for(int j = i; j < target.Length; j++)
            //     {
            //         for(int k = 0; k < (target.Length - j); k++)
            //         {
            //             int start = j;
            //             int length = target.Length - (j + k);
            //             subStrs[new Slice(start, length)] = target.Substring(start, length);
            //             // I can use the sub string args to filter unique slices in the string
            //         }
            //     }
            // }

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
            // int start = 0;
            // for(int openCursor = 1; openCursor < target.Length - 1; openCursor++)
            // {
            //     for(int i = openCursor + 1; i < target.Length; i++)
            //     {

            //         int sA = start;
            //         int lA = openCursor;
            //         int sB = i;
            //         int lB = target.Length - i;
            //         string s = (target.Substring(sA, lA) 
            //             + "*"
            //             + target.Substring(sB, lB) );
            //         subStrsSplit[new SingleSplit(sA, lA, sB, lB)] = s;
            //     }
            // }


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
                        subStrsDoubleSplit[new DoubleSplit(s1,l1,s2,l2,s3,l3)] = s;
                    }
                }
            }

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

            // foreach(var item in subStrs.Values)
            // {
            //     Console.WriteLine(item);
            // }

            // foreach(var item in subStrsSplit.Values)
            // {
            //     Console.WriteLine(item);
            // }

            foreach(var item in subStrsDoubleSplit.Values)
            {
                Console.WriteLine(item);  
            }
        }
    }
}
