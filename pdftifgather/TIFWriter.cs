using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace pdftifgather {
    class TIFPageReader : IPageReader, IDisposable {
        public FIMULTIBITMAP tifIn { get; set; }

        public TIFPageReader(String fpin) {
            tifIn = FreeImage.OpenMultiBitmapEx(fpin, false, true, true);
        }

        public int NumberOfPages {
            get { return FreeImage.GetPageCount(tifIn); }
        }

        public void Dispose() {
            var tifIn = this.tifIn;
            FreeImage.CloseMultiBitmapEx(ref tifIn);
            this.tifIn = tifIn;
        }
    }

    class TIFWriter : IRangesWriter, IDisposable {
        FIMULTIBITMAP tifOut;

        public TIFWriter(String fpout) {
            tifOut = FreeImage.OpenMultiBitmap(FREE_IMAGE_FORMAT.FIF_TIFF, fpout, true, false, false, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
        }

        public void AddRanges(IPageReader reader, IEnumerable<Range> ranges) {
            TIFPageReader tifPageReader = reader as TIFPageReader;
            PDFPageReader pdfPageReader = reader as PDFPageReader;
            foreach (var range in ranges) {
                for (int y = range.first; y <= range.last && y <= reader.NumberOfPages; y++) {
                    if (tifPageReader != null) {
                        var dib = FreeImage.LockPage(tifPageReader.tifIn, y - 1);
                        try {
                            FreeImage.AppendPage(tifOut, dib);
                        }
                        finally {
                            FreeImage.UnlockPage(tifPageReader.tifIn, dib, false);
                        }
                    }
                    else if (pdfPageReader != null) {
                        String fpTmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                        ProcessStartInfo psi = new ProcessStartInfo(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pdftoppm.exe"),
                            String.Format(" -f {0} -l {0} -singlefile -png \"{1}\" \"{2}\" "
                                , y
                                , pdfPageReader.fpin
                                , fpTmp
                                )
                            );
                        psi.UseShellExecute = false;
                        psi.RedirectStandardInput = true;
                        psi.RedirectStandardOutput = true;
                        Process p = Process.Start(psi);
                        p.WaitForExit();
                        if (p.ExitCode != 0) {
                            throw new ApplicationException("変換に失敗(" + p.ExitCode + ") " + pdfPageReader.fpin);
                        }

                        var dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, fpTmp + ".png", FREE_IMAGE_LOAD_FLAGS.DEFAULT);
                        try {
                            FreeImage.AppendPage(tifOut, dib);
                        }
                        finally {
                            FreeImage.UnloadEx(ref dib);
                        }

                        File.Delete(fpTmp + ".png");
                    }
                    else {
                        throw new NotSupportedException("未対応 " + reader);
                    }
                }
            }
        }

        public void Dispose() {
            FreeImage.CloseMultiBitmapEx(ref tifOut, FREE_IMAGE_SAVE_FLAGS.TIFF_LZW | FREE_IMAGE_SAVE_FLAGS.TIFF_CCITTFAX3);
        }
    }
}
