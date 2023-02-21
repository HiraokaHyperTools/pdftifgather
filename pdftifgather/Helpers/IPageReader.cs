using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public interface IPageReader
    {
        int NumberOfPages { get; }
    }
}
