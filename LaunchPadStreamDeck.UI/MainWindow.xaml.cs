using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaunchPadStreamDeck.API.Classes;
using LaunchPadStreamDeck.API.Enums;
using LaunchPadStreamDeck.API.Services;

namespace LaunchPadStreamDeck.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Color textColor = (Color)(ColorConverter.ConvertFromString("WhiteSmoke") ?? Color.FromRgb(0, 0, 0));
        private readonly Color overlayColor = (Color)(ColorConverter.ConvertFromString("WhiteSmoke") ?? Color.FromRgb(0, 0, 0));
        private readonly Color radialInnerColor = Color.FromRgb(255, 0, 0);
        private readonly Color radialOuterColor = Color.FromArgb(153, 255, 172, 172);
        private readonly Brush fillBrush;
        private readonly Brush textBrush;
        private readonly List<int> rows = Enumerable.Range(0, 8).ToList();
        private readonly List<int> columns = Enumerable.Range(0, 8).ToList();
        private readonly int rectWidth = 78;
        private readonly int rectHeight = 78;
        private readonly int profileWidth = 58;
        private readonly int profileHeight = 58;
        private readonly Size arcSize = new Size(30, 30);
        private readonly LaunchpadService _launchPadService = new LaunchpadService();

        public MainWindow()
        {
            InitializeComponent();
            overlayColor.A = 150;
            fillBrush = new RadialGradientBrush(overlayColor, overlayColor);
            textBrush = new SolidColorBrush(textColor);
            RenderProfileGrid();
            RenderEventGrid();
            RenderSideGrid();
            _launchPadService.SetButtonPressed(ButtonPressed);
            _launchPadService.SetButtonDown(ButtonDown);
        }

        private void ButtonDown(object sender, ButtonPressEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var button = GetUIButton(e);
                HandleButtonDown(button, e);
            });
        }

        private void ButtonPressed(object sender, ButtonPressEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var button = GetUIButton(e);

                HandleButtonUp(button, e);
            });

        }

        private FrameworkElement GetUIButton(ButtonPressEventArgs e)
        {
            switch (e.Type)
            {
                case ButtonType.Grid:
                    return GetEventButton(e);
                case ButtonType.Side:
                    return GetSidebarButton(e);
                case ButtonType.Toolbar:
                    return GetProfileButton(e);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private LaunchpadButton GetLaunchpadButton(FrameworkElement element)
        {
            int index = Convert.ToInt32(element.Name.Last().ToString());
            if (element.Name.StartsWith("Profile"))
            {
                return _launchPadService.GetButton(ButtonType.Toolbar, index);
            }

            if (element.Name.StartsWith("Side"))
            {
                return _launchPadService.GetButton(ButtonType.Side, index);
            }

            var X = Convert.ToInt32(element.Name.Substring(element.Name.IndexOf("X") + 1, 1));
            var Y = Convert.ToInt32(element.Name.Substring(element.Name.IndexOf("Y") + 1, 1));

            var button = _launchPadService.Device[X, Y];
            return button;
        }

        private ButtonPressEventArgs GetLaunchpadButtonEventArgs(LaunchpadButton button, FrameworkElement element)
        {
            if (button.ButtonType == ButtonType.Toolbar)
            {
                return new ButtonPressEventArgs((ToolbarButton) button.mIndex, button);
            }
            if (button.ButtonType == ButtonType.Side)
            {
                return new ButtonPressEventArgs((SideButton) button.mIndex, button);
            }

            var X = Convert.ToInt32(element.Name.Substring(element.Name.IndexOf("X") + 1, 1));
            var Y = Convert.ToInt32(element.Name.Substring(element.Name.IndexOf("Y") + 1, 1));


            return new ButtonPressEventArgs(X, Y, button, button.mIndex);
        }

        private FrameworkElement GetProfileButton(ButtonPressEventArgs e)
        {
            var stack = ProfileButtons.Children.Cast<StackPanel>().ToList()[(int)e.ToolbarButton];
            return stack.Children.OfType<Border>().First();
        }

        private FrameworkElement GetSidebarButton(ButtonPressEventArgs e)
        {
            var stack = SideButtons.Children.Cast<StackPanel>().ToList()[(int) e.SidebarButton];
            return stack.Children.OfType<Border>().First();
        }

        private FrameworkElement GetEventButton(ButtonPressEventArgs e)
        {
            return EventButtons.Children.OfType<Border>().First(x => Grid.GetRow(x) == e.Y && Grid.GetColumn(x) == e.X);
        }

        private void HandleButtonUp(FrameworkElement button, ButtonPressEventArgs e)
        {
            SetButtonColor(button, e, ButtonPressState.Up);
        }

        private void HandleButtonDown(FrameworkElement button, ButtonPressEventArgs e)
        {
            if (e.Type == ButtonType.Grid)
            {
                ((Border) button).Child = GetPath(e.Y, e.X, fillBrush);
            }
            else
            {
                ((Border) button).Child = GetProfilePath(fillBrush);
            }
        }

        private void SetButtonColor(FrameworkElement button, ButtonPressEventArgs e, ButtonPressState buttonState)
        {
            var outerColor = e.Button.RedBrightness == ButtonBrightness.Full ? overlayColor : radialOuterColor;
            var fillColor = e.Button.RedBrightness == ButtonBrightness.Full ? overlayColor : radialInnerColor;
            var redBrightness = e.Button.RedBrightness == ButtonBrightness.Full
                ? ButtonBrightness.Off
                : ButtonBrightness.Full;
            var newBrush = new RadialGradientBrush(fillColor, outerColor);
            newBrush.RadiusX = .6;
            newBrush.RadiusY = .6;
            if (e.Type == ButtonType.Grid)
            {
                ((Border)button).Child = GetPath(e.Y, e.X, newBrush);
            }
            else
            {
                ((Border)button).Child = GetProfilePath(newBrush);
            }

            _launchPadService.SetButtonBrightness(e.Button, redBrightness, ButtonBrightness.Off);

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
                    Text = $"{i + 1}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = textBrush,
                    FontSize = 20
                };
                var border = new Border { Child = GetProfilePath(fillBrush), Name = $"Profile{i}", Width = rectWidth + 4, Height = rectHeight };
                border.MouseUp += Button_MouseUp;
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
                SideButtons.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            });
            SideButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            for (int i = 0; i < rows.Count; i++)
            {
                var textBlock = new TextBlock
                {
                    Text = ((char)(65 + i)).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = textBrush,
                    FontSize = 20
                };
                var border = new Border { Child = GetProfilePath(fillBrush), Name = $"Side{i}", Width = rectWidth, Height = rectHeight + 4 };
                border.MouseUp += Button_MouseUp;
                var stack = new StackPanel { Orientation = Orientation.Horizontal };
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
                    border.MouseUp += Button_MouseUp;
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

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var button = GetLaunchpadButton(element);
            var eventArgs = GetLaunchpadButtonEventArgs(button, element);
            HandleButtonUp(element, eventArgs);
        }
    }
}
