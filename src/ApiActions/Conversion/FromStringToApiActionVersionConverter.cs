// Copyright (c) 2018-2018 Daniel A Hill. All rights reserved.
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
using System.Reflection;
using ApiActions.Versioning;

namespace ApiActions.Conversion
{
    public class FromStringToApiActionVersionConverter : ITypeConverter
    {
        private static readonly TypeInfo StringTypeInfo = typeof(string).GetTypeInfo();
        private static readonly TypeInfo ApiActionVersionTypeInfo = typeof(ApiActionVersion).GetTypeInfo();

        public bool CanConvert(Type sourceType, Type destinationType)
        {
            return destinationType.GetTypeInfo().IsAssignableFrom(ApiActionVersionTypeInfo)
                   && StringTypeInfo.IsAssignableFrom(sourceType.GetTypeInfo());
        }

        public object Convert(object source, Type destinationType)
        {
            return source == null ? null : ApiActionVersion.Parse(source.ToString());
        }
    }
}