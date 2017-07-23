using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Topz.ArmV6Z
{
    /// <summary>
    /// Provides tests for the <see cref="AssemblerTests.Assemble_Procedure_CodeAssembled(string, byte[])"/> method.
    /// </summary>
    internal sealed class ProcedureProvider : IEnumerable<TestCaseData>
    {
        /// <summary>
        /// Generates test cases for the <see cref="AssemblerTests.Assemble_Procedure_CodeAssembled(string, byte[])"/> method.
        /// </summary>
        /// <returns>Test cases for the <see cref="AssemblerTests.Assemble_Procedure_CodeAssembled(string, byte[])"/> method.</returns>
        public IEnumerator<TestCaseData> GetEnumerator()
        {
            var tests = new Dictionary<string, byte[]>()
            {
                { "add r10, r10, #21",       new byte[] { 0xE2, 0x8A, 0xA0, 0x15 } },
                { "add r10, r10, r10",       new byte[] { 0xE0, 0x8A, 0xA0, 0x0A } },
                { "add r0, r1, r2, lsl #2",  new byte[] { 0xE0, 0x81, 0x01, 0x02 } },
                { "add r0, r1, r2, lsl r4",  new byte[] { 0xE0, 0x81, 0x04, 0x12 } },
                { "add r0, r1, r2, lsr #2",  new byte[] { 0xE0, 0x81, 0x01, 0x22 } },
                { "add r0, r1, r2, lsr r4",  new byte[] { 0xE0, 0x81, 0x04, 0x32 } },
                { "add r0, r1, r2, asr #2",  new byte[] { 0xE0, 0x81, 0x01, 0x42 } },
                { "add r0, r1, r2, asr r4",  new byte[] { 0xE0, 0x81, 0x04, 0x52 } },
                { "add r0, r1, r2, ror #2",  new byte[] { 0xE0, 0x81, 0x01, 0x62 } },
                { "add r0, r1, r2, ror r4",  new byte[] { 0xE0, 0x81, 0x04, 0x72 } },
                { "add r0, r1, r2, rrx",     new byte[] { 0xE0, 0x81, 0x00, 0x62 } },

                { "and r5, r6, #432",        new byte[] { 0xE2, 0x06, 0x5E, 0x1B } },
            };

            foreach (var test in tests)
            {
                yield return new TestCaseData(test.Key, test.Value)
                {
                    TestName = $"Assemble_Procedure_CodeAssembled(\"{test.Key}\", {test.Value.ToHex()})"
                };
            }
        }

        /// <summary>
        /// Generates test cases for the <see cref="Assembler.Assemble(string)"/> method.
        /// </summary>
        /// <returns>Test cases for the <see cref="Assembler.Assemble(string)"/> method.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}