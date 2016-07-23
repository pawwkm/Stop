using NUnit.Framework;
using Pote.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topz.ArmV6Z.Tests
{
    /// <summary>
    /// Provides tests for the <see cref="Mnemonic"/> class.
    /// </summary>
    [TestFixture]
    public class MnemonicTests
    {
        /// <summary>
        /// Tests that <see cref="Mnemonic(string, InputPosition)"/> constructor
        /// can populate <see cref="Mnemonic.Name"/>, <see cref="Mnemonic.RawName"/> and 
        /// <see cref="Mnemonic.Condition"/> correctly if accepted parameters are used.
        /// </summary>
        [Test]
        public void Mnemonic_ValidParameters_PropertiesPopulated()
        {
            var values = PreParse(Mnemonic.B);

            foreach (var value in values)
            {
                Mnemonic mnemonic = new Mnemonic(value.Name, new InputPosition());

                Assert.AreEqual(value.Name, mnemonic.Name);
                Assert.AreEqual(value.RawName, mnemonic.RawName);
                Assert.AreEqual(value.Condition, mnemonic.Condition);
            }
        }

        private static IEnumerable<PreParsedMnemonic> PreParse(string rawName)
        {
            yield return new PreParsedMnemonic(rawName, rawName, Condition.Always);
            yield return new PreParsedMnemonic(rawName + Condition.CarrySet.AsText(), rawName, Condition.CarrySet);
            yield return new PreParsedMnemonic(rawName + Condition.CarryClear.AsText(), rawName, Condition.CarryClear);
            yield return new PreParsedMnemonic(rawName + Condition.Equal.AsText(), rawName, Condition.Equal);
            yield return new PreParsedMnemonic(rawName + Condition.LessThanOrEqual.AsText(), rawName, Condition.LessThanOrEqual);
            yield return new PreParsedMnemonic(rawName + Condition.Minus.AsText(), rawName, Condition.Minus);
            yield return new PreParsedMnemonic(rawName + Condition.NoOverflow.AsText(), rawName, Condition.NoOverflow);
            yield return new PreParsedMnemonic(rawName + Condition.NotEqual.AsText(), rawName, Condition.NotEqual);
            yield return new PreParsedMnemonic(rawName + Condition.Overflow.AsText(), rawName, Condition.Overflow);
            yield return new PreParsedMnemonic(rawName + Condition.Plus.AsText(), rawName, Condition.Plus);
            yield return new PreParsedMnemonic(rawName + Condition.SignedGreaterThan.AsText(), rawName, Condition.SignedGreaterThan);
            yield return new PreParsedMnemonic(rawName + Condition.SignedGreaterThanOrEqual.AsText(), rawName, Condition.SignedGreaterThanOrEqual);
            yield return new PreParsedMnemonic(rawName + Condition.SignedLessThan.AsText(), rawName, Condition.SignedLessThan);
            yield return new PreParsedMnemonic(rawName + Condition.UnsignedHigher.AsText(), rawName, Condition.UnsignedHigher);
            yield return new PreParsedMnemonic(rawName + Condition.UnsignedLowerOrSame.AsText(), rawName, Condition.UnsignedLowerOrSame);
        }

        private class PreParsedMnemonic
        {
            public PreParsedMnemonic(string name, string rawName, Condition condition)
            {
                Name = name;
                RawName = rawName;
                Condition = condition;
            }

            public string Name
            {
                get;
                private set;
            }

            public string RawName
            {
                get;
                private set;
            }

            public Condition Condition
            {
                get;
                private set;
            }
        }
    }
}
