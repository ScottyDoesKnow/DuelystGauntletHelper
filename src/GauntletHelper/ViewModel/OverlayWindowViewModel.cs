using GalaSoft.MvvmLight;
using GauntletHelper.Properties;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace GauntletHelper
{
    public class OverlayWindowViewModel : ViewModelBase
    {
        private const string PROCESS_NAME = "duelyst";
        private const int COLOR_SIMILARITY_MIN = 150;

        #region Binding

        public CardControlViewModel Card1 { get; private set; }
        public CardControlViewModel Card2 { get; private set; }
        public CardControlViewModel Card3 { get; private set; }

        public string SelectedFaction
        {
            get { return selectedFaction; }
            set
            {
                selectedFaction = value;
                RaisePropertyChanged("SelectedFaction");
            }
        }
        private string selectedFaction = string.Empty;

        public bool AllowVisible
        {
            get { return allowVisible; }
            set
            {
                allowVisible = value;
                RaisePropertyChanged("AllowVisible");
            }
        }
        private bool allowVisible = true;

        public bool Windowed
        {
            get { return Settings.Default.Windowed; }
            set
            {
                Settings.Default.Windowed = value;
                Settings.Default.Save();

                RaisePropertyChanged("Windowed");
            }
        }

        public Visibility Visibility
        {
            get { return visiblity; }
            set
            {
                visiblity = value;
                RaisePropertyChanged("Visibility");
            }
        }
        private Visibility visiblity = Visibility.Hidden;

        public double Top
        {
            get { return top; }
            set
            {
                top = value;
                RaisePropertyChanged("Top");
            }
        }
        private double top;

        public double Left
        {
            get { return left; }
            set
            {
                left = value;
                RaisePropertyChanged("Left");
            }
        }
        private double left;

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                RaisePropertyChanged("Height");
            }
        }
        private double height;

        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                RaisePropertyChanged("Width");
            }
        }
        private double width;

        #endregion

        public Window Window { get; set; }

        private DuelystData data;
        private List<CardControlViewModel> cardViewModels = new List<CardControlViewModel>();
        private Timer placeWindowTimer, updateCardsTimer;
        private bool updateCardsRunning = false;

        public OverlayWindowViewModel(DuelystData duelystData)
        {
            data = duelystData;

            Card1 = new CardControlViewModel(data);
            Card2 = new CardControlViewModel(data);
            Card3 = new CardControlViewModel(data);

            cardViewModels.Add(Card1);
            cardViewModels.Add(Card2);
            cardViewModels.Add(Card3);

            placeWindowTimer = new Timer(PlaceWindow, null, 25, 25);
            updateCardsTimer = new Timer(UpdateCards, null, 1000, 1000);
        }

        #region Timer

        private void PlaceWindow(object state)
        {
            PlaceWindow();
        }

        private void PlaceWindow()
        {
            // > -1000 check below is a minimized check, it shows -32000 when minimized but can be slightly negative while maximized but not in fullscreen mode
            Rectangle rectangle;
            if (WindowUtility.GetRectangle(PROCESS_NAME, out rectangle) && rectangle.Top > -1000 && AllowVisible && !string.IsNullOrEmpty(SelectedFaction))
            {
                // Binding window dimensions/location is really delayed for some reason, so just doing it directly
                try
                {
                    Window.Dispatcher.Invoke(new Action(() =>
                    {
                        Window.Visibility = Visibility.Visible;

                        Window.Top = rectangle.Top;
                        Window.Left = rectangle.Left;
                        Window.Height = rectangle.Height - (Windowed ? 0 : 22);
                        Window.Width = rectangle.Width;
                    }));
                }
                catch (TaskCanceledException) { return; } // Happens on close, too lazy to do it right
            }
            else
                Visibility = Visibility.Hidden;
        }

        private void UpdateCards(object state)
        {
            lock (placeWindowTimer)
                if (updateCardsRunning)
                    return;
                else
                    updateCardsRunning = true;

            UpdateCards();

            lock (placeWindowTimer)
                updateCardsRunning = false;
        }

        private void UpdateCards()
        {
            if (Visibility == Visibility.Visible)
            {
                Bitmap screenGrab;
                if (WindowUtility.GrabScreen(PROCESS_NAME, out screenGrab))
                {
                    foreach (CardControlViewModel cardViewModel in cardViewModels)
                        cardViewModel.SetCard(GetCard(screenGrab, cardViewModel));

                    screenGrab.Dispose();
                }
            }
        }

        private Card GetCard(Bitmap screenGrab, CardControlViewModel cardViewModel)
        {
            using (Bitmap cardNameImage = GetCardNameImage(screenGrab, cardViewModel))
                if (cardNameImage != null)
                {
                    string imageName;
                    if (OcrUtility.ProcessTesseract(cardNameImage, out imageName))
                    {
                        Card result;
                        if (!string.IsNullOrEmpty(imageName) && data.GetCard(SelectedFaction, imageName, out result))
                            return result;
                    }
                }

            return Card.Placeholder;
        }

        private Bitmap GetCardNameImage(Bitmap screenGrab, CardControlViewModel cardViewModel)
        {
            Rect rect = Rect.Empty;
            try { Window.Dispatcher.Invoke(new Action(() => rect = cardViewModel.CaptureArea.TransformToVisual(Window).TransformBounds(cardViewModel.CaptureArea.RenderTransform.TransformBounds(new Rect(cardViewModel.CaptureArea.RenderSize))))); }
            catch (TaskCanceledException) { return null; } // Happens on close, too lazy to do it right

            Bitmap bitmap;
            try { bitmap = screenGrab.Clone(new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height), screenGrab.PixelFormat); }
            catch (OutOfMemoryException) { return null; }

            int whiteValue = Color.White.R + Color.White.G + Color.White.B;
            for (int w = 0; w < bitmap.Width; ++w)
                for (int h = 0; h < bitmap.Height; ++h)
                {
                    Color pixelColor = bitmap.GetPixel(w, h);
                    int pixelValue = pixelColor.R + pixelColor.G + pixelColor.B;
                    if (whiteValue - pixelValue < COLOR_SIMILARITY_MIN)
                        bitmap.SetPixel(w, h, Color.Black);
                    else
                        bitmap.SetPixel(w, h, Color.White);
                }

            // Window.Dispatcher.Invoke(new Action(() => new TestBitmapWindow(bitmap).ShowDialog()));

            return bitmap;
        }

        #endregion
    }
}
