namespace Danqzq
{
    public interface IReadMemory
    {
        public void OnMemoryWrite(short address, short value);
    }
}