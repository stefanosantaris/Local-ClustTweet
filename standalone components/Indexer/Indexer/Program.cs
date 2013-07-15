using Indexer.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    class Program
    {
        private static string infile, outfile, command;
        private static IndexHandler indexManager;
        static int Main(string[] args)
        {
            int argsLength = args.Length;
            if (argsLength != 6)
            {
                Console.WriteLine("Wrong arguments Usage! The right format is : ./Indexer -infile inputFile -outfile outputFile -command command");
                return -1;
            }

            Func<string, string> GetArg = (name) =>
            {
                for (int i = 0; i < argsLength; i++)
                {
                    if (string.Equals(name, args[i]))
                    {
                        return args[i + 1];
                    }
                }
                return string.Empty;
            };

            infile = GetArg("-infile");
            outfile = GetArg("-outfile");
            command = GetArg("-command");



            TextReader reader = new StreamReader(infile, Encoding.UTF8);
            string json = reader.ReadLine();
            reader.Close();
            List<ParsedTweet> tweetList = (List<ParsedTweet>)JsonConvert.DeserializeObject<List<ParsedTweet>>(json);
            indexManager = new IndexHandler();

            if (command.Equals("index"))
            {
                IndexList(tweetList);
            }
            else if (command.Equals("search"))
            {
                ArrayList itemRepresentationList = SearchList(tweetList);
                SaveList(itemRepresentationList);
            }

            return 0;
        }

        private static void SaveList(ArrayList itemRepresentationList)
        {
            ArrayList tempList = new ArrayList();
            foreach (ItemRepresentation item in itemRepresentationList)
            {
                
                string json = JsonConvert.SerializeObject(item);
                tempList.Add(json);
            }
            TextWriter writer = new StreamWriter(outfile);
            writer.Write('[');
            writer.Write(String.Join(",", tempList.ToArray()));
            writer.Write(']');
            writer.Close();
        }

        private static ArrayList SearchList(List<ParsedTweet> jsonArray)
        {
            ArrayList writeList = new ArrayList();
            foreach (ParsedTweet tweet in jsonArray)
            {
                ItemRepresentation item = indexManager.GetItemRepresentation(tweet);
                writeList.Add(item);
            }

            return writeList;
           

        }

        private static void IndexList(List<ParsedTweet> jsonArray)
        {
            foreach (ParsedTweet tweet in jsonArray)
            {
                indexManager.IndexTweet(tweet);
            }

            indexManager.ImplementTfIdfArray();
            indexManager.SaveProperties();
        }
    }
}
