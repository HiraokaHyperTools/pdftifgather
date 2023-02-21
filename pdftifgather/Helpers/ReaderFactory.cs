using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public class ReaderFactory
    {
        public class Entry
        {
            public string Extension { get; set; }
            public Func<string, IPageReader> Factory { get; set; }
        }

        public List<Entry> Entries { get; set; } = new List<Entry>();

        public ReaderFactory()
        {
            Entries.Add(new Entry { Extension = ".pdf", Factory = inputPath => new PDFPageReader(inputPath) });
            Entries.Add(new Entry { Extension = ".tif", Factory = inputPath => new TIFPageReader(inputPath) });
            Entries.Add(new Entry { Extension = ".tiff", Factory = inputPath => new TIFPageReader(inputPath) });
        }

        public IPageReader Create(string inputPath)
        {
            var extension = Path.GetExtension(inputPath);

            var found = Entries
                .FirstOrDefault(it => StringComparer.InvariantCultureIgnoreCase.Compare(it.Extension, extension) == 0);

            if (found != null)
            {
                return found.Factory(inputPath);
            }
            else
            {
                throw new Exception(extension + " 入力は未知です。");
            }
        }
    }

}
