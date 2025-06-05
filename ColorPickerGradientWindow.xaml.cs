using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrosshairOverlayApp
{
    public partial class ColorPickerGradientWindow : Window
    {
        private const int GradientWidth = 360;
        private const int GradientHeight = 256;

        private WriteableBitmap? _gradientBitmap;

        public Color? SelectedColor { get; private set; }

        public ColorPickerGradientWindow(Color initialColor)
        {
            InitializeComponent();

            GenerateGradient();
            SetInitialPreview(initialColor);
        }

        private void GenerateGradient()
        {
            _gradientBitmap = new WriteableBitmap(
                GradientWidth,
                GradientHeight,
                96,
                96,
                PixelFormats.Bgra32,
                null);

            int stride = GradientWidth * (_gradientBitmap.Format.BitsPerPixel / 8);
            byte[] pixelData = new byte[GradientHeight * stride];

            for (int y = 0; y < GradientHeight; y++)
            {
                double v = 1.0 - (double)y / (GradientHeight - 1);

                for (int x = 0; x < GradientWidth; x++)
                {
                    double h = (double)x / (GradientWidth - 1) * 360.0;
                    double s = 1.0;

                    Color rgb = HsvToRgb(h, s, v);

                    int pixelOffset = y * stride + x * 4;
                    pixelData[pixelOffset + 0] = rgb.B;
                    pixelData[pixelOffset + 1] = rgb.G;
                    pixelData[pixelOffset + 2] = rgb.R;
                    pixelData[pixelOffset + 3] = 255;
                }
            }
            Int32Rect rect = new Int32Rect(0, 0, GradientWidth, GradientHeight);
            _gradientBitmap.WritePixels(rect, pixelData, stride, 0);

            GradientImage.Source = _gradientBitmap;
        }

        private void SetInitialPreview(Color initialColor)
        {
            SelectedColor = initialColor;
            ColorPreview.Background = new SolidColorBrush(initialColor);
            ApplyButton.IsEnabled = true;
        }

        private void GradientImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(GradientImage);
            SelectColorAtPoint(p);
        }

        private void GradientImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(GradientImage);
                SelectColorAtPoint(p);
            }
        }

        private void SelectColorAtPoint(Point p)
        {
            if (_gradientBitmap == null)
                return;

            int x = (int)Math.Round(p.X);
            int y = (int)Math.Round(p.Y);

            if (x < 0) x = 0;
            if (x >= GradientWidth) x = GradientWidth - 1;
            if (y < 0) y = 0;
            if (y >= GradientHeight) y = GradientHeight - 1;

            try
            {
                int stride = GradientWidth * (_gradientBitmap.Format.BitsPerPixel / 8);
                byte[] pixelData = new byte[4];
                Int32Rect rect = new Int32Rect(x, y, 1, 1);
                _gradientBitmap.CopyPixels(rect, pixelData, 4, 0);

                byte b = pixelData[0];
                byte g = pixelData[1];
                byte r = pixelData[2];

                Color picked = Color.FromRgb(r, g, b);
                SelectedColor = picked;
                ColorPreview.Background = new SolidColorBrush(picked);
                ApplyButton.IsEnabled = true;
            }
            catch
            {}
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private Color HsvToRgb(double h, double s, double v)
        {
            if (s == 0)
            {
                byte vv = (byte)(v * 255);
                return Color.FromRgb(vv, vv, vv);
            }

            double hh = h / 60.0;
            int sector = (int)Math.Floor(hh);
            double fraction = hh - sector;
            double p = v * (1 - s);
            double q = v * (1 - s * fraction);
            double t = v * (1 - s * (1 - fraction));

            double r = 0, g = 0, b = 0;
            switch (sector)
            {
                case 0:
                    r = v; g = t; b = p;
                    break;
                case 1:
                    r = q; g = v; b = p;
                    break;
                case 2:
                    r = p; g = v; b = t;
                    break;
                case 3:
                    r = p; g = q; b = v;
                    break;
                case 4:
                    r = t; g = p; b = v;
                    break;
                case 5:
                default:
                    r = v; g = p; b = q;
                    break;
            }

            return Color.FromRgb(
                (byte)(r * 255),
                (byte)(g * 255),
                (byte)(b * 255));
        }
    }
}
