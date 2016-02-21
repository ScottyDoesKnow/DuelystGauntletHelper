using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace GauntletHelper
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Binding

        public ObservableCollection<string> Factions { get; private set; }

        public ObservableCollection<Tuple<string, int>> FactionRatings { get; private set; }

        public bool ProgressRunning
        {
            get { return progressRunning; }
            set
            {
                progressRunning = value;
                RaisePropertyChanged("ProgressRunning");
            }
        }
        private bool progressRunning = false;

        public string ProgressText
        {
            get { return progressText; }
            set
            {
                progressText = value;
                RaisePropertyChanged("ProgressText");
            }
        }
        private string progressText = string.Empty;

        #endregion

        public OverlayWindowViewModel OverlayWindow { get; set; }
        private DuelystData data = new DuelystData();

        public MainWindowViewModel()
        {
            Factions = new ObservableCollection<string>();
            FactionRatings = new ObservableCollection<Tuple<string, int>>();

            DownloadData();

            OverlayWindow = new OverlayWindowViewModel(data);
        }

        private async void DownloadData()
        {
            ProgressRunning = true;
            ProgressText = "Downloading and parsing data...";

            await Task.Run(() =>
            {
                data.GetData();

                DispatcherHelper.RunAsync(() =>
                {
                    foreach (var faction in data.Factions)
                    {
                        Factions.Add(faction.Key);
                        FactionRatings.Add(new Tuple<string, int>(faction.Key, faction.Value));
                    }
                });
            });

            ProgressRunning = false;
            ProgressText = "Select faction in top left to enable helper.";
        }
    }
}
