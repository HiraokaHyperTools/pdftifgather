using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pdftifgather {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 2) {
                helpYa(); Environment.ExitCode = 1;
                return;
            }
            try {
                String fpout = args[0];
                if (fpout == "/GPC" && args.Length >= 2) {
                    try {
                        Environment.ExitCode = ReaderFactory.Create(args[1]).NumberOfPages;
                    }
                    catch (Exception err) {
                        Console.Error.WriteLine("" + err);
                        Environment.ExitCode = -1;
                    }
                    return;
                }
                using (var writer = WriterFactory.Create(fpout)) {
                    int x = 1, cx = args.Length;
                    while (x < cx) {
                        if (args[x] != "(") {
                            String fpinEntire = args[x];
                            writer.AddRanges(ReaderFactory.Create(fpinEntire), new Range[] { new Range { first = 1, last = int.MaxValue } });
                            x++;
                            continue;
                        }
                        x++;
                        String fpin = args[x];
                        x++;
                        List<Range> ranges = new List<Range>();
                        while (args[x] != ")") {
                            int pi;
                            Match M;
                            Range range;
                            if (false) { }
                            else if ((M = Regex.Match(args[x], "^(?<a>\\d+)\\-(?<b>\\d+)$")).Success) {
                                range = new Range {
                                    first = Convert.ToInt32(M.Groups["a"].Value),
                                    last = Convert.ToInt32(M.Groups["b"].Value),
                                };
                            }
                            else if ((M = Regex.Match(args[x], "^(?<a>\\d+)\\-$")).Success) {
                                range = new Range {
                                    first = Convert.ToInt32(M.Groups["a"].Value),
                                    last = int.MaxValue,
                                };
                            }
                            else if ((M = Regex.Match(args[x], "^\\-(?<b>\\d+)$")).Success) {
                                range = new Range {
                                    first = 1,
                                    last = Convert.ToInt32(M.Groups["b"].Value),
                                };
                            }
                            else if (int.TryParse(args[x], out pi)) {
                                range = new Range {
                                    first = pi,
                                    last = pi,
                                };
                            }
                            else {
                                x++;
                                continue;
                            }

                            ranges.Add(range);
                            x++;
                        }

                        writer.AddRanges(ReaderFactory.Create(fpin), ranges);

                        x++; // ")"
                    }
                }
            }
            catch (Exception err) {
                Console.Error.WriteLine("" + err);
                Environment.ExitCode = 2;
            }
        }

        private static void helpYa() {
            Console.Error.WriteLine("pdftifgather out.pdf in1.pdf in2.pdf in3.pdf");
            Console.Error.WriteLine("pdftifgather out.pdf ( in-even.pdf 2 4 6 ) ( in-odd.pdf 1 3 5 )");
            Console.Error.WriteLine("pdftifgather out.tif ( in-even.tif 2 4 6 ) ( in-odd.tif 1 3 5 )");
            Console.Error.WriteLine("pdftifgather out.tif ( in.tif 1- )");
            Console.Error.WriteLine("pdftifgather out.tif ( in.tif 2-3 )");
            Console.Error.WriteLine("pdftifgather out.tif ( in.tif 4- )");
            Console.Error.WriteLine("pdftifgather /GPC in.pdf ");
            Console.Error.WriteLine("pdftifgather /GPC in.tif ");
        }

        class ReaderFactory {
            internal static IPageReader Create(string fpin) {
                var ext = Path.GetExtension(fpin);
                if ("|.pdf|".IndexOf(ext) >= 0) {
                    return new PDFPageReader(fpin);
                }
                else if ("|.tif|.tiff|".IndexOf(ext) >= 0) {
                    return new TIFPageReader(fpin);
                }
                else {
                    throw new ApplicationException(ext + " 入力は未知です。");
                }
            }
        }

        class WriterFactory {
            internal static IRangesWriter Create(string fpout) {
                var ext = Path.GetExtension(fpout);
                if ("|.pdf|".IndexOf(ext) >= 0) {
                    return new PDFWriter(fpout);
                }
                else if ("|.tif|.tiff|".IndexOf(ext) >= 0) {
                    return new TIFWriter(fpout);
                }
                else {
                    throw new ApplicationException(ext + " 出力は未知です。");
                }
            }
        }
    }

    public class Range {
        /// <summary>
        /// 1-
        /// </summary>
        public int first { get; set; }

        /// <summary>
        /// 1- ... int.MaxValue
        /// </summary>
        public int last { get; set; }
    }

    public interface IRangesWriter : IDisposable {
        void AddRanges(IPageReader reader, IEnumerable<Range> ranges);
    }

    public interface IPageReader {
        int NumberOfPages { get; }
    }
}
