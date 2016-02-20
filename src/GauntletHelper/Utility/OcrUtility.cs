using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace GauntletHelper
{
    public static class OcrUtility
    {
        private static TesseractEngine tesseract;

        static OcrUtility()
        {
            tesseract = new TesseractEngine("tessdata", "eng", EngineMode.Default, "config");
            tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }

        public static bool ProcessTesseract(Bitmap bitmap, out string result)
        {
            using (Tesseract.Page page = tesseract.Process(bitmap, PageSegMode.SingleBlock))
                result = page.GetText().Trim();

            return true;
        }
    }
}
