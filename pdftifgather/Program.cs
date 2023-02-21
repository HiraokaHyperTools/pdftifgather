using pdftifgather.Helpers;
using pdftifgather.Usecases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pdftifgather
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                HelpYa();
                return 1;
            }
            try
            {
                var gatheringUsecase = new GatheringUsecase(
                    new WriterFactory(),
                    new ReaderFactory()
                );

                String first = args[0];
                if (first == "/GPC" && args.Length >= 2)
                {
                    try
                    {
                        return new ReaderFactory().Create(args[1]).NumberOfPages;
                    }
                    catch (Exception err)
                    {
                        Console.Error.WriteLine("" + err);
                        return -1;
                    }
                }
                else if (first == "/v0.4")
                {
                    var outputPath = args[1];
                    var parsed = new SourceSpecParser(args.Skip(2));
                    if (parsed.Valid)
                    {
                        gatheringUsecase.Gather(outputPath, parsed.Specs);
                        return 0;
                    }
                    else
                    {
                        HelpYa();
                        return 1;
                    }
                }
                else
                {
                    var outputPath = args[0];
                    var parsed = new SourceSpecParser(args.Skip(1));
                    if (parsed.Valid)
                    {
                        gatheringUsecase.Gather(outputPath, parsed.Specs);
                        return 0;
                    }
                    else
                    {
                        HelpYa();
                        return 1;
                    }
                }
            }
            catch (Exception err)
            {
                Console.Error.WriteLine("" + err);
                return 2;
            }
        }

        private static void HelpYa()
        {
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
    }
}
