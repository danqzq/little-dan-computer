namespace Danqzq
{
    public class Accumulator : ValueDisplay
    {
        public void Load(short value)
        {
            Value = value;
        }
        
        public void Add(short value)
        {
            Value += value;
        }
        
        public void Subtract(short value)
        {
            Value -= value;
        }
        
        public void Multiply(short value)
        {
            Value *= value;
        }
        
        public void Divide(short value)
        {
            Value /= value;
        }
        
        public void Modulus(short value)
        {
            Value %= value;
        }
        
        public void Clear()
        {
            Value = 0;
        }
    }
}