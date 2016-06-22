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
using System.IO;
using System.Reflection;
using DanielAHill.AspNet.ApiActions.Versioning;
using Microsoft.AspNet.Http;

namespace DanielAHill.AspNet.ApiActions.Conversion
{
    public class FormFileToStreamConverter: ITypeConverter
    {
        private static readonly TypeInfo StreamTypeInfo = typeof(Stream).GetTypeInfo();
        private static readonly TypeInfo FormFileTypeInfo = typeof(IFormFile).GetTypeInfo();

        public bool CanConvert(Type sourceType, Type destinationType)
        {
            return destinationType.GetTypeInfo().IsAssignableFrom(StreamTypeInfo) 
                && FormFileTypeInfo.IsAssignableFrom(sourceType.GetTypeInfo());
        }

        public object Convert(object source, Type destinationType)
        {
            return ((IFormFile) source).OpenReadStream();
        }
    }

    public class FromStringToApiActionVersionConverter: ITypeConverter
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
            if (source == null)
            {
                return null;
            }

            return ApiActionVersion.Parse(source.ToString());
        }
    }
}