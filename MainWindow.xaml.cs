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

        // DependencyProperty для биндинга превью-цвета
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

            // Инициализируем превью-цвет (белый по умолчанию)
            SelectedColorBrush = new SolidColorBrush(
                Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B)
            );

            // Обновление текстовых полей значениями из слайдеров
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

            // Обработчик кнопки выбора цвета
            ColorButton.Click += ColorButton_Click;

            // Кнопки Apply / Exit
            ApplyButton.Click += ApplyButton_Click;
            ExitButton.Click += (s, e) => { this.Close(); };

            // Запуск оверлея
            overlayWindow = new OverlayWindow();
            overlayWindow.Show();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ColorDialog
            {
                Color = selectedColor
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedColor = dlg.Color;
                SelectedColorBrush = new SolidColorBrush(
                    Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B)
                );
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
