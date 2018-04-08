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
using System.Collections.Generic;
using ApiActions.Swagger.Specification;
using DanielAHill.Reflection;

namespace ApiActions.Swagger.Creation
{
    public class SwaggerPropertyFactory : ISwaggerPropertyFactory
    {
        private readonly ISwaggerTypeProvider _swaggerTypeConverter;
        private readonly ISwaggerDefinitionNameProvider _definitionNameProvider;

        public SwaggerPropertyFactory(ISwaggerTypeProvider swaggerTypeConverter,
            ISwaggerDefinitionNameProvider definitionNameProvider)
        {
            _swaggerTypeConverter =
                swaggerTypeConverter ?? throw new ArgumentNullException(nameof(swaggerTypeConverter));
            _definitionNameProvider =
                definitionNameProvider ?? throw new ArgumentNullException(nameof(definitionNameProvider));
        }

        public SwaggerProperty Create(IPropertyDetails propertyDetails)
        {
            return Create(propertyDetails, null);
        }

        public SwaggerProperty Create(IPropertyDetails propertyDetails, Queue<Type> typeQueue)
        {
            if (propertyDetails == null) throw new ArgumentNullException(nameof(propertyDetails));

            var swaggerType = _swaggerTypeConverter.GetSwaggerType(propertyDetails.PropertyType);

            switch (swaggerType)
            {
                case SwaggerType.Object:
                    typeQueue.Enqueue(propertyDetails.PropertyType);
                    return new ReferencedSwaggerProperty
                    {
                        Name = propertyDetails.Name,
                        Reference = _definitionNameProvider.GetDefinitionName(propertyDetails.PropertyType)
                    };
                case SwaggerType.Array:
                    var propertyType = _swaggerTypeConverter.GetTypeToDocument(propertyDetails.PropertyType);

                    typeQueue?.Enqueue(propertyType);

                    return new ArraySwaggerProperty
                    {
                        Name = propertyDetails.Name,
                        Reference = _definitionNameProvider.GetDefinitionName(propertyType)
                    };
            }

            return new TypedSwaggerProperty {Name = propertyDetails.Name, Type = swaggerType};
        }
    }
}