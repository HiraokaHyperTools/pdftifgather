using FreeImageAPI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public class PDFWriter : IRangesWriter, IDisposable
    {
        private readonly List<PdfReader> _includedReaders = new List<PdfReader>();

        private Document _document;
        private PdfCopy _copy;
        private FileStream _fs;

        public PDFWriter(String fppdf)
        {
            _fs = File.Create(fppdf);
        }

        public void AddRanges(IPageReader reader, IEnumerable<Range> ranges)
        {
            PDFPageReader pdfPageReader = reader as PDFPageReader;
            TIFPageReader tifPageReader = reader as TIFPageReader;

            foreach (var range in ranges)
            {
                for (int y = range.First; y <= range.Last && y <= reader.NumberOfPages; y++)
                {
                    if (_document == null)
                    {
                        if (pdfPageReader != null)
                        {
                            _document = new Document(pdfPageReader.Reader.GetPageSizeWithRotation(y));
                        }
                        else if (tifPageReader != null)
                        {
                            _document = new Document(TIFInterop.GetPageSizeWithRotation(tifPageReader, y));
                        }
                        else
                        {
                            throw new NotSupportedException("未対応 " + reader);
                        }
                        _copy = new PdfCopy(_document, _fs);
                        _document.Open();
                    }

                    if (pdfPageReader != null)
                    {
                        // http://stackoverflow.com/a/6155962
                        int rot = pdfPageReader.Reader.GetPageRotation(y);
                        var pageDict = pdfPageReader.Reader.GetPageN(y);
                        pageDict.Put(PdfName.ROTATE, new PdfNumber((rot + range.Angle) % 360));
                        PdfImportedPage page = _copy.GetImportedPage(pdfPageReader.Reader, y);
                        _copy.AddPage(page);
                    }
                    else if (tifPageReader != null)
                    {
                        FIBITMAP dib = FreeImage.LockPage(tifPageReader.TiffSource, y - 1);
                        using (Bitmap picWin = FreeImage.GetBitmap(dib))
                        {
                            iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(picWin, System.Drawing.Imaging.ImageFormat.Png);
                            try
                            {
                                Document localDoc = new Document();
                                MemoryStream localStream = new MemoryStream();
                                PdfWriter localWriter = PdfWriter.GetInstance(localDoc, new NoclosePassthru(localStream));

                                var pdfPageSize = _document.PageSize;

                                localDoc.SetPageSize(pdfPageSize); // A3横
                                localDoc.SetMargins(0, 0, 0, 0);
                                localDoc.Open();

                                pic.ScaleAbsolute(pdfPageSize.Width, pdfPageSize.Height);
                                localDoc.Add(pic);

                                localDoc.Close();
                                localWriter.Close();

                                localStream.Position = 0;

                                PdfReader localReader = new PdfReader(localStream);

                                var pageDict = localReader.GetPageN(y);
                                pageDict.Put(PdfName.ROTATE, new PdfNumber(range.Angle % 360));

                                PdfImportedPage page = _copy.GetImportedPage(localReader, 1);
                                _copy.AddPage(page);

                                _includedReaders.Add(localReader);
                                //localReader.Close();
                                //localStream.Close();
                            }
                            finally
                            {
                                FreeImage.UnlockPage(tifPageReader.TiffSource, dib, false);
                            }
                        }
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
            if (_copy != null) _copy.Close();
            if (_document != null) _document.Close();
            if (_fs != null) _fs.Close();
        }
    }
}
