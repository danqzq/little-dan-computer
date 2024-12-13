using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Danqzq
{
    public class GraphicsScreen : DeviceComponent, IReadMemory
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private RawImage _rawImage;

        [SerializeField] private ColorMode _colorMode;

        private Color[] _colors = new Color[100];
        
        private Stack<short> _values = new Stack<short>();
        
        private int GetSize()
        {
            return _colorMode switch
            {
                ColorMode.RGBA32 => 50,
                ColorMode.RGB48 => 33,
                _ => 100
            };
        }
        
        private int GetOneSideSize()
        {
            return _colorMode switch
            {
                ColorMode.RGBA32 => 5,
                ColorMode.RGB48 => 3,
                _ => 10
            };
        }
        
        private enum ColorMode
        {
            RGB656,
            RGB555,
            RGB444,
            RGBA32,
            RGB48
        }
        
        public void OnMemoryWrite(short address, short value)
        {
            switch (_colorMode)
            { 
                case ColorMode.RGBA32:
                    HandleTwoValueDraw(address, value);
                    break;
                case ColorMode.RGB48:
                    HandleThreeValueDraw(address, value);
                    break;
                default:
                    SingleValueDraw(address, value);
                    break;
            }
        }
        
        private void HandleTwoValueDraw(short address, short value)
        {
            _values.Push(value);
            if (_values.Count == 2)
            {
                TwoValueDraw((short) (address / 2), _values.Pop(), _values.Pop());
            }
        }
        
        private void HandleThreeValueDraw(short address, short value)
        {
            _values.Push(value);
            if (_values.Count == 3)
            {
                var b = _values.Pop();
                var g = _values.Pop();
                var r = _values.Pop();
                SingleValueDraw((short) (address / 3), RGB48(r, g, b));
            }
        }

        private void SingleValueDraw(short address, short value)
        {
            var color = _colorMode switch
            {
                ColorMode.RGB656 => RGB565(value),
                ColorMode.RGB555 => RGB555(value),
                ColorMode.RGB444 => RGB444(value),
                _ => Color.black
            };
            SingleValueDraw(address, color);
        }
        
        private void SingleValueDraw(short address, Color color)
        {
            var sideSize = GetOneSideSize();
            var index = address % sideSize * address / sideSize;
            if (index >= _colors.Length)
            {
                return;
            }
            
            _colors[index] = color;
            DrawPixel(address % sideSize, address / sideSize, color);
        }
        
        private void TwoValueDraw(short address, short value1, short value2)
        {
            var sideSize = GetOneSideSize();
            var color = RGBA32(value1, value2);
            var index = address % sideSize * address / sideSize;
            _colors[index] = color;
            DrawPixel(address % sideSize, address / sideSize, color);
        }
        
        protected override void Start()
        {
            base.Start();
            UpdateTexture();
            Clear();
        }
        
        private static Color RGB565(short value)
        {
            var r = (value & 0xF800) >> 8;
            var g = (value & 0x07E0) >> 3;
            var b = (value & 0x001F) << 3;
            return new Color((ushort)r / 31f, (ushort)g / 63f, (ushort)b / 31f);
        }
        
        private static Color RGB555(short value)
        {
            var r = (value & 0x7C00) >> 7;
            var g = (value & 0x03E0) >> 2;
            var b = (value & 0x001F) << 3;
            return new Color((ushort)r / 31f, (ushort)g / 31f, (ushort)b / 31f);
        }
        
        private static Color RGB444(short value)
        {
            var r = (value & 0x0F00) >> 4;
            var g = (value & 0x00F0);
            var b = (value & 0x000F) << 4;
            return new Color((ushort)r / 15f, (ushort)g / 15f, (ushort)b / 15f);
        }

        private static Color RGB48(short r, short g, short b)
        {
            return new Color((ushort)r / 65535f, (ushort)g / 65535f, (ushort)b / 65535f);
        }
        
        private static Color RGBA32(short value1, short value2)
        {
            var r = value1 >> 8;
            var g = value1 & 0xFF;
            var b = value2 >> 8;
            var a = value2 & 0xFF;
            return new Color((byte)r / 255f, (byte)g / 255f, (byte)b / 255f, (byte)a / 255f);
        }

        private void DrawPixel(int x, int y, Color color, bool apply = true)
        {
            _texture.SetPixel(x, y, color);
            if (apply) _texture.Apply();
        }
        
        public void ChangeColorMode(int mode)
        {
            _colorMode = (ColorMode)mode;
            
            UpdateTexture();
            Clear();
        }
        
        public void Clear()
        {
            var sideSize = GetOneSideSize();
            for (byte i = 0; i < _colors.Length; i++)
            {
                _colors[i] = Color.black;
                DrawPixel(i % sideSize, i / sideSize, Color.black, false);
            }
            
            _texture.Apply();
        }

        private void UpdateTexture()
        {
            var size = GetSize();
            var sideSize = GetOneSideSize();
            _texture = new Texture2D(sideSize, sideSize)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            _colors = new Color[size];
            
            _texture.Apply();
            _rawImage.texture = _texture;
        }
    }
}