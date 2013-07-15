using ClusterManager.clusterModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager
{
    class Program
    {
        private static string infile, experinment;
        static int Main(string[] args)
        {
            int argsLength = args.Length;
            if (argsLength != 4)
            {
                Console.WriteLine("Wrong arguments Usage! The right format is : ./ClusterManager -infile inputFile -experinemnt experinment");
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
            experinment = GetArg("-experinment");

            TextReader reader = new StreamReader(infile, Encoding.UTF8);
            string json = reader.ReadLine();
            reader.Close();

            ClusterHandler clusterManager = new ClusterHandler();

            List<ItemRepresentation> items = (List<ItemRepresentation>)JsonConvert.DeserializeObject<List<ItemRepresentation>>(json);
            foreach (ItemRepresentation item in items)
            {
                clusterManager.AssignItemToCluster(item);
            }

            clusterManager.SaveClusters();

            return 0;
        }
    }
}
