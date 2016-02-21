using GalaSoft.MvvmLight;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace GauntletHelper
{
    public class CardControlViewModel : ViewModelBase
    {
        #region Binding

        public string Rating
        {
            get { return rating; }
            set
            {
                rating = value;
                RaisePropertyChanged("Rating");
            }
        }
        private string rating;

        public string Symbols
        {
            get { return symbols; }
            set
            {
                symbols = value;
                RaisePropertyChanged("Symbols");
            }
        }
        private string symbols;

        #endregion

        public FrameworkElement CaptureArea { get; set; }

        private DuelystData data;

        public CardControlViewModel(DuelystData duelystData)
        {
            data = duelystData;
            SetCard(Card.Placeholder);
        }

        public void SetCard(Card card)
        {
            if (string.IsNullOrEmpty(card.Name))
                Rating = string.Empty;
            else
                Rating = string.Format("{0}: {1}", card.Name, card.Value == -1 ? "Undefined" : card.Value.ToString());

            string symbols = string.Empty;
            foreach (char symbol in card.Symbols.ToCharArray())
            {
                if (!string.IsNullOrEmpty(symbols))
                    symbols += Environment.NewLine;

                if (data.Symbols.ContainsKey(symbol.ToString()))
                    symbols += "- " + data.Symbols[symbol.ToString()];
                else
                    symbols += "- Unknown symbol encountered.";
            }

            Symbols = symbols;
        }
    }
}
