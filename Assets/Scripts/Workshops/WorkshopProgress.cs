namespace Danqzq.Workshops
{
    [System.Serializable]
    public struct WorkshopProgress
    {
        public byte[] workshops;
        
        public byte this[int index]
        {
            get => workshops[index];
            set => workshops[index] = value;
        }
        
        public WorkshopProgress(int workshopCount)
        {
            workshops = new byte[workshopCount];
        }
    }
}