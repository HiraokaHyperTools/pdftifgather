using NUnit.Framework;
using pdftifgather.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdftifgather.tests
{
    public class SourceSpecParserTest
    {
        [Test]
        public void Some()
        {
            {
                var it = new SourceSpecParser(new string[] { "in1.pdf", "in2.pdf", "in3.pdf" });
                Assert.True(it.Valid);
                Assert.That(it.Specs.Count, Is.EqualTo(3));
                Assert.That(it.Specs[0].InputPath, Is.EqualTo("in1.pdf"));
                Assert.True(it.Specs[0].Entire);
                Assert.That(it.Specs[1].InputPath, Is.EqualTo("in2.pdf"));
                Assert.True(it.Specs[1].Entire);
                Assert.That(it.Specs[2].InputPath, Is.EqualTo("in3.pdf"));
                Assert.True(it.Specs[2].Entire);
            }

            {
                var it = new SourceSpecParser(new string[] { "(", "in-even.pdf", "2", "4", "6", ")", "(", "in-odd.pdf", "1", "3", "5", ")" });
                Assert.True(it.Valid);
                Assert.That(it.Specs.Count, Is.EqualTo(2));
                Assert.That(it.Specs[0].InputPath, Is.EqualTo("in-even.pdf"));
                Assert.False(it.Specs[0].Entire);
                Assert.That(it.Specs[0].Range.Count, Is.EqualTo(3));
                Assert.That(it.Specs[0].Range[0].First, Is.EqualTo(2));
                Assert.That(it.Specs[0].Range[0].Last, Is.EqualTo(2));
                Assert.That(it.Specs[0].Range[0].Angle, Is.EqualTo(0));
                Assert.That(it.Specs[0].Range[1].First, Is.EqualTo(4));
                Assert.That(it.Specs[0].Range[1].Last, Is.EqualTo(4));
                Assert.That(it.Specs[0].Range[1].Angle, Is.EqualTo(0));
                Assert.That(it.Specs[0].Range[2].First, Is.EqualTo(6));
                Assert.That(it.Specs[0].Range[2].Last, Is.EqualTo(6));
                Assert.That(it.Specs[0].Range[2].Angle, Is.EqualTo(0));
                Assert.That(it.Specs[1].InputPath, Is.EqualTo("in-odd.pdf"));
                Assert.False(it.Specs[1].Entire);
                Assert.That(it.Specs[1].Range.Count, Is.EqualTo(3));
                Assert.That(it.Specs[1].Range[0].First, Is.EqualTo(1));
                Assert.That(it.Specs[1].Range[0].Last, Is.EqualTo(1));
                Assert.That(it.Specs[1].Range[0].Angle, Is.EqualTo(0));
                Assert.That(it.Specs[1].Range[1].First, Is.EqualTo(3));
                Assert.That(it.Specs[1].Range[1].Last, Is.EqualTo(3));
                Assert.That(it.Specs[1].Range[1].Angle, Is.EqualTo(0));
                Assert.That(it.Specs[1].Range[2].First, Is.EqualTo(5));
                Assert.That(it.Specs[1].Range[2].Last, Is.EqualTo(5));
                Assert.That(it.Specs[1].Range[2].Angle, Is.EqualTo(0));
            }

            {
                var it = new SourceSpecParser(new string[] { "(", "in.tif", "1L", "1Left", "1R", "1Right", "1D", "1Down", ")" });
                Assert.True(it.Valid);
                Assert.That(it.Specs.Count, Is.EqualTo(1));
                Assert.That(it.Specs[0].InputPath, Is.EqualTo("in.tif"));
                Assert.False(it.Specs[0].Entire);
                Assert.That(it.Specs[0].Range.Count, Is.EqualTo(6));
                Assert.That(it.Specs[0].Range[0].First, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[0].Last, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[0].Angle, Is.EqualTo(270));
                Assert.That(it.Specs[0].Range[1].First, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[1].Last, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[1].Angle, Is.EqualTo(270));
                Assert.That(it.Specs[0].Range[2].First, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[2].Last, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[2].Angle, Is.EqualTo(90));
                Assert.That(it.Specs[0].Range[3].First, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[3].Last, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[3].Angle, Is.EqualTo(90));
                Assert.That(it.Specs[0].Range[4].First, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[4].Last, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[4].Angle, Is.EqualTo(180));
                Assert.That(it.Specs[0].Range[5].First, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[5].Last, Is.EqualTo(1));
                Assert.That(it.Specs[0].Range[5].Angle, Is.EqualTo(180));
            }
        }
    }
}
