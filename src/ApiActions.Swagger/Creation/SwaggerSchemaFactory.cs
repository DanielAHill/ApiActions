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
using System.Collections.Generic;
using System.Linq;
using ApiActions.Swagger.Specification;
using DanielAHill.Reflection;

namespace ApiActions.Swagger.Creation
{
    public class SwaggerSchemaFactory : ISwaggerSchemaFactory
    {
        private readonly ISwaggerTypeProvider _swaggerTypeConverter;
        private readonly ISwaggerPropertyFactory _propertyFactory;

        public SwaggerSchemaFactory(ISwaggerTypeProvider swaggerTypeConverter, ISwaggerPropertyFactory propertyFactory)
        {
            _swaggerTypeConverter =
                swaggerTypeConverter ?? throw new ArgumentNullException(nameof(swaggerTypeConverter));
            _propertyFactory = propertyFactory ?? throw new ArgumentNullException(nameof(propertyFactory));
        }

        public SwaggerSchema Create(Type type, IEnumerable<IPropertyDetails> propertyDetails)
        {
            return Create(type, propertyDetails, null);
        }

        public SwaggerSchema Create(Type type, IEnumerable<IPropertyDetails> propertyDetails, Queue<Type> typeQueue)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var schema = new SwaggerSchema
            {
                Title = type.Name,
                Type = _swaggerTypeConverter.GetSwaggerType(type)
                // TODO: Required
            };

            if (schema.Type == SwaggerType.Object && propertyDetails != null)
            {
                schema.Properties =
                    new SwaggerObjectCollectionFacade<SwaggerProperty>(
                        propertyDetails.Select(p => _propertyFactory.Create(p, typeQueue)));
            }

            return schema;
        }
    }
}