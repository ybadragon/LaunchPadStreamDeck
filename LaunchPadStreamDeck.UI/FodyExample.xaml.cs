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
using System.Windows.Shapes;
using LaunchPadStreamDeck.UI.ViewModels;

namespace LaunchPadStreamDeck.UI
{
    /// <summary>
    /// Interaction logic for FodyExample.xaml
    /// </summary>
    public partial class FodyExample : Window
    {
        public FodyExampleViewModel ViewModel = new FodyExampleViewModel();
        public FodyExample()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.TestBinding = "1234";
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var target = sender as TextBox;
            ViewModel.TestBinding = target.Text;
        }
    }
}
