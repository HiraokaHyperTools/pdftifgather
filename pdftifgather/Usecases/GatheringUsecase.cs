using pdftifgather.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pdftifgather.Usecases
{
    public class GatheringUsecase
    {
        private readonly WriterFactory _writerFactory;
        private readonly ReaderFactory _readerFactory;

        public GatheringUsecase(
            WriterFactory writerFactory,
            ReaderFactory readerFactory
        )
        {
            _writerFactory = writerFactory;
            _readerFactory = readerFactory;
        }

        public void Gather(string outputPath, IEnumerable<SourceSpec> specs)
        {
            var keep = false;
            try
            {
                using (var writer = _writerFactory.Create(outputPath))
                {
                    foreach (var spec in specs)
                    {
                        if (spec.Entire)
                        {
                            writer.AddRanges(
                                _readerFactory.Create(spec.InputPath),
                                new Range[] { new Range { First = 1, Last = int.MaxValue } }
                            );
                        }
                        else
                        {
                            writer.AddRanges(
                                _readerFactory.Create(spec.InputPath),
                                spec.Range
                            );
                        }
                    }
                }
                keep = true;
            }
            finally
            {
                if (!keep)
                {
                    if (File.Exists(outputPath))
                    {
                        File.Delete(outputPath);
                    }
                }
            }
        }
    }
}
