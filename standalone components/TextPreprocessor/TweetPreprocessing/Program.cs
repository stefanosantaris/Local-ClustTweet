using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextPreprocessor.model;
using TextPreprocessor.office;

namespace TextPreprocessor
{
    class Program
    {

        public static WordHandler wordManager;
        public static Dictionary<string, string> slangDictionary = new Dictionary<string,string>();
        static int Main(string[] args)
        {
            DateTime startDt = DateTime.UtcNow;
            int argsLength = args.Length;

            // Console.Out.WriteLine(argsLength);
            if (argsLength != 4)   //NEW
            {
                Console.WriteLine("Wrong Arguments Usage! The right format is: ./TweetPreprocessing -infile inputFile -outfile outputFile");//NEW
                return 1;
            }


            Func<string, string> GetArg = (name) =>
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (string.Equals(name, args[i]))
                        return args[i + 1];
                }
                return string.Empty;
            };

            string infile = GetArg("-infile");
            string outfile = GetArg("-outfile");


            TextReader reader = new StreamReader(infile, Encoding.UTF8);
            string jsonString = reader.ReadLine();
            reader.Close();
            List<ParsedTweet> tweetList = (List<ParsedTweet>)JsonConvert.DeserializeObject < List<ParsedTweet>>(jsonString);

            ArrayList preprocessedTweetList = new ArrayList();

            DBConnect databaseManager = new DBConnect();
            databaseManager.SelectAll();
            wordManager = new WordHandler();

            #region preprocessing area
            foreach (ParsedTweet tweet in tweetList)
            {
                TweetPreprocessor tweetProcessor = new TweetPreprocessor(tweet);
                tweetProcessor.PreprocessTweet();
                if (tweetProcessor.tweet == null)
                    continue;

                if (tweetProcessor.tweet.blogs != null)
                {
                    BlogPreprocessor blogProcessor = new BlogPreprocessor(tweetProcessor.tweet.blogs);
                    blogProcessor.PreprocessBlogs();
                    tweetProcessor.tweet.blogs = blogProcessor.blogs;
                }
                string preprocessedJson = JsonConvert.SerializeObject(tweetProcessor.tweet);
                preprocessedTweetList.Add(preprocessedJson);
                
            }

            #endregion


            wordManager.CloseWord();


            TextWriter writer = new StreamWriter(outfile);
            writer.Write('[');
            writer.Write(String.Join(",",  preprocessedTweetList.ToArray()));
            writer.Write(']');

            writer.Close();
            DateTime stopDT = DateTime.UtcNow;
            Console.WriteLine("Time passed to complete 1k tweets preprocessing : " + (stopDT - startDt));
            Console.Read();
            return 0;
        }
    }
}
