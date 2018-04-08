using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiActions.Versioning
{
    [TestClass]
    public class ApiActionVersionTest
    {
        [DataTestMethod]
        [DataRow("1", "2")]
        [DataRow("0", "1")]
        [DataRow("1.1", "1.2")]
        [DataRow("1.1.1", "1.2")]
        [DataRow("1.1", "1.2.1.3")]
        [DataRow("1.1.1.1", "1.1.1.2")]
        [DataRow("1.1", "1.1.1")]
        public void InEqualComparisons(string smaller, string larger)
        {
            Assert.IsTrue(ApiActionVersion.TryParse(smaller, out var smallerVersion));
            Assert.IsTrue(ApiActionVersion.TryParse(larger, out var largerVersion));

            Assert.IsFalse(smallerVersion == largerVersion);
            Assert.IsFalse(smallerVersion.Equals(largerVersion));
            Assert.IsTrue(smallerVersion != largerVersion);
            Assert.AreNotEqual(smallerVersion.GetHashCode(), largerVersion.GetHashCode());

            Assert.IsTrue(smallerVersion < largerVersion);
            Assert.IsTrue(smallerVersion <= largerVersion);
            Assert.IsTrue(smallerVersion.CompareTo(largerVersion) < 0);

            Assert.IsTrue(largerVersion > smallerVersion);
            Assert.IsTrue(largerVersion >= smallerVersion);
            Assert.IsTrue(largerVersion.CompareTo(smallerVersion) > 0);

            Assert.AreNotEqual(largerVersion.ToString(), smallerVersion.ToString());
        }

        [DataTestMethod]
        [DataRow("1", "1")]
        [DataRow("1.2", "1.2")]
        [DataRow("1.2.3.4", "1.2.3.4")]
        public void EqualComparisons(string value1, string value2)
        {
            var version1 = ApiActionVersion.Parse(value1);
            var version2 = ApiActionVersion.Parse(value2);

            Assert.IsTrue(version1 == version1);
            Assert.IsTrue(version1.Equals(version1));
            Assert.IsFalse(version1 != version1);
            Assert.AreEqual(0, version1.CompareTo(version1));
            Assert.AreEqual(version1, version1);

            Assert.IsTrue(version1 == version2);
            Assert.IsTrue(version2 == version1);
            Assert.IsTrue(version1.Equals(version2));
            Assert.IsTrue(version2.Equals(version1));
            Assert.IsFalse(version1 != version2);
            Assert.AreEqual(0, version1.CompareTo(version2));
            Assert.AreEqual(0, version2.CompareTo(version1));
            Assert.AreEqual(version1, version2);
      
            Assert.AreEqual(version1.GetHashCode(), version2.GetHashCode());
            Assert.AreEqual(version1.ToString(), version2.ToString());
        }

        [DataTestMethod]
        [DataRow("0.-2.1")]
        [DataRow("")]
        [DataRow(null)]
        public void TryParseInvalidVersion(string value)
        {
            Assert.IsFalse(ApiActionVersion.TryParse(value, out var output));
            Assert.IsNull(output);
        }

        [TestMethod]
        public void ConstructorParameterRequired()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApiActionVersion(null));
            Assert.ThrowsException<ArgumentNullException>(() => new ApiActionVersion(new int [0]));
        }

        [TestMethod]
        public void DoesNotEqualNull()
        {
            var version = ApiActionVersion.Parse("1.0");

            Assert.AreNotEqual(version, null);
            Assert.IsFalse(version == null);
            Assert.IsFalse(version.Equals(null));
            Assert.IsTrue(version != null);
        }

        [TestMethod]
        public void ComparisonsErrorOnNull()
        {
            var version = ApiActionVersion.Parse("1.0");

            Assert.ThrowsException<ArgumentNullException>(() => version < null);
            Assert.ThrowsException<ArgumentNullException>(() => version <= null);
            Assert.ThrowsException<ArgumentNullException>(() => version > null);
            Assert.ThrowsException<ArgumentNullException>(() => version >= null);

            Assert.ThrowsException<ArgumentNullException>(() => null < version);
            Assert.ThrowsException<ArgumentNullException>(() => null <= version);
            Assert.ThrowsException<ArgumentNullException>(() => null > version);
            Assert.ThrowsException<ArgumentNullException>(() => null >= version);
        }
    }
}
