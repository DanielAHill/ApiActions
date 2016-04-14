#region Copyright
// Copyright (c) 2016 Daniel Alan Hill. All rights reserved.
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
using DanielAHill.AspNet.ApiActions.AbstractModeling;
using Xunit;

namespace DanielAHill.AspNet.ApiActions.Testing.Helpers
{
    public static class AbstractModelHelpers
    {
        public static void AssertEqualTo(this AbstractModel expected, AbstractModel actual)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));

            Assert.Equal(expected.ChildCount, actual.ChildCount);
            Assert.Equal(expected.ValueCount, expected.ValueCount);

            for (var x = 0; x < expected.ValueCount; x++)
            {
                var expectedValue = expected.Values[x]?.ToString();
                var actualValue = actual.Values[x]?.ToString();

                if (expectedValue == null)
                {
                    Assert.Null(actualValue);
                }
                else
                {
                    Assert.Equal(expectedValue, actualValue);
                }
            }

            foreach (var child in expected)
            {
                child.AssertEqualTo(actual[child.Name]);
            }
        }
    }
}
