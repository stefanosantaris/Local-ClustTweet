using ClusterManager.clusterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager
{
    interface SimilarityHandlerInterface
    {
        void CalculateSimilarity(ItemRepresentation item);
        void OrderSimilarities();
        ItemClusterRepresentation GetGroupId();
    }
}
