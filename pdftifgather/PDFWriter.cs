using FreeImageAPI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace pdftifgather {
    class PDFPageReader : IPageReader, IDisposable {
        public PdfReader reader { get; set; }
        public String fpin { get; set; }

        public PDFPageReader(String fpin) {
            this.fpin = fpin;
            reader = new PdfReader(fpin);
            reader.ConsolidateNamedDestinations();
        }

        public int NumberOfPages {
            get { return reader.NumberOfPages; }
        }

        public void Dispose() {
            reader.Close();
        }
    }

    class PDFWriter : IRangesWriter, IDisposable {
        Document document;
        PdfCopy copy;
        FileStream fs;
        List<PdfReader> includedReaders = new List<PdfReader>();

        public PDFWriter(String fppdf) {
            this.fs = File.Create(fppdf);
        }

        public void AddRanges(IPageReader reader, IEnumerable<Range> ranges) {
            PDFPageReader pdfPageReader = reader as PDFPageReader;
            TIFPageReader tifPageReader = reader as TIFPageReader;

            foreach (var range in ranges) {
                for (int y = range.first; y <= range.last && y <= reader.NumberOfPages; y++) {
                    if (document == null) {
                        if (pdfPageReader != null) {
                            document = new Document(pdfPageReader.reader.GetPageSizeWithRotation(y));
                        }
                        else if (tifPageReader != null) {
                            document = new Document(TIFInterop.GetPageSizeWithRotation(tifPageReader, y));
                        }
                        else {
                            throw new NotSupportedException("未対応 " + reader);
                        }
                        copy = new PdfCopy(document, fs);
                        document.Open();
                    }

                    if (pdfPageReader != null) {
                        // http://stackoverflow.com/a/6155962
                        int rot = pdfPageReader.reader.GetPageRotation(y);
                        var pageDict = pdfPageReader.reader.GetPageN(y);
                        pageDict.Put(PdfName.ROTATE, new PdfNumber((rot + range.rotAngle) % 360));
                        PdfImportedPage page = copy.GetImportedPage(pdfPageReader.reader, y);
                        copy.AddPage(page);
                    }
                    else if (tifPageReader != null) {
                        FIBITMAP dib = FreeImage.LockPage(tifPageReader.tifIn, y - 1);
                        using (Bitmap picWin = FreeImage.GetBitmap(dib)) {
                            iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(picWin, System.Drawing.Imaging.ImageFormat.Png);
                            try {
                                Document localDoc = new Document();
                                MemoryStream localStream = new MemoryStream();
                                PdfWriter localWriter = PdfWriter.GetInstance(localDoc, new NoclosePassthru(localStream));

                                var pdfPageSize = document.PageSize;

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
                                pageDict.Put(PdfName.ROTATE, new PdfNumber(range.rotAngle % 360));
                                
                                PdfImportedPage page = copy.GetImportedPage(localReader, 1);
                                copy.AddPage(page);

                                includedReaders.Add(localReader);
                                //localReader.Close();
                                //localStream.Close();
                            }
                            finally {
                                FreeImage.UnlockPage(tifPageReader.tifIn, dib, false);
                            }
                        }
                    }
                    else {
                        throw new NotSupportedException("未対応 " + reader);
                    }
                }
            }
        }

        class NoclosePassthru : Stream {
            public NoclosePassthru(Stream baseStream) {
                this.baseStream = baseStream;
            }

            Stream baseStream;

            public override bool CanRead {
                get { return baseStream.CanRead; }
            }

            public override bool CanSeek {
                get { return baseStream.CanSeek; }
            }

            public override bool CanWrite {
                get { return baseStream.CanWrite; }
            }

            public override void Flush() {
                baseStream.Flush();
            }

            public override long Length {
                get { return baseStream.Length; }
            }

            public override long Position {
                get {
                    return baseStream.Position;
                }
                set {
                    baseStream.Position = value;
                }
            }

            public override int Read(byte[] buffer, int offset, int count) {
                return baseStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin) {
                return baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value) {
                baseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count) {
                baseStream.Write(buffer, offset, count);
            }
        }

        public void Dispose() {
            if (copy != null) copy.Close();
            if (document != null) document.Close();
            if (fs != null) fs.Close();
        }

        class TIFInterop {
            internal static iTextSharp.text.Rectangle GetPageSizeWithRotation(TIFPageReader tifPageReader, int y) {
                var dib = FreeImage.LockPage(tifPageReader.tifIn, y - 1);
                try {
                    return new iTextSharp.text.Rectangle(
                        0, 0,
                        FreeImage.GetWidth(dib) * FreeImage.GetResolutionX(dib) / 555.6344f,
                        FreeImage.GetHeight(dib) * FreeImage.GetResolutionY(dib) / 555.6344f
                        );
                }
                finally {
                    FreeImage.UnlockPage(tifPageReader.tifIn, dib, false);

                }
            }
        }
    }
}
