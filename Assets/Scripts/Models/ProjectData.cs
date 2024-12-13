namespace Danqzq.Models
{
    [System.Serializable]
    public struct ProjectData
    {
        public string[] assemblyLines;
        public int[] memory;

        public DeviceComponentData[] deviceComponents;
    }
}