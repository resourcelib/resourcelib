using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Vestris.ResourceLibUnitTests
{
    public abstract class ByteUtils
    {
        public static void CompareBytes(byte[] expectedBytes, byte[] testedBytes)
        {
            Console.WriteLine("Expected: {0}:{1}", expectedBytes, expectedBytes.Length);
            Console.WriteLine("Tested: {0}:{1}", testedBytes, testedBytes.Length);

            StringBuilder expectedString = new StringBuilder();
            StringBuilder testedString = new StringBuilder();

            int errors = 0;
            for (int i = 0; i < Math.Min(expectedBytes.Length, testedBytes.Length); i++)
            {
                if (char.IsLetterOrDigit((char)testedBytes[i]))
                    testedString.Append((char)testedBytes[i]);
                else if (testedBytes[i] != 0)
                    testedString.AppendFormat("[{0}]", (int)testedBytes[i]);

                if (char.IsLetterOrDigit((char)expectedBytes[i]))
                    expectedString.Append((char)expectedBytes[i]);
                else if (expectedBytes[i] != 0)
                    expectedString.AppendFormat("[{0}]", (int)expectedBytes[i]);

                if (expectedBytes[i] != testedBytes[i])
                {
                    Console.WriteLine(expectedString.ToString());
                    Console.WriteLine(testedString.ToString());
                }

                if (expectedBytes[i] != testedBytes[i])
                {
                    Console.WriteLine(string.Format("Error at offset {0}, expected {1} got {2}",
                        i, expectedBytes[i], testedBytes[i]));
                    errors++;
                }
            }
            Assert.AreEqual(expectedBytes.Length, testedBytes.Length, "Invalid byte count.");
            Assert.IsTrue(errors == 0, "Errors in binary comparisons.");
        }
    }
}
