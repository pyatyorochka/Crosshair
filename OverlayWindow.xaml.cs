using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;

namespace CrosshairOverlayApp
{
    public partial class OverlayWindow : Window
    {
        private int length = 50;
        private int gap = 10;
        private int thickness = 2;
        private Color color = Colors.White;
        private double opacityValue = 1.0;
        private bool isTShape = false;

        public OverlayWindow()
        {
            InitializeComponent();
            Loaded += OverlayWindow_Loaded;
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Делаем окно «сквозным» для кликов и прозрачным
            var hwnd = new WindowInteropHelper(this).Handle;
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);

            // Разворачиваем окно на весь экран
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Left = 0;
            Top = 0;

            // Первый запуск отрисовки прицела и анимации
            DrawCrosshair();
            AnimateOpacity();
        }

        public void UpdateCrosshair(int length, int gap, int thickness, System.Drawing.Color drawColor, double opacity, bool tShape)
        {
            this.length = length;
            this.gap = gap;
            this.thickness = thickness;
            this.color = Color.FromRgb(drawColor.R, drawColor.G, drawColor.B);
            this.opacityValue = opacity;
            this.isTShape = tShape;
            DrawCrosshair();
        }

        private void DrawCrosshair()
        {
            OverlayCanvas.Children.Clear();
            double centerX = Width / 2;
            double centerY = Height / 2;
            SolidColorBrush brush = new SolidColorBrush(color) { Opacity = opacityValue };

            // Вертикальная линия сверху
            var line1 = new Line
            {
                X1 = centerX,
                Y1 = centerY - gap - length,
                X2 = centerX,
                Y2 = centerY - gap,
                Stroke = brush,
                StrokeThickness = thickness
            };
            OverlayCanvas.Children.Add(line1);

            // Вертикальная линия снизу (только если это не T-образный прицел)
            if (!isTShape)
            {
                var line2 = new Line
                {
                    X1 = centerX,
                    Y1 = centerY + gap,
                    X2 = centerX,
                    Y2 = centerY + gap + length,
                    Stroke = brush,
                    StrokeThickness = thickness
                };
                OverlayCanvas.Children.Add(line2);
            }

            // Горизонтальная линия слева
            var line3 = new Line
            {
                X1 = centerX - gap - length,
                Y1 = centerY,
                X2 = centerX - gap,
                Y2 = centerY,
                Stroke = brush,
                StrokeThickness = thickness
            };
            OverlayCanvas.Children.Add(line3);

            // Горизонтальная линия справа
            // Заменили "#" на "//" — иначе C# думает, что это директива препроцессора
            var line4 = new Line
            {
                X1 = centerX + gap,
                Y1 = centerY,
                X2 = centerX + gap + length,
                Y2 = centerY,
                Stroke = brush,
                StrokeThickness = thickness
            };
            OverlayCanvas.Children.Add(line4);
        }

        private void AnimateOpacity()
        {
            foreach (var child in OverlayCanvas.Children)
            {
                if (child is Shape shape)
                {
                    var anim = new DoubleAnimation
                    {
                        From = opacityValue,
                        To = opacityValue * 0.5,
                        Duration = TimeSpan.FromSeconds(1),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    shape.BeginAnimation(Shape.OpacityProperty, anim);
                }
            }
        }

        #region Win32
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion
    }
}
