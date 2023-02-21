using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public interface IRangesWriter : IDisposable
    {
        void AddRanges(IPageReader reader, IEnumerable<Range> ranges);
    }
}
