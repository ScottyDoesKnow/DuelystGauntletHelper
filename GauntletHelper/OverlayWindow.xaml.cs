using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GauntletHelper
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        public OverlayWindow(OverlayWindowViewModel viewModel)
        {
            InitializeComponent();

            viewModel.Window = this;
            viewModel.Card1.CaptureArea = Card1.CaptureArea;
            viewModel.Card2.CaptureArea = Card2.CaptureArea;
            viewModel.Card3.CaptureArea = Card3.CaptureArea;

            DataContext = viewModel;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowUtility.SetWindowExTransparent(this);
        }
    }
}
