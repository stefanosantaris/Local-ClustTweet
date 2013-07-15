using ClusterManager.clusterModel;
using ClusterManager.similarity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager
{
    class ClusterHandler
    {
        private ClusterCollection clusterCollection;
        private readonly float threshold = (float) 0.2;
        public ClusterHandler()
        {
            InitializeClusters();
        }

        private void InitializeClusters()
        {
            if (!File.Exists("ClusterCollection.dat"))
            {
                clusterCollection = new ClusterCollection();
            }
            else
            {
                FileStream fs = new FileStream("ClusterCollection.dat", FileMode.Open);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    clusterCollection = (ClusterCollection)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason : " + e.Message);
                    clusterCollection = new ClusterCollection();
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        public void SaveClusters()
        {
            FileStream fs = new FileStream("ClusterCollection.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, clusterCollection);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason : " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public void AssignItemToCluster(ItemRepresentation item)
        {
            if (clusterCollection.collectionList.Count == 0)
            {
                CreateNewCluster(item);
            }
            else
            {
                IdentifyClusterAndAssign(item);
            }
        }

        private void IdentifyClusterAndAssign(ItemRepresentation item)
        {
            CosineSimilarity similarity = new CosineSimilarity(clusterCollection.collectionList);
            similarity.CalculateSimilarity(item);
            similarity.OrderSimilarities();
            ItemClusterRepresentation cluster = similarity.GetGroupId();
            if (cluster.similarity > threshold)
            {
                item.itemExists = true;
                clusterCollection.collectionList.ElementAt(cluster.groupId).clusterGroupList.Add(item);
                CalculateCentralizedItem(cluster.groupId);
            }
            else
            {
                item.itemExists = true;
                CreateNewCluster(item);
            }
        }

        private void CalculateCentralizedItem(int id)
        {
            List<ItemRepresentation> clusterItemList = clusterCollection.collectionList.ElementAt(id).clusterGroupList;
            List<int> fakeItemsIds = new List<int>();
            for (int i = 0; i < clusterItemList.Count; i++)
            {
                if (!clusterItemList.ElementAt(i).itemExists)
                {
                    fakeItemsIds.Add(i);
                }
            }

            for (int i = 0; i < fakeItemsIds.Count; i++)
            {
                clusterItemList.RemoveAt(fakeItemsIds.ElementAt(i));
            }


            int arrayLength = clusterItemList.First().vectorValues.Length;
            float[] tempArray = new float[arrayLength];
            foreach (ItemRepresentation item in clusterItemList)
            {
                float[] itemArray = item.vectorValues;
                for (int i = 0; i < itemArray.Length; i++)
                {
                    tempArray[i] += itemArray[i];
                }
            }

            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = tempArray[i] / clusterItemList.Count;
            }

            ItemRepresentation tempItemRepresentation = new ItemRepresentation("centerID", tempArray, false);
            clusterItemList.Add(tempItemRepresentation);

            clusterCollection.collectionList.ElementAt(id).clusterGroupList = clusterItemList;

        }

        private void CreateNewCluster(ItemRepresentation item)
        {
            
            ClusterGroup newGroup = new ClusterGroup();
            newGroup.clusterGroupList.Add(item);
            newGroup.centralizedItem = item;
            newGroup.groupId = clusterCollection.collectionList.Count;
            
            clusterCollection.collectionList.Add(newGroup);
        }
    }
}
