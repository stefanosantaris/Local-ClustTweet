using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.clusterModel
{
    [Serializable]
    class ClusterCollection
    {
        public List<ClusterGroup> collectionList { get; set; }
        public ClusterCollection()
        {
            collectionList = new List<ClusterGroup>();
        }
    }
}
