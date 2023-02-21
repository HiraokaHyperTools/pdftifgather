using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public class PDFPageReader : IPageReader, IDisposable
    {
        public PdfReader Reader { get; set; }
        public string InputPath { get; set; }

        public PDFPageReader(String inputPath)
        {
            InputPath = inputPath;
            Reader = new PdfReader(inputPath);
            Reader.ConsolidateNamedDestinations();
        }

        public int NumberOfPages
        {
            get { return Reader.NumberOfPages; }
        }

        public void Dispose()
        {
            Reader.Close();
        }
    }

}
