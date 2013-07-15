using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.clusterModel
{
    [Serializable]
    class ClusterGroup
    {


        public ClusterGroup()
        {
            clusterGroupList = new List<ItemRepresentation>();
        }


        public List<ItemRepresentation> clusterGroupList { get; set; }
        public ItemRepresentation centralizedItem { get; set; }
        public int groupId { get; set; }

    }
}
