using ClusterManager.clusterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.similarity
{
    class CosineSimilarity : SimilarityHandlerInterface
    {

        private List<ItemClusterRepresentation> clustersList;
        private List<ClusterGroup> clusterGroupList;

        public CosineSimilarity(List<ClusterGroup> clusterGroupList)
        {
            clustersList = new List<ItemClusterRepresentation>();
            this.clusterGroupList = clusterGroupList;
        }

        public void CalculateSimilarity(clusterModel.ItemRepresentation item)
        {
            foreach (ClusterGroup cluster in clusterGroupList)
            {
                ItemRepresentation centralizedItem = cluster.centralizedItem;
                float similarity = (float)GetSimilarity(centralizedItem.vectorValues, item.vectorValues);
                ItemClusterRepresentation tempItemClusterRepresantation = new ItemClusterRepresentation(cluster.groupId, item.id, similarity);
                clustersList.Add(tempItemClusterRepresantation);
            }
        }

        private float GetSimilarity(float[] clusterVector, float[] itemVector)
        {
            float numerator = 0;
            float similarity = 0;
            float clusterDenominator = 0;
            float itemDenominator = 0;
            for (int i = 0; i < clusterVector.Length; i++)
            {
                numerator += (float)(clusterVector[i] * itemVector[i]);
                clusterDenominator += (float)(Math.Pow(clusterVector[i], 2));
                itemDenominator += (float)(Math.Pow(itemVector[i], 2));
            }

            similarity = (float)(numerator / (Math.Sqrt(clusterDenominator) * Math.Sqrt(itemDenominator)));


            return similarity;
        }

        public void OrderSimilarities()
        {
            clustersList = clustersList.OrderByDescending(x => x.similarity).ToList();
        }

        public ItemClusterRepresentation GetGroupId()
        {
            return clustersList.First();
        }
    }
}
