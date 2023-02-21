using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pdftifgather.tests
{
    internal static class SamplesResolver
    {
        public static string Resolve(string path) => Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "..",
            "..",
            "..",
            "Samples",
            path
        );
    }
}
