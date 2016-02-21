using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletHelper
{
    public class Card
    {
        public static Card Placeholder { get; private set; }

        static Card()
        {
            Placeholder = new Card(string.Empty, -1, string.Empty);
        }

        public string Name { get; set; }
        public int Value { get; set; }
        public string Symbols { get; set; }

        public Card(string name, int value, string symbols)
        {
            Name = name;
            Value = value;
            Symbols = symbols;
        }
    }
}
