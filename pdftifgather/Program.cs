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
                else if (fpout == "/v0.4") {
                    new Program().Run(args, 1);
                }
                else {
                    new Program().Run(args, 0);
                }
            }
            catch (Exception err) {
                Console.Error.WriteLine("" + err);
                Environment.ExitCode = 2;
            }
        }

        void Run(string[] args, int nextArg) {
            string fpout = args[nextArg];
            nextArg++;
            using (var writer = WriterFactory.Create(fpout)) {
                int cx = args.Length;
                while (nextArg < cx) {
                    if (args[nextArg] != "(") {
                        String fpinEntire = args[nextArg];
                        writer.AddRanges(ReaderFactory.Create(fpinEntire), new Range[] { new Range { first = 1, last = int.MaxValue } });
                        nextArg++;
                        continue;
                    }
                    nextArg++;
                    String fpin = args[nextArg];
                    nextArg++;
                    List<Range> ranges = new List<Range>();
                    while (args[nextArg] != ")") {
                        int pi;
                        Match M;
                        Range range;
                        if (false) { }
                        else if ((M = Regex.Match(args[nextArg], "^((?<p>\\d+)|(?<a>\\d+)-|-(?<b>\\d+)|(?<a>\\d+)-(?<b>\\d+))(?<c>l|r|d|left|right|down)?$", RegexOptions.IgnoreCase)).Success) {
                            if (M.Groups["p"].Success) {
                                // 単一
                                range = new Range {
                                    first = Convert.ToInt32(M.Groups["p"].Value),
                                    last = Convert.ToInt32(M.Groups["p"].Value),
                                    rot = M.Groups["c"].Value,
                                };
                            }
                            else {
                                // 複数
                                range = new Range {
                                    first = M.Groups["a"].Success ? Convert.ToInt32(M.Groups["a"].Value) : 1,
                                    last = M.Groups["b"].Success ? Convert.ToInt32(M.Groups["b"].Value) : int.MaxValue,
                                    rot = M.Groups["c"].Value,
                                };
                            }
                        }
                        else if (int.TryParse(args[nextArg], out pi)) {
                            range = new Range {
                                first = pi,
                                last = pi,
                                rot = "",
                            };
                        }
                        else {
                            nextArg++;
                            continue;
                        }

                        ranges.Add(range);
                        nextArg++;
                    }

                    writer.AddRanges(ReaderFactory.Create(fpin), ranges);

                    nextArg++; // ")"
                }
            }
        }

        private static void helpYa() {
            Console.Error.WriteLine("pdftifgather out.pdf in1.pdf in2.pdf in3.pdf");
            Console.Error.WriteLine("pdftifgather out.pdf ( in-even.pdf 2 4 6 ) ( in-odd.pdf 1 3 5 )");
            Console.Error.WriteLine("pdftifgather out.tif ( in-even.tif 2 4 6 ) ( in-odd.tif 1 3 5 )");
            Console.Error.WriteLine("pdftifgather       out.tif ( in.tif 1L 1Left 1R 1Right 1D 1Down )");
            Console.Error.WriteLine("pdftifgather /v0.4 out.tif ( in.tif 1L 1Left 1R 1Right 1D 1Down )");
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

        /// <summary>
        /// l,r,d
        /// </summary>
        public string rot { get; set; }

        /// <summary>
        /// 右回転の度数
        /// 0,90,180,270 のいずれか
        /// </summary>
        public int rotAngle {
            get {
                switch (char.ToLowerInvariant((rot + " ")[0])) {
                    case 'l': return 270;
                    case 'r': return 90;
                    case 'd': return 180;
                }
                return 0;
            }
        }
    }

    public interface IRangesWriter : IDisposable {
        void AddRanges(IPageReader reader, IEnumerable<Range> ranges);
    }

    public interface IPageReader {
        int NumberOfPages { get; }
    }
}
