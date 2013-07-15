using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.clusterModel
{
    class ItemClusterRepresentation
    {
        public int groupId { get; set; }
        public string itemId { get; set; }
        public float similarity { get; set; }

        public ItemClusterRepresentation(int groupId, string itemId, float similarity)
        {
            this.groupId = groupId;
            this.itemId = itemId;
            this.similarity = similarity;
        }


    }
       
}
