using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaunchPadStreamDeck.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Color textColor = (Color)(ColorConverter.ConvertFromString("WhiteSmoke") ?? Color.FromRgb(0, 0, 0));
        private readonly Color overlayColor = (Color)(ColorConverter.ConvertFromString("WhiteSmoke") ?? Color.FromRgb(0, 0, 0));
        private readonly Color fillColor = Color.FromRgb(255, 0, 0);
        private readonly Brush fillBrush;
        private readonly Brush overlayBrush;
        private readonly Brush textBrush;
        private readonly List<int> rows = Enumerable.Range(0, 8).ToList();
        private readonly List<int> columns = Enumerable.Range(0, 8).ToList();
        private readonly int rectWidth = 78;
        private readonly int rectHeight = 78;
        private readonly int profileWidth = 58;
        private readonly int profileHeight = 58;
        private readonly Size arcSize = new Size(30, 30);

        public MainWindow()
        {
            InitializeComponent();
            overlayColor.A = 150;
            overlayBrush = new SolidColorBrush(overlayColor);
            fillBrush = new RadialGradientBrush(fillColor, overlayColor);
            textBrush = new SolidColorBrush(textColor);
            RenderProfileGrid();
            RenderEventGrid();
            RenderSideGrid();
        }

        private void RenderProfileGrid()
        {
            columns.ForEach(x =>
            {
                ProfileButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            });
            ProfileButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int i = 0; i < rows.Count; i++)
            {
                var textBlock = new TextBlock
                {
                    Text = $"{i+1}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = textBrush,
                    FontSize = 20
                };
                var border = new Border { Child = GetProfilePath(fillBrush), Name = $"Profile{i}", Width = rectWidth + 4, Height = rectHeight};
                border.MouseDown += Button_MouseDown;
                var stack = new StackPanel();
                stack.Children.Add(textBlock);
                stack.Children.Add(border);
                ProfileButtons.Children.Add(stack);
                Grid.SetRow(stack, 0);
                Grid.SetColumn(stack, i);
            }
        }

        private void RenderSideGrid()
        {
            rows.ForEach(x =>
            {
                SideButtons.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            });
            SideButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            for (int i = 0; i < rows.Count; i++)
            {
                var textBlock = new TextBlock
                {
                    Text = ((char)(65+i)).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = textBrush,
                    FontSize = 20
                };
                var border = new Border { Child = GetProfilePath(fillBrush), Name = $"Side{i}", Width = rectWidth, Height = rectHeight + 4};
                border.MouseDown += Button_MouseDown;
                var stack = new StackPanel{Orientation = Orientation.Horizontal};
                stack.Children.Add(border);
                stack.Children.Add(textBlock);
                SideButtons.Children.Add(stack);
                Grid.SetRow(stack, i);
                Grid.SetColumn(stack, 0);
            }
        }

        private void RenderEventGrid()
        {

            rows.ForEach(x =>
            {
                EventButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                EventButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            });

            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < columns.Count; j++)
                {
                    var border = new Border { Child = GetPath(i, j, fillBrush), Name = $"EventY{i}X{j}" };
                    border.MouseDown += Button_MouseDown;
                    EventButtons.Children.Add(border);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                }
            }
        }

        private UIElement GetProfilePath(Brush brush)
        {
            return new Ellipse
            {
                Width = profileWidth,
                Height = profileHeight,
                Fill = brush
            };
        }

        private UIElement GetNormalPath(Brush brush)
        {
            return new Rectangle
            {
                Fill = brush,
                Height = rectWidth,
                Width = rectHeight
            };
        }

        private UIElement GetCurvedPath(Brush brush, SweepDirection sweepDirection, Point pathPoint, Point arcPoint, Point linePoint)
        {
            var initialRect = new RectangleGeometry(new Rect(new Size(rectWidth, rectHeight)));
            var arcPathSegments = new PathSegmentCollection
            {
                new ArcSegment
                {
                    Point = arcPoint, Size = arcSize, SweepDirection = sweepDirection
                },
                new LineSegment {Point = linePoint}
            };

            var arcPathFigures = new PathFigureCollection
            {
                new PathFigure
                {
                    StartPoint = pathPoint,
                    Segments = arcPathSegments
                }
            };
            var arcPath = new PathGeometry
            {
                Figures = arcPathFigures
            };
            return new Path
            {
                Data = new CombinedGeometry(GeometryCombineMode.Exclude, initialRect, arcPath),
                Fill = brush
            };
        }

        private UIElement GetPath(int row, int column, Brush brush)
        {
            if (row == 3 && column == 3)
            {
                return GetCurvedPath(brush, SweepDirection.Clockwise, new Point(55, 78), new Point(78, 55), new Point(78, 78));
            }

            if (row == 3 && column == 4)
            {
                return GetCurvedPath(brush, SweepDirection.Clockwise, new Point(0, 55), new Point(25, 78), new Point(0, 78));
            }

            if (row == 4 && column == 3)
            {
                return GetCurvedPath(brush, SweepDirection.Counterclockwise, new Point(55, 0), new Point(78, 25), new Point(78, 0));
            }

            if (row == 4 && column == 4)
            {
                return GetCurvedPath(brush, SweepDirection.Counterclockwise, new Point(0, 25), new Point(25, 0), new Point(0, 0));
            }

            return GetNormalPath(brush);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            MessageBox.Show(element.Name, "Clicked", MessageBoxButton.OK);
        }
    }
}
