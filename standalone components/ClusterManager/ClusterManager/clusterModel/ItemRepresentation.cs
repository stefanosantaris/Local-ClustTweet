using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterManager.clusterModel
{
    [Serializable]
    class ItemRepresentation
    {
        public ItemRepresentation(string id, float[] vectorValues, bool itemExist)
        {
            this.id = id;
            this.vectorValues = vectorValues;
            this.itemExists = itemExists;
        }

        public string id { get; set; }
        public float[] vectorValues { get; set; }
        public bool itemExists { get; set; }
    }
}
