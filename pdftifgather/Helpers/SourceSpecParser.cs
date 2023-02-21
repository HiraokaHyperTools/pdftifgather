using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pdftifgather.Helpers
{
    public class SourceSpecParser
    {
        public List<SourceSpec> Specs { get; set; } = new List<SourceSpec>();
        public bool Valid { get; set; }

        public SourceSpecParser(IEnumerable<string> args)
        {
            var en = args.GetEnumerator();
            var any = false;
            while (en.MoveNext())
            {
                if (en.Current != "(")
                {
                    Specs.Add(
                        new SourceSpec
                        {
                            InputPath = en.Current,
                            Entire = true,
                        }
                    );
                    any = true;
                    continue;
                }
                else
                {
                    if (!en.MoveNext())
                    {
                        return;
                    }

                    var spec = new SourceSpec
                    {
                        InputPath = en.Current,
                    };

                    var ranges = spec.Range;

                    while (true)
                    {
                        if (!en.MoveNext())
                        {
                            return;
                        }
                        if (en.Current == ")")
                        {
                            Specs.Add(spec);
                            any = true;
                            break;
                        }

                        var parsed = new RangeParser(en.Current);
                        if (parsed.Valid)
                        {
                            ranges.Add(parsed.Range);
                        }
                    }
                }
            }

            if (any)
            {
                Valid = true;
            }
        }
    }
}
