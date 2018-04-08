// Copyright (c) 2017-2018 Daniel A Hill. All rights reserved.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ApiActions.Conversion.Caching;

namespace ApiActions.Conversion
{
    internal class ConverterDelegateProvider : IConverterDelegateProvider
    {
        private readonly ITypeConverter[] _typeConverters;

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, ICachedConverter>> _cachedConverters =
            new ConcurrentDictionary<Type, ConcurrentDictionary<Type, ICachedConverter>>();

        public ConverterDelegateProvider(IEnumerable<ITypeConverter> typeConverters)
        {
            _typeConverters = typeConverters.ToArray();
        }

        public ConverterDelegate Get(Type sourceType, Type destinationType)
        {
            if (sourceType == destinationType)
            {
                return ConvertSameObjectType;
            }

            var destinationCacheLevel =
                _cachedConverters.GetOrAdd(sourceType, c => new ConcurrentDictionary<Type, ICachedConverter>());
            var converter =
                destinationCacheLevel.GetOrAdd(destinationType, t => CreateConverter(sourceType, destinationType));

            if (converter == null)
            {
                return null;
            }

            return converter.Convert;
        }

        private ICachedConverter CreateConverter(Type sourceType, Type destinationType)
        {
            // Attempt Type Converters
            var typeConverter = _typeConverters.FirstOrDefault(c => c.CanConvert(sourceType, destinationType));
            if (typeConverter != null)
            {
                return new TypeConverterCachedConverter(typeConverter, destinationType);
            }

            // Attempt TypeDescriptor Conversion
            var converter = TypeDescriptor.GetConverter(sourceType);
            if (converter.CanConvertTo(destinationType))
            {
                return new TypeDescriptorConvertToCachedConverter(converter, destinationType);
            }

            converter = TypeDescriptor.GetConverter(destinationType);
            return converter.CanConvertFrom(sourceType)
                ? new TypeDescriptorConvertFromCachedConverter(converter)
                : null;
        }

        private static object ConvertSameObjectType(object source)
        {
            return source;
        }
    }
}