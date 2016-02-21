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
        private const int SYMBOLS_WIDTH = 260 - (1 * 2) - (1 * 3); // Control width minus margin and border

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

        /// <summary>
        /// Needed because the viewbox seems to either resize things to fit when not necessary (resulting in tiny font) or not resize anything.
        /// </summary>
        public double SymbolsWidth
        {
            get { return symbolsWidth; }
            set
            {
                symbolsWidth = value;
                RaisePropertyChanged("SymbolsWidth");
            }
        }
        private double symbolsWidth = SYMBOLS_WIDTH;

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

            char[] symbolChars = card.Symbols.ToCharArray();
            string symbols = string.Empty;
            foreach (char symbol in symbolChars)
            {
                if (!string.IsNullOrEmpty(symbols))
                    symbols += Environment.NewLine;

                if (data.Symbols.ContainsKey(symbol.ToString()))
                    symbols += data.Symbols[symbol.ToString()];
                else
                    symbols += "Unknown symbol encountered.";
            }

            Symbols = symbols;
            SymbolsWidth = symbolChars.Length > 2 ? double.NaN : SYMBOLS_WIDTH;
        }
    }
}
