using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public class WriterFactory
    {
        public class Entry
        {
            public string Extension { get; set; }
            public Func<string, IRangesWriter> Factory { get; set; }
        }

        public List<Entry> Entries { get; set; } = new List<Entry>();

        public WriterFactory()
        {
            Entries.Add(new Entry { Extension = ".pdf", Factory = inputPath => new PDFWriter(inputPath) });
            Entries.Add(new Entry { Extension = ".tif", Factory = inputPath => new TIFWriter(inputPath) });
            Entries.Add(new Entry { Extension = ".tiff", Factory = inputPath => new TIFWriter(inputPath) });
        }

        public IRangesWriter Create(string outputPath)
        {
            var extension = Path.GetExtension(outputPath);

            var found = Entries
                .FirstOrDefault(it => StringComparer.InvariantCultureIgnoreCase.Compare(it.Extension, extension) == 0);

            if (found != null)
            {
                return found.Factory(outputPath);
            }
            else
            {
                throw new Exception(extension + " 出力は未知です。");
            }
        }
    }

}
