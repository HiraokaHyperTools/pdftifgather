using NUnit.Framework;
using pdftifgather.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.tests
{
    public class RangeParserTest
    {
        [Test]
        [TestCase("1", 1, 1, 0)]
        [TestCase("2-3", 2, 3, 0)]
        [TestCase("-4", 1, 4, 0)]
        [TestCase("5-", 5, int.MaxValue, 0)]
        public void Ranges(string input, int first, int last, int angle)
        {
            var it = new RangeParser(input);
            Assert.True(it.Valid);
            Assert.That(it.Range.First, Is.EqualTo(first));
            Assert.That(it.Range.Last, Is.EqualTo(last));
            Assert.That(it.Range.Angle, Is.EqualTo(angle));

        }
    }
}
