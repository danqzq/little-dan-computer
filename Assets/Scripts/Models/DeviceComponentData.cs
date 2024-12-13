using UnityEngine;

namespace Danqzq.Models
{
    [System.Serializable]
    public struct DeviceComponentData
    {
        public Vector2 position;
        
        public DeviceComponentData(Vector2 position)
        {
            this.position = position;
        }
    }
}