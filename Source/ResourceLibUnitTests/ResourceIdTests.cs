using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Vestris.ResourceLib;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLibUnitTests
{
    [TestFixture]
    public class ResourceIdTests
    {
        [Test]
        public void TestIntResourceId()
        {
            // zero resource id
            ResourceId zeroRid = new ResourceId(IntPtr.Zero);
            Assert.AreEqual(IntPtr.Zero, zeroRid.Id);
            Assert.AreEqual("0", zeroRid.Name);
            // known resource types
            foreach (Kernel32.ResourceTypes resourceType in Enum.GetValues(typeof(Kernel32.ResourceTypes)))
            {
                ResourceId rid = new ResourceId((IntPtr)resourceType);
                Assert.AreEqual(new IntPtr((uint)resourceType), rid.Id);
                Assert.AreEqual(((uint)resourceType).ToString(), rid.Name);
            }
        }

        [Test]
        public void TestStringResourceId()
        {
            // empty resource id
            ResourceId emptyRid = new ResourceId(String.Empty);
            Assert.AreEqual(String.Empty, emptyRid.Name);
            Assert.AreEqual(String.Empty, Marshal.PtrToStringUni(emptyRid.Id));
            // known resource types
            foreach (Kernel32.ResourceTypes resourceType in Enum.GetValues(typeof(Kernel32.ResourceTypes)))
            {
                ResourceId rid = new ResourceId((IntPtr) resourceType);
                Assert.AreEqual(resourceType, rid.ResourceType);
                Assert.AreEqual(((uint) resourceType).ToString(), rid.Name);
            }
            // string resource types
            ResourceId stringRid = new ResourceId("CUSTOM");
            Assert.AreEqual("CUSTOM", stringRid.Name);
            Assert.AreEqual("CUSTOM", Marshal.PtrToStringUni(stringRid.Id));
        }

        [Test]
        public void TestResourceTypeInvalid()
        {
            ResourceId invalidRid = new ResourceId("CUSTOM");
            Assert.Throws<InvalidCastException>(() => Console.WriteLine(invalidRid.ResourceType));
        }

        [Test]
        public void TestResourceTypeResourceId()
        {
            // known resource types
            foreach (Kernel32.ResourceTypes resourceType in Enum.GetValues(typeof(Kernel32.ResourceTypes)))
            {
                ResourceId rid = new ResourceId(resourceType);
                Assert.AreEqual(resourceType, rid.ResourceType);
                Assert.AreEqual(((uint) resourceType).ToString(), rid.Name);
                Assert.AreEqual((IntPtr) resourceType, rid.Id);
            }
        }
    }
}
