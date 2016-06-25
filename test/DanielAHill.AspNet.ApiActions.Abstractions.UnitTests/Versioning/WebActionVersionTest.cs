#region Copyright
// Copyright (c) 2016 Daniel A Hill. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using DanielAHill.AspNet.ApiActions.Versioning;
using Xunit;

namespace DanielAHill.AspNet.ApiActions.Abstractions.UnitTests.Versioning
{
    public class ApiActionVersionTest
    {
        #region Equality Tests

        [Fact]
        public void EqualsSelf()
        {
            // Arrange
            var version = new ApiActionVersion(2,2,1);

            // Act & Assert
            // ReSharper disable once EqualExpressionComparison
            Assert.True(version == version, "Operator Equal");
            Assert.True(version.Equals((object)version), "IEquatable Equal");
            Assert.True(version.Equals(version), "IEquatable<ApiActionVersion> Equal");
            // ReSharper disable once EqualExpressionComparison
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.True((ApiActionVersion)null == (ApiActionVersion)null, "null == null");
        }

        [Fact]
        public void EqualsSameData()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var versionCopy = new ApiActionVersion(2, 2, 1);

            // Act & Assert
            Assert.True(version == versionCopy, "Operator Equal");
            Assert.True(version.Equals((object)versionCopy), "IEquatable Equal");
            Assert.True(version.Equals(versionCopy), "IEquatable<ApiActionVersion> Equal");
        }

        [Fact]
        public void DoesNotEqualDifferentData()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var sameLength = new ApiActionVersion(2, 2, 2);
            var longerLength = new ApiActionVersion(2, 2, 1, 5);
            var shorterLength = new ApiActionVersion(2, 2);

