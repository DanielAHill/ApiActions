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
using System.Reflection;
using DanielAHill.AspNetCore.ApiActions.Conversion;
using Microsoft.Extensions.Logging;

namespace DanielAHill.AspNetCore.ApiActions.AbstractModeling
{
    public class RequestModelFactory : IRequestModelFactory
    {
        private readonly IConverterDelegateProvider _converterDelegateProvider;
        private readonly ILogger<RequestModelFactory> _log;
        
        public RequestModelFactory(IConverterDelegateProvider converterDelegateProvider, ILogger<RequestModelFactory> log)
        {
            if (converterDelegateProvider == null) throw new ArgumentNullException(nameof(converterDelegateProvider));
            if (log == null) throw new ArgumentNullException(nameof(log));
            _converterDelegateProvider = converterDelegateProvider;
            _log = log;
        }

        public T Create<T>(AbstractModel abstractModel) where T : class, new()
        {
            if (abstractModel == null) throw new ArgumentNullException(nameof(abstractModel));
            return (T)Create(typeof (T), abstractModel);
        }

        private object Create(Type type, AbstractModel abstractModel)
        {
            var model = Activator.CreateInstance(type);
            var typeDetails = type.GetTypeDetails();
            var propertyWriters = typeDetails.PropertyWriters;

            for (var x = 0; x < propertyWriters.Count; x++)
            {
                var writer = propertyWriters[x];

                var abstractProperty = abstractModel[writer.Name];
                if (abstractProperty == null)
                {
                    continue;
                }

                object value;

                if (abstractProperty.ChildCount > 0)
                {
                    value = Create(writer.PropertyType, abstractProperty);
                }
                if (abstractProperty.ValueCount > 0)
                {
                    value = ChangeType(abstractProperty.Values[0], writer.PropertyType);
                }
                else
                {
#if DEBUG
                    // TODO?: Consider if this is okay or not
                    throw new InvalidOperationException();
#else
                    continue;
#endif
                }

                writer.Write(model, value);
            }

            return model;
        }

        private object ChangeType(object value, Type destinationType)
        {
#if DEBUG
            if (value == null) throw new ArgumentNullException(nameof(value));
#endif
            try
            {
                var converter = _converterDelegateProvider.Get(value.GetType(), destinationType);
                if (converter != null)
                {
                    return converter(value);
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning($"Could not convert {value.GetType()} to {destinationType}", ex);
            }

            return destinationType.GetTypeInfo().IsValueType ? Activator.CreateInstance(destinationType) : null;
        }
    }
}