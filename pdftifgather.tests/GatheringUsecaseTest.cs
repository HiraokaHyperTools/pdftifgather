using NUnit.Framework;
using pdftifgather.Helpers;
using pdftifgather.Usecases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pdftifgather.tests
{
    public class GatheringUsecaseTest
    {
        private readonly GatheringUsecase _gatheringUsecase;

        public GatheringUsecaseTest()
        {
            _gatheringUsecase = new GatheringUsecase(
                new WriterFactory(),
                new ReaderFactory()
            );
        }

        [Test]
        [TestCase(@".\M.pdf ( %SAMPLES%\Samples.pdf -1 2-2r 3d 4-l )")]
        [TestCase(@".\0-t.tif ( %SAMPLES%\Sample.tif 1 )")]
        [TestCase(@".\90-t.tif ( %SAMPLES%\Sample.tif 1right )")]
        [TestCase(@".\180-t.tif ( %SAMPLES%\Sample.tif 1down )")]
        [TestCase(@".\270-t.tif ( %SAMPLES%\Sample.tif 1left )")]
        [TestCase(@".\0-p.tif ( %SAMPLES%\Sample.pdf 1 )")]
        [TestCase(@".\90-p.tif ( %SAMPLES%\Sample.pdf 1r )")]
        [TestCase(@".\180-p.tif ( %SAMPLES%\Sample.pdf 1d )")]
        [TestCase(@".\270-p.tif ( %SAMPLES%\Sample.pdf 1l )")]
        [TestCase(@".\0-t.pdf ( %SAMPLES%\Sample.tif 1 )")]
        [TestCase(@".\90-t.pdf ( %SAMPLES%\Sample.tif 1r )")]
        [TestCase(@".\180-t.pdf ( %SAMPLES%\Sample.tif 1d )")]
        [TestCase(@".\270-t.pdf ( %SAMPLES%\Sample.tif 1l )")]
        [TestCase(@".\0-p.pdf ( %SAMPLES%\Sample.pdf 1 )")]
        [TestCase(@".\90-p.pdf ( %SAMPLES%\Sample.pdf 1r )")]
        [TestCase(@".\180-p.pdf ( %SAMPLES%\Sample.pdf 1d )")]
        [TestCase(@".\270-p.pdf ( %SAMPLES%\Sample.pdf 1l )")]
        public void Gather(string line)
        {
            Environment.SetEnvironmentVariable("SAMPLES", SamplesResolver.Resolve("."));

            var args = Regex.Split(line.Trim(), "\\s+")
                .Select(Environment.ExpandEnvironmentVariables)
                .ToArray();

            var outputPath = Path.GetFullPath(args[0]);
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            var parsed = new SourceSpecParser(args.Skip(1));
            Assert.True(parsed.Valid);
            _gatheringUsecase.Gather(outputPath, parsed.Specs);
            FileAssert.Exists(outputPath);
        }

        [TestCase(@".\error1.pdf %SAMPLES%\notfound.pdf")]
        [TestCase(@".\error2.pdf %SAMPLES%\notfound.tif")]
        [TestCase(@".\error3.tif %SAMPLES%\notfound.pdf")]
        [TestCase(@".\error4.tif %SAMPLES%\notfound.tif")]
        [TestCase(@".\error5.pdf %SAMPLES%\Sample.pdf %SAMPLES%\notfound.pdf")]
        [TestCase(@".\error6.pdf %SAMPLES%\Sample.tif %SAMPLES%\notfound.tif")]
        [TestCase(@".\error7.tif %SAMPLES%\Sample.pdf %SAMPLES%\notfound.pdf")]
        [TestCase(@".\error8.tif %SAMPLES%\Sample.tif %SAMPLES%\notfound.tif")]
        public void Error(string line)
        {
            Environment.SetEnvironmentVariable("SAMPLES", SamplesResolver.Resolve("."));

            var args = Regex.Split(line.Trim(), "\\s+")
                .Select(Environment.ExpandEnvironmentVariables)
                .ToArray();

            var outputPath = Path.GetFullPath(args[0]);
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            var parsed = new SourceSpecParser(args.Skip(1));
            Assert.True(parsed.Valid);
            try
            {
                _gatheringUsecase.Gather(outputPath, parsed.Specs);
                Assert.Fail();
            }
            catch (IOException)
            {
                // ok
            }
            FileAssert.DoesNotExist(outputPath);
        }
    }
}
