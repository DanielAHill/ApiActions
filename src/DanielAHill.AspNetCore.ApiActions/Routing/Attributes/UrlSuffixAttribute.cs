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

// ReSharper disable once CheckNamespace - Targeting Attributes should be in the namespace of their target
namespace DanielAHill.AspNetCore.ApiActions
{
    public class UrlSuffixAttribute : UrlAttribute
    {
        public UrlSuffixAttribute(string suffix) 
            : base("{namespace}/" + suffix)
        {
            if (suffix == null) throw new ArgumentNullException(nameof(suffix));
        }
    }
}