// Copyright (c) 2018 Daniel A Hill. All rights reserved.
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

using System;

namespace ApiActions
{
    public class NamespaceActionTypeFilter : IActionTypeFilter
    {
        private readonly string[] _namespaceParts;

        public NamespaceActionTypeFilter(string namespacePrefix)
        {
            if (namespacePrefix == null) throw new ArgumentNullException(nameof(namespacePrefix));

            _namespaceParts = namespacePrefix.ToLowerInvariant().Split('.');
        }

        public bool Matches(Type apiActionType)
        {
            var typeNamespaceParts = (apiActionType.Namespace ?? string.Empty).Split('.');

            var matches = _namespaceParts.Length <= typeNamespaceParts.Length;

            for (var x = 0; x < _namespaceParts.Length && matches; x++)
            {
                matches = _namespaceParts[x].Equals(typeNamespaceParts[x], StringComparison.OrdinalIgnoreCase);
            }

            return matches;
        }
    }
}