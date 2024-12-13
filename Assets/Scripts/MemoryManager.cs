using System.Collections.Generic;
using UnityEngine;

namespace Danqzq
{
    public class MemoryManager : MonoBehaviour
    {
        [SerializeField] private Chunk[] _chunks;

        [SerializeField] private DeviceComponent[] _deviceComponents;

        [field: SerializeField] public short Size { get; private set; } = 100;
        
        private List<IReadMemory> _memoryReaders;
        
        private void Start()
        {
            InitializeDeviceComponents();
            Clear();
        }
        
        private void InitializeDeviceComponents()
        {
            _memoryReaders = new List<IReadMemory>();
            for (byte i = 0; i < _deviceComponents.Length; i++)
            {
                if (_deviceComponents[i] is IReadMemory reader)
                {
                    _memoryReaders.Add(reader);
                }
            }
        }

        public void DeselectAll()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Deselect();
            }
        }
        
        public void Write(short address, short value)
        { 
            if (address < 0 || address >= Size)
            {
                return;
            }
            _chunks[address].Value = value;
            foreach (var reader in _memoryReaders)
            {
                reader.OnMemoryWrite(address, value);
            }
        }
        
        public short Read(short address)
        {
            return _chunks[address].Value;
        }
        
        public void Clear()
        {
            for (byte i = 0; i < _chunks.Length; i++)
            {
                _chunks[i].Init(i, 0);
            }
        }
    }
}