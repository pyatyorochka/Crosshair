using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Interop;

namespace CrosshairOverlayApp
{
    public partial class MainWindow : Window
    {
        private System.Drawing.Color selectedColor = System.Drawing.Color.White;
        private OverlayWindow overlayWindow;

        public SolidColorBrush SelectedColorBrush
        {
            get { return (SolidColorBrush)GetValue(SelectedColorBrushProperty); }
            set { SetValue(SelectedColorBrushProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorBrushProperty =
            DependencyProperty.Register(
                nameof(SelectedColorBrush),
                typeof(SolidColorBrush),
                typeof(MainWindow),
                new PropertyMetadata(new SolidColorBrush(Colors.White))
            );

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            SelectedColorBrush = new SolidColorBrush(
                Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B)
            );

            LengthSlider.ValueChanged += (s, e) =>
            {
                LengthValue.Text = ((int)LengthSlider.Value).ToString();
            };
            GapSlider.ValueChanged += (s, e) =>
            {
                GapValue.Text = ((int)GapSlider.Value).ToString();
            };
            ThicknessSlider.ValueChanged += (s, e) =>
            {
                ThicknessValue.Text = ((int)ThicknessSlider.Value).ToString();
            };
            OpacitySlider.ValueChanged += (s, e) =>
            {
                OpacityValue.Text = OpacitySlider.Value.ToString("0.0");
            };

            ColorButton.Click += ColorButton_Click;

            ApplyButton.Click += ApplyButton_Click;
            ExitButton.Click += (s, e) => { this.Close(); };

            overlayWindow = new OverlayWindow();
            overlayWindow.Show();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Преобразуем текущий selectedColor (System.Drawing.Color) в WPF Color
            var initialWpfColor = Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B);

            // Создаём наш WPF-градентный диалог
            var picker = new ColorPickerGradientWindow(initialWpfColor)
            {
                Owner = this
            };

            bool? result = picker.ShowDialog();
            if (result == true)
            {
                // Переключаемся из WPF Color в System.Drawing.Color
                var wpfC = picker.SelectedColor.Value;
                selectedColor = System.Drawing.Color.FromArgb(wpfC.R, wpfC.G, wpfC.B);

                // Обновляем превью-границу
                SelectedColorBrush = new SolidColorBrush(wpfC);
            }
        }



        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var length = (int)LengthSlider.Value;
            var gap = (int)GapSlider.Value;
            var thickness = (int)ThicknessSlider.Value;
            var opacity = OpacitySlider.Value;
            var isTShape = TShapeCheckbox.IsChecked == true;
            overlayWindow.UpdateCrosshair(length, gap, thickness, selectedColor, opacity, isTShape);
        }
    }
}
