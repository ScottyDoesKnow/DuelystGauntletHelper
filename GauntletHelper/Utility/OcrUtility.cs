using Aspose.OCR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GauntletHelper
{
    public static class OcrUtility
    {
        private static OcrEngine ocr;

        static OcrUtility()
        {
            ocr = new OcrEngine();
        }

        public static bool Process(Bitmap bitmap, out string result)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                ocr.Image = ImageStream.FromStream(stream, ImageStreamFormat.Png);

                if (ocr.Process())
                {
                    result = ocr.Text.ToString();
                    return true;
                }
                else
                {
                    result = string.Empty;
                    return false;
                }
            }
        }
    }
}
