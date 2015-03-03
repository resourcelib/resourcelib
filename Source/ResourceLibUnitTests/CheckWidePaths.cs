using System;
using System.IO;
using System.Reflection;

using NUnit.Framework;

using Vestris.ResourceLib;

namespace Vestris.ResourceLibUnitTests
{
    /// <summary>
    /// Check operation with files at paths with non-ANSI chars.
    /// </summary>
    [TestFixture]
    public class CheckWidePaths
    {
        [TestCase("atl.dll", "OneΔυοТриארבעה" /* whatever your ANSI codepage, this won't fit in any single one */)]
        public void NoExceptionsOnNonAnsiPaths(string pefile, string widepath)
        {
            string widedir = Path.Combine(Path.GetTempPath(), widepath);
            try
            {
                if(Directory.Exists(widedir))
                    Directory.Delete(widedir, true);
                Directory.CreateDirectory(widedir);

                // Some binary file
                string pefileSrc = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase, UriKind.Absolute).LocalPath), "Binaries\\" + pefile);
                Assert.That(File.Exists(pefileSrc), Is.True);
                string pefileDest = Path.Combine(widedir, pefile);
                File.Copy(pefileSrc, pefileDest);

                // Check it works — the operation is not important, LoadLibraryEx failed any of them
                var versionResource = new VersionResource();
                versionResource.Language = ResourceUtil.USENGLISHLANGID;
                versionResource.LoadFrom(pefileDest); // “System.ComponentModel.Win32Exception : The system cannot find the file specified” if fails
                DumpResource.Dump(versionResource);
            }
            finally
            {
                try
                {
                    Directory.Delete(widedir, true);
                }
                catch
                {
                    // Ignore, that's post-test cleanup
                }
            }
        }
    }
}