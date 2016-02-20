using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GauntletHelper
{
    public class DuelystData
    {
        private const string URL = @"https://docs.google.com/document/d/1JKJ5fjvwhchefhJcrQDOgRIwqaDKpj13PRstVyn7mG0/pub";
        private const int MIN_NAME_DISTANCE = 5; // Going over 2 means nonsense will sometimes match to Maw, but it's necessary for some cards.

        public Dictionary<string, int> Factions { get; set; }
        public Dictionary<string, string> Symbols { get; set; }
        public Dictionary<string, Dictionary<string, Card>> Cards { get; set; }

        private Dictionary<string, string[]> cardNames = null;

        public DuelystData()
        {
            Factions = new Dictionary<string, int>();
            Symbols = new Dictionary<string, string>();
            Cards = new Dictionary<string, Dictionary<string, Card>>();
        }

        #region GetData

        public void GetData()
        {
            HtmlDocument doc = DownloadPage(URL);

            int tableIndex = 0;
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//tbody"))
            {
                switch (tableIndex)
                {
                    case 0:
                        {
                            foreach (HtmlNode row in table.SelectNodes("tr").Skip(1))
                            {
                                string name = row.ChildNodes[0].InnerText;
                                int value = int.Parse(row.ChildNodes[1].InnerText);

                                Factions.Add(name, value);
                            }
                            break;
                        }
                    case 1:
                        {
                            foreach (HtmlNode row in table.SelectNodes("tr").Skip(1))
                            {
                                string symbol = row.ChildNodes[0].InnerText;
                                string description = row.ChildNodes[1].InnerText;

                                // Anal
                                if (!description.EndsWith("."))
                                    description += ".";

                                Symbols.Add(ReplaceAmp(symbol), description);
                            }
                            break;
                        }
                    default:
                        {
                            string faction = table.SelectNodes("tr").First().InnerText;
                            faction = faction.Substring(0, faction.IndexOf(" "));

                            if (faction != "Rating") // Currently there's a second rating table on the page
                            {
                                Cards.Add(faction, new Dictionary<string, Card>());

                                foreach (HtmlNode row in table.SelectNodes("tr").Skip(2))
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        string name = row.ChildNodes[0 + (i * 3)].InnerText;
                                        if (!string.IsNullOrEmpty(name))
                                        {
                                            int value;
                                            if (!int.TryParse(row.ChildNodes[1 + (i * 3)].InnerText, out value))
                                                value = -1;

                                            string symbols = row.ChildNodes[2 + (i * 3)].InnerText;

                                            Cards[faction].Add(name, new Card(name, value, ReplaceAmp(symbols)));
                                        }
                                    }

                                }
                            }
                            break;
                        }
                }

                tableIndex++;
            }
        }

        private static string ReplaceAmp(string input)
        {
            return input.Replace("&amp;", "&");
        }

        private static HtmlDocument DownloadPage(string url)
        {
            HtmlDocument doc = new HtmlDocument();
            using (WebClient client = new WebClient())
                doc.LoadHtml(client.DownloadString(url));
            return doc;
        }

        #endregion

        #region GetCard

        public bool GetCard(string faction, string name, out Card result)
        {
            if (cardNames == null)
                BuildCardNames();

            Tuple<string, int> closestName = LevenshteinDistance.ComputeDistance(name, cardNames[faction]);
            // Console.WriteLine("{0}\t{1}\t{2}", name, closestName.Item1, closestName.Item2);

            result = Cards[faction][closestName.Item1];
            return closestName.Item2 <= MIN_NAME_DISTANCE;
        }

        private void BuildCardNames()
        {
            cardNames = new Dictionary<string, string[]>();
            foreach (var factionCards in Cards)
            {
                HashSet<string> cardNameSet = new HashSet<string>();

                foreach (var card in factionCards.Value)
                    if (!cardNameSet.Contains(card.Key))
                        cardNameSet.Add(card.Key);

                cardNames.Add(factionCards.Key, cardNameSet.ToArray());
            }
        }

        #endregion
    }
}
