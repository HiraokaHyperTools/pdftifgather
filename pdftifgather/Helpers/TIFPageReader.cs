using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public class TIFPageReader : IPageReader, IDisposable
    {
        public FIMULTIBITMAP TiffSource { get; set; }

        public TIFPageReader(String fpin)
        {
            TiffSource = FreeImage.OpenMultiBitmapEx(fpin, false, true, true);
        }

        public int NumberOfPages
        {
            get { return FreeImage.GetPageCount(TiffSource); }
        }

        public void Dispose()
        {
            var tifIn = TiffSource;
            FreeImage.CloseMultiBitmapEx(ref tifIn);
            TiffSource = tifIn;
        }
    }

}
