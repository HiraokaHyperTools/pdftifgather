using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public class TIFWriter : IRangesWriter, IDisposable
    {
        private FIMULTIBITMAP _tiffDest;

        public TIFWriter(String fpout)
        {
            _tiffDest = FreeImage.OpenMultiBitmap(FREE_IMAGE_FORMAT.FIF_TIFF, fpout, true, false, false, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
        }

        public void AddRanges(IPageReader reader, IEnumerable<Range> ranges)
        {
            TIFPageReader tifPageReader = reader as TIFPageReader;
            PDFPageReader pdfPageReader = reader as PDFPageReader;
            foreach (var range in ranges)
            {
                for (int y = range.First; y <= range.Last && y <= reader.NumberOfPages; y++)
                {
                    if (tifPageReader != null)
                    {
                        var dib = FreeImage.LockPage(tifPageReader.TiffSource, y - 1);
                        var dibRot = FreeImage.Rotate(dib, -range.Angle);
                        try
                        {
                            FreeImage.AppendPage(_tiffDest, dibRot);
                        }
                        finally
                        {
                            FreeImage.UnlockPage(tifPageReader.TiffSource, dib, false);
                        }
                    }
                    else if (pdfPageReader != null)
                    {
                        String fpTmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                        ProcessStartInfo psi = new ProcessStartInfo(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pdftoppm.exe"),
                            String.Format(" -f {0} -l {0} -singlefile -png \"{1}\" \"{2}\" "
                                , y
                                , pdfPageReader.InputPath
                                , fpTmp
                                )
                            );
                        psi.UseShellExecute = false;
                        psi.RedirectStandardInput = true;
                        psi.RedirectStandardOutput = true;
                        Process p = Process.Start(psi);
                        p.WaitForExit();
                        if (p.ExitCode != 0)
                        {
                            throw new Exception("変換に失敗(" + p.ExitCode + ") " + pdfPageReader.InputPath);
                        }

                        var dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, fpTmp + ".png", FREE_IMAGE_LOAD_FLAGS.DEFAULT);
                        var dibRot = FreeImage.Rotate(dib, -range.Angle);
                        try
                        {
                            FreeImage.AppendPage(_tiffDest, dibRot);
                        }
                        finally
                        {
                            FreeImage.UnloadEx(ref dib);
                        }

                        File.Delete(fpTmp + ".png");
                    }
                    else
                    {
                        throw new NotSupportedException("未対応 " + reader);
                    }
                }
            }
        }

        public void Dispose()
        {
            FreeImage.CloseMultiBitmapEx(ref _tiffDest, FREE_IMAGE_SAVE_FLAGS.TIFF_LZW | FREE_IMAGE_SAVE_FLAGS.TIFF_CCITTFAX3);
        }
    }
}