            // Act & Assert
            Assert.False(version == sameLength, "Operator Equal, same length");
            Assert.False(version.Equals((object)sameLength), "IEquatable Equal, same length");
            Assert.False(version.Equals(sameLength), "IEquatable<ApiActionVersion> Equal, same length");
            Assert.False(version == longerLength, "Operator Equal, longer length");
            Assert.False(version.Equals((object)longerLength), "IEquatable Equal, longer length");
            Assert.False(version.Equals(longerLength), "IEquatable<ApiActionVersion> Equal, longer length");
            Assert.False(version == shorterLength, "Operator Equal, shorter length");
            Assert.False(version.Equals((object)shorterLength), "IEquatable Equal, shorter length");
            Assert.False(version.Equals(shorterLength), "IEquatable<ApiActionVersion> Equal, shorter length");
        }
   
        [Fact]
        public void DoesNotEqualNull()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);

            // Act & Assert
            // ReSharper disable once RedundantCast
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.False(version == (ApiActionVersion)null, "Operator Equal");
            Assert.False(version.Equals((object)null), "IEquatable Equal");
            Assert.False(version.Equals(null), "IEquatable<ApiActionVersion> Equal");
        }

        #endregion

        #region Greater Than Or Equal To Tests

        [Fact]
        public void GreaterThanOrEqualToSelf()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);

            // Act & Assert
            // ReSharper disable once EqualExpressionComparison
            Assert.True(version >= version);
        }

        [Fact]
        public void GreaterThanLessSpecific()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var lessSpecific = new ApiActionVersion(2, 2);

            // Act & Assert
            Assert.True(version >= lessSpecific, ">=");
            Assert.False(version <= lessSpecific, "Negated >=");
            Assert.False(version == lessSpecific, "Should not be equal");
        }

        [Fact]
        public void GreaterThanIncrementLess()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var incrementLess = new ApiActionVersion(2, 2, 0);

            // Act & Assert
            Assert.True(version >= incrementLess, ">=");
            Assert.False(version <= incrementLess, "Negated >=");
            Assert.False(version == incrementLess, "Should not be equal");
        }

        [Fact]
        public void GreaterThanSecondLevelIncrementLess()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var incrementLess = new ApiActionVersion(2, 1, 1);

            // Act & Assert
            Assert.True(version >= incrementLess, ">=");
            Assert.False(version <= incrementLess, "Negated >=");
            Assert.False(version == incrementLess, "Should not be equal");
        }

        [Fact]
        public void GreaterThanTopLevelIncrementLess()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var incrementLess = new ApiActionVersion(1, 2, 1);

            // Act & Assert
            Assert.True(version >= incrementLess, ">=");
            Assert.False(version <= incrementLess, "Negated >=");
            Assert.False(version == incrementLess, "Should not be equal");
        }

        [Fact]
        public void GreaterThanLessWithHigherSubParts()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var incrementLess = new ApiActionVersion(1, 8, 10);

            // Act & Assert
            Assert.True(version >= incrementLess, ">=");
            Assert.False(version <= incrementLess, "Negated >=");
            Assert.False(version == incrementLess, "Should not be equal");
        }

        #endregion

        #region Less Than Or Equal To Tests

        [Fact]
        public void LessThanOrEqualToSelf()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);

            // Act & Assert
            // ReSharper disable once EqualExpressionComparison
            Assert.True(version <= version);
        }

        [Fact]
        public void LessThanMoreSpecific()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2);
            var lessSpecific = new ApiActionVersion(2, 2, 1);

            // Act & Assert
            Assert.True(version <= lessSpecific, "<=");
            Assert.False(version >= lessSpecific, "Negated <=");
            Assert.False(version == lessSpecific, "Should not be equal");
        }

        [Fact]
        public void LessThanIncrementMore()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var incrementMore = new ApiActionVersion(2, 2, 2);

            // Act & Assert
            Assert.True(version <= incrementMore, "<=");
            Assert.False(version >= incrementMore, "Negated <=");
            Assert.False(version == incrementMore, "Should not be equal");
        }

        [Fact]
        public void LessThanSecondLevelIncrementMore()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var incrementMore = new ApiActionVersion(2, 3, 1);

            // Act & Assert
            Assert.True(version <= incrementMore, "<=");
            Assert.False(version >= incrementMore, "Negated <=");
            Assert.False(version == incrementMore, "Should not be equal");
        }

        [Fact]
        public void LessThanTopLevelIncrementMore()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var incrementMore = new ApiActionVersion(3, 2, 1);

            // Act & Assert
            Assert.True(version <= incrementMore, "<=");
            Assert.False(version >= incrementMore, "Negated <=");
            Assert.False(version == incrementMore, "Should not be equal");
        }

        [Fact]
        public void LessThanMoreWithLowerSubParts()
        {
            // Arrange
            var version = new ApiActionVersion(2, 2, 1);
            var incrementMore = new ApiActionVersion(3, 1, 0);

            // Act & Assert
            Assert.True(version <= incrementMore, "<=");
            Assert.False(version >= incrementMore, "Negated <=");
            Assert.False(version == incrementMore, "Should not be equal");
        }

        #endregion

        #region Parse And Back To String

        [Fact]
        public void ParseValidValueThenToString()
        {
            PerformParseTest("0");
            PerformParseTest("0.0");
            PerformParseTest("0.0.0");
            PerformParseTest("0.0.0.0");
            PerformParseTest("0.0.0.0.0");

            PerformParseTest("2.2.2.2");
            PerformParseTest("22.22.22.22");
            PerformParseTest("22.0.22.0");
            PerformParseTest("0.22.0.22");

            PerformParseTest("1.2.3.4.5.6.7.8.9.10.11.12.13.14.15.16.17.18.19.20.21.22.23.24.25.26.27.28.29.30.31.32");
            PerformParseTest("2147483647.2147483647.2147483647.2147483647.2147483647.2147483647.2147483647.2147483647.2147483647.2147483647.2147483647");
        }

        [Fact]
        public void ParseInValidValue()
        {
            PerformParseTest(null, false);
            PerformParseTest(string.Empty, false);
            PerformParseTest("-1", false);
            PerformParseTest("-1.-1.-1", false);
            PerformParseTest("22.22.-22.22", false);
            PerformParseTest("22..22", false);
            PerformParseTest("22.22.", false);
            PerformParseTest("22,22", false);
            PerformParseTest("22,22", false);
        }

        private static void PerformParseTest(string value, bool shouldSucceed = true)
        {
            ApiActionVersion tryParseVersion;
            ApiActionVersion directParseVersion = null;
            var directParseResult = true;
            string directParseString = null;
            string tryParseString = null;

            // Act
            var tryParseResult = ApiActionVersion.TryParse(value, out tryParseVersion);
            try
            {
                directParseVersion = ApiActionVersion.Parse(value);
            }
            catch (Exception)
            {
                if (shouldSucceed)
                {
                    throw;
                }

                directParseResult = false;
            }

            if (tryParseResult)
            {
                tryParseString = tryParseVersion.ToString();
            }

            if (directParseResult)
            {
                directParseString = tryParseVersion.ToString();
            }

            // Assert
            Assert.Equal(shouldSucceed, directParseResult);
            Assert.Equal(shouldSucceed, tryParseResult);

            if (shouldSucceed)
            {
                Assert.Equal(directParseVersion, tryParseVersion);
                Assert.Equal(value, directParseString);
                Assert.Equal(value, tryParseString);
            }
        }

        #endregion
    }
}
