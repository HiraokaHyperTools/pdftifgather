using FreeImageAPI.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pdftifgather.Helpers
{
    public class RangeParser
    {
        public bool Valid { get; set; }
        public Range Range { get; set; }

        public RangeParser(string spec)
        {
            Match match;
            if (false) { }
            else if ((match = Regex.Match(spec, "^((?<p>\\d+)|(?<a>\\d+)-|-(?<b>\\d+)|(?<a>\\d+)-(?<b>\\d+))(?<c>l|r|d|left|right|down)?$", RegexOptions.IgnoreCase)).Success)
            {
                if (match.Groups["p"].Success)
                {
                    // 単一
                    Range = (
                        new Range
                        {
                            First = Convert.ToInt32(match.Groups["p"].Value),
                            Last = Convert.ToInt32(match.Groups["p"].Value),
                            Rot = match.Groups["c"].Value,
                        }
                    );
                }
                else
                {
                    // 複数
                    Range = (
                        new Range
                        {
                            First = match.Groups["a"].Success ? Convert.ToInt32(match.Groups["a"].Value) : 1,
                            Last = match.Groups["b"].Success ? Convert.ToInt32(match.Groups["b"].Value) : int.MaxValue,
                            Rot = match.Groups["c"].Value,
                        }
                    );
                }
            }
            else if (int.TryParse(spec, out int pageNum))
            {
                Range = (
                    new Range
                    {
                        First = pageNum,
                        Last = pageNum,
                        Rot = "",
                    }
                );
            }
            else
            {
                return;
            }

            Valid = true;
        }
    }
}
