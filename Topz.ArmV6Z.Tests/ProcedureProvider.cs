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
                { "add r10, r10, #21",             new byte[] { 0xE2, 0x8A, 0xA0, 0x15 } },
                { "add r10, r10, r10",             new byte[] { 0xE0, 0x8A, 0xA0, 0x0A } },
                { "add r0, r1, r2, lsl #2",        new byte[] { 0xE0, 0x81, 0x01, 0x02 } },
                { "add r0, r1, r2, lsl r4",        new byte[] { 0xE0, 0x81, 0x04, 0x12 } },
                { "add r0, r1, r2, lsr #2",        new byte[] { 0xE0, 0x81, 0x01, 0x22 } },
                { "add r0, r1, r2, lsr r4",        new byte[] { 0xE0, 0x81, 0x04, 0x32 } },
                { "add r0, r1, r2, asr #2",        new byte[] { 0xE0, 0x81, 0x01, 0x42 } },
                { "add r0, r1, r2, asr r4",        new byte[] { 0xE0, 0x81, 0x04, 0x52 } },
                { "add r0, r1, r2, ror #2",        new byte[] { 0xE0, 0x81, 0x01, 0x62 } },
                { "add r0, r1, r2, ror r4",        new byte[] { 0xE0, 0x81, 0x04, 0x72 } },
                { "add r0, r1, r2, rrx",           new byte[] { 0xE0, 0x81, 0x00, 0x62 } },

                { "and r5, r6, #432",              new byte[] { 0xE2, 0x06, 0x5E, 0x1B } },
                { "cmp r0, r1",                    new byte[] { 0xE1, 0x50, 0x00, 0x01 } },
                { "mov r0, r1",                    new byte[] { 0xE1, 0xA0, 0x00, 0x01 } },
                { "movs r0, r1",                   new byte[] { 0xE1, 0xB0, 0x00, 0x01 } },
                { "sub r7, r8, #234",              new byte[] { 0xE2, 0x48, 0x70, 0xEA } },
                { "subs r7, r8, #234",             new byte[] { 0xE2, 0x48, 0x70, 0xEA } },
                { "teq r9, r10",                   new byte[] { 0xE1, 0x39, 0x00, 0x0A } },
                { "tst r11, r12",                  new byte[] { 0xE1, 0x1B, 0x00, 0x0C } },

                { "ldr r0, [r1, #0]",              new byte[] { 0xE5, 0x91, 0x00, 0x00 } },
                { "ldr r2, [r3, -r4]",             new byte[] { 0xE7, 0x13, 0x20, 0x04 } },
                { "ldr r5, [r6, r7, lsl #12]",     new byte[] { 0xE7, 0x96, 0x56, 0x07 } },
                { "ldr r8, [r9, #0]!",             new byte[] { 0xE5, 0xB9, 0x80, 0x00 } },
                { "ldr r10, [r11, r12]!",          new byte[] { 0xE7, 0xBB, 0xA0, 0x0C } },
                { "ldr r13, [r14, r15, lsl #12]!", new byte[] { 0xE7, 0xBE, 0xD6, 0x0F } },
                { "ldr sp, [lr], #1231",           new byte[] { 0xE4, 0x9E, 0xD4, 0xCF } },
                { "ldr pc, [r0], -r1",             new byte[] { 0xE6, 0x10, 0xF0, 0x01 } },
                { "ldr r2, [r3], r4, lsl #12",     new byte[] { 0xE6, 0x93, 0x26, 0x04 } },

                { "ldrb r0, [r1, #0]",             new byte[] { 0xE5, 0xD1, 0x00, 0x00 } },
                { "ldrd r0, [r1, #0]",             new byte[] { 0xE1, 0xC1, 0x00, 0xD0 } },

                { "str r0, [r1, #0]",              new byte[] { 0xE5, 0x81, 0x00, 0x00 } },
                { "strh r0, [r1, #0]",             new byte[] { 0xE1, 0xC1, 0x00, 0xB0 } },

                { "b #1234",                       new byte[] { 0xEA, 0x00, 0x01, 0x34 } },
                { "bl #1234",                      new byte[] { 0xEB, 0x00, 0x01, 0x34 } }
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