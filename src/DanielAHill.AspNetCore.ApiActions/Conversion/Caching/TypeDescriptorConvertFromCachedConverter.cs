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

using System.ComponentModel;

namespace DanielAHill.AspNetCore.ApiActions.Conversion.Caching
{
    internal class TypeDescriptorConvertFromCachedConverter : ICachedConverter
    {
        private readonly TypeConverter _typeConverter;
        
        internal TypeDescriptorConvertFromCachedConverter(TypeConverter typeConverter)
        {
            _typeConverter = typeConverter;
        }

        public object Convert(object value)
        {
            return _typeConverter.ConvertFrom(value);
        }
    }
}