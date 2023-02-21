using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.Helpers
{
    public class SourceSpec
    {
        public string InputPath { get; set; }
        public bool Entire { get; set; }
        public List<Range> Range { get; set; } = new List<Range>();
    }
}
