using JsonParser.model;
using JsonParser.network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParser
{
    public class Program
    {
        private static string infile, outfile;

        public static int Main(string[] args)
        {
            DateTime startdt = DateTime.UtcNow;
            int argsLength = args.Length;
            if (argsLength != 4)
            {
                Console.WriteLine("Wrong arguments Usage! The right format is : ./JsonParser -infile inputFile -outfile outputFile");
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

            TextReader reader = new StreamReader(infile, Encoding.UTF8);
            JsonReader jReader = new JsonTextReader(reader);
            JArray jsonArray = JArray.Load(jReader);
            reader.Close();
            ArrayList writeList = new ArrayList();

            foreach (JObject jsonObject in jsonArray)
            {
                JsonObjectHandler jsonHandler = new JsonObjectHandler(jsonObject);
                 
                if(jsonHandler.source.Value.ToString().Contains("foursquare")) {
                        continue;
                }

                if (jsonHandler.hashtagsArray.Count != 0)
                {
                    jsonHandler.processHashTags();
                }

                ParsedBlog[] blogsArray = null;
                if (jsonHandler.urlsArray.Count != 0)
                {
                    UrlHandler urlManager = new UrlHandler(jsonHandler.urlsArray, jsonHandler.text);
                    urlManager.processUrls();
                    if (urlManager.blogList.Count != 0)
                    {
                        blogsArray = urlManager.blogList.ToArray();
                    }
                    jsonHandler.text = urlManager.text;
                }

                string[] hashtagsTextArray = null;
                if (jsonHandler.hashtagsArray.Count != 0)
                {
                    hashtagsTextArray = (string[])jsonHandler.hashtagTexts.ToArray(typeof(string));
                }

                if (jsonHandler.text.Contains("http"))
                {
                    Console.WriteLine(jsonHandler.id + " : " + jsonHandler.urlsArray.Count + " - " + jsonHandler.text);
                    Console.WriteLine("oops");
                }
                ParsedTweet tweet = new ParsedTweet(jsonHandler.id, jsonHandler.text, jsonHandler.createdDateTime, jsonHandler.geolon, jsonHandler.geolat, jsonHandler.place, hashtagsTextArray, jsonHandler.userId, blogsArray);
                string json = JsonConvert.SerializeObject(tweet);
                writeList.Add(json);

            }

            TextWriter writer = new StreamWriter(outfile);
            writer.Write("[");
            writer.Write(String.Join(",", writeList.ToArray()));
            //foreach (string json in writeList)
            //{
            //    writer.WriteLine(json + ",");
            //}
            writer.Write("]");
            writer.Close();

            DateTime stopdt = DateTime.UtcNow;

            Console.WriteLine("Time passed to complete 1k json : " + (stopdt - startdt));
            

            return 0;
        }
    }
}
