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
using System.Linq;

namespace DanielAHill.AspNet.ApiActions.Versioning
{
    public class ApiActionVersion : IEquatable<ApiActionVersion>
    {
        private readonly int[] _parts;

        public ApiActionVersion(params int[] parts)
        {
            if (parts == null) throw new ArgumentNullException(nameof(parts));

            if (parts.Any(p => p < 0))
            {
                throw new ArgumentOutOfRangeException(nameof(parts), "All version sections must be equal to or greater than zero");
            }

            _parts = parts;
        }

        public override string ToString()
        {
            return _parts.Select(p => p.ToString()).Aggregate((a, b) => string.Concat(a, ".", b));
        }

        #region Equatable

        public bool Equals(ApiActionVersion other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ApiActionVersion)obj);
        }

        public override int GetHashCode()
        {
            return _parts?.GetHashCode() ?? 0;
        }

        #endregion

        #region Parsing
        public static ApiActionVersion Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            return new ApiActionVersion(value.Split('.').Select(int.Parse).ToArray());
        }

        public static bool TryParse(string value, out ApiActionVersion version)
        {
            try
            {
                version = Parse(value);
                return true;
            }
            catch (Exception)
            {
                version = null;
                return false;
            }
        }
        #endregion

        #region Operator Overrides

        public static bool operator <(ApiActionVersion item1, ApiActionVersion item2)
        {
            if (item1 == null) throw new ArgumentNullException(nameof(item1));
            if (item2 == null) throw new ArgumentNullException(nameof(item2));

            var item1Parts = item1._parts;
            var item2Parts = item2._parts;

            for (var x = 0; x < Math.Min(item1Parts.Length, item2Parts.Length); x++)
            {
                if (item1Parts[x] > item2Parts[x])
                {
                    return false;
                }
            }

            return item1Parts.Length < item2Parts.Length;
        }

        public static bool operator >(ApiActionVersion item1, ApiActionVersion item2)
        {
            if (item1 == null) throw new ArgumentNullException(nameof(item1));
            if (item2 == null) throw new ArgumentNullException(nameof(item2));

            var item1Parts = item1._parts;
            var item2Parts = item2._parts;

            for (var x = 0; x < Math.Min(item1Parts.Length, item2Parts.Length); x++)
            {
                if (item1Parts[x] < item2Parts[x])
                {
                    return false;
                }
            }

            return item1Parts.Length > item2Parts.Length;
        }

        public static bool operator <=(ApiActionVersion item1, ApiActionVersion item2)
        {
            if (item1 == null) throw new ArgumentNullException(nameof(item1));
            if (item2 == null) throw new ArgumentNullException(nameof(item2));

            var item1Parts = item1._parts;
            var item2Parts = item2._parts;

            if (item2Parts.Length < item1Parts.Length)
            {
                return false;
            }

            for (var x = 0; x < item1Parts.Length; x++)
            {
                if (item1Parts[x] > item2Parts[x])
                {
                    return false;
                }

                if (item1Parts[x] < item2Parts[x])
                {
                    return true;
                }
            }

            return true;
        }

        public static bool operator >=(ApiActionVersion item1, ApiActionVersion item2)
        {
            if (item1 == null) throw new ArgumentNullException(nameof(item1));
            if (item2 == null) throw new ArgumentNullException(nameof(item2));

            var item1Parts = item1._parts;
            var item2Parts = item2._parts;

            if (item2Parts.Length > item1Parts.Length)
            {
                return false;
            }

            for (var x = 0; x < item2Parts.Length; x++)
            {
                if (item1Parts[x] < item2Parts[x])
                {
                    return false;
                }

                if (item1Parts[x] > item2Parts[x])
                {
                    return true;
                }
            }

            return true;
        }

        public static bool operator ==(ApiActionVersion item1, ApiActionVersion item2)
        {
            if (ReferenceEquals(item1, item2))
            {
                return true;
            }

            if (((object)item1 == null) || ((object)item2 == null))
            {
                return false;
            }

            var item1Parts = item1._parts;
            var item2Parts = item2._parts;

            var areEqual = item1Parts.Length == item2Parts.Length;
            for (var x = 0; areEqual && x < item1Parts.Length; x++)
            {
                areEqual = item1Parts[x] == item2Parts[x];
            }

            return areEqual;
        }

        public static bool operator !=(ApiActionVersion item1, ApiActionVersion item2)
        {
            return !(item1 == item2);
        }
        #endregion
    }
}