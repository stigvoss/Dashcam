using Library.Dashcam.Extensibility.Interfaces;
using System.Drawing;

namespace Timestamp
{
    public class ModuleConfiguration : IConfiguration
    {
        private const string DEFAULT_FONT_NAME = "Ariel";
        private const int DEFAULT_FONT_SIZE = 18;

        private string _format = "dd/MM/yyyy hh:mm:ss";
        private Brush _fontColor = Brushes.White;
        private PointF _position = new PointF(8f, 8f);

        private Font _font;

        public ModuleConfiguration()
        {
            _font = new Font(DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE);
        }

        public string Format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
            }
        }

        public Font Font
        {
            get
            {
                return _font;
            }
        }

        public string FontName
        {
            get
            {
                return _font.Name;
            }
        }

        public float Size
        {
            get
            {
                return _font.Size;
            }
        }

        public Brush Color
        {
            get
            {
                return _fontColor;
            }
            set
            {
                _fontColor = value;
            }
        }

        public PointF Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
    }
}