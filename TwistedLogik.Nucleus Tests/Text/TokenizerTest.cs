﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using TwistedLogik.Nucleus.Testing;
using TwistedLogik.Nucleus.Text;

namespace TwistedLogik.Nucleus.Tests.IO
{
    [TestFixture]
    public class TokenizerTest : NucleusTestFramework
    {
        [Test]
        public void Tokenizer_ShouldTokenizeString()
        {
            var tokens = new List<String>();
            var input = "Hello, world!  This is a test of the \"TwistedLogik Nucleus String Tokenizer.\"";

            input.Tokenize(tokens);

            TheResultingCollection(tokens)
                .ShouldBeExactly("Hello,", "world!", "This", "is", "a", "test", "of", "the", "TwistedLogik Nucleus String Tokenizer.");
        }

        [Test]
        public void Tokenizer_ShouldReturnLeftoverStringWhenTokenCountIsConstrained()
        {
            var remainder = String.Empty;
            var tokens = new List<String>();
            var input = "/cmd arg1 arg2 the rest is leftover";

            input.Tokenize(tokens, 3, out remainder);

            TheResultingCollection(tokens)
                .ShouldBeExactly("/cmd", "arg1", "arg2");

            TheResultingString(remainder)
                .ShouldBe("the rest is leftover");
        }
    }
}
