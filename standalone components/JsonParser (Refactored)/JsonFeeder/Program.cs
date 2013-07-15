using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JsonFeeder
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(@"H:\thesis\dataset\cross-grouped");
           // HashSet<Thread> threadSet = new HashSet<Thread>();
            foreach (string file in files)
            {
                string fileName = file.Split('\\').Last();
                string[] parameters = { "-infile", file, "-outfile", @"H:\thesis\dataset\cross_parsed\" + fileName };
                JsonParser.Program.Main(parameters);
                //ThreadStart starter = delegate () { JsonParser.Program.Main(parameters); };
                //Thread thread =new Thread(starter);
                //thread.Start();
                //if (!threadSet.Contains(thread))
                //{
                //    threadSet.Add(thread);
                //}

                //while (threadSet.Count == 1)
                //{
                //    foreach (Thread threadTemp in threadSet)
                //    {
                //        if (!threadTemp.IsAlive)
                //        {
                //            threadSet.Remove(threadTemp);
                //            break;
                //        }
                //    }
                //}

            }
            Console.Read();

        }
    }
}
