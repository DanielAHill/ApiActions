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
// ReSharper disable once RedundantUsingDirective
using System;
using System.Reflection;

namespace DanielAHill.AspNet.ApiActions.AbstractModeling
{
    public static class AbstractModelExtensions
    {
        public static bool IsAbstractModelValueType(this object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return value.GetType().IsAbstractModelValueType();
        }

        public static bool IsAbstractModelValueType(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type == typeof(string) || type.GetProperties().Length == 0;
        }
    }
}