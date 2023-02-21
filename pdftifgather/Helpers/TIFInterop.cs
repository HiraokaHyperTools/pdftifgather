using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    internal class TIFInterop
    {
        internal static iTextSharp.text.Rectangle GetPageSizeWithRotation(TIFPageReader tifPageReader, int pageNum)
        {
            var dib = FreeImage.LockPage(tifPageReader.TiffSource, pageNum - 1);
            try
            {
                return new iTextSharp.text.Rectangle(
                    0, 0,
                    FreeImage.GetWidth(dib) * FreeImage.GetResolutionX(dib) / 555.6344f,
                    FreeImage.GetHeight(dib) * FreeImage.GetResolutionY(dib) / 555.6344f
                    );
            }
            finally
            {
                FreeImage.UnlockPage(tifPageReader.TiffSource, dib, false);

            }
        }
    }

}
