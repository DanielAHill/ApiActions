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
using System.Collections.Generic;
using System.Linq;
using DanielAHill.AspNet.ApiActions.Introspection;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;
using DanielAHill.Reflection;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public class SwaggerDefinitionsFactory : ISwaggerDefinitionsFactory
    {
        private readonly IApiActionResponseInfoFactory _responseInfoFactory;
        private readonly ISwaggerTypeProvider _swaggerTypeConverter;
        private readonly ISwaggerDefinitionNameProvider _definitionNameProvider;

        public SwaggerDefinitionsFactory(IApiActionResponseInfoFactory responseInfoFactory, ISwaggerTypeProvider swaggerTypeConverter, ISwaggerDefinitionNameProvider definitionNameProvider)
        {
            if (responseInfoFactory == null) throw new ArgumentNullException(nameof(responseInfoFactory));
            if (swaggerTypeConverter == null) throw new ArgumentNullException(nameof(swaggerTypeConverter));
            if (definitionNameProvider == null) throw new ArgumentNullException(nameof(definitionNameProvider));
            _responseInfoFactory = responseInfoFactory;
            _swaggerTypeConverter = swaggerTypeConverter;
            _definitionNameProvider = definitionNameProvider;
        }

        public IReadOnlyCollection<SwaggerDefinition> Create(IReadOnlyCollection<IApiActionRegistration> registrations)
        {
            var typeQueue = new Queue<Type>(registrations.SelectMany(r => _responseInfoFactory.CreateResponses(r.ApiActionType)).Select(ri => ri.ResponseData).Where(t => t != null));
            var resultsLookup = new Dictionary<string, SwaggerDefinition>();

            while (typeQueue.Count > 0)
            {
                var type = typeQueue.Dequeue();
                var name = _definitionNameProvider.GetDefinitionName(type);

                if (!resultsLookup.ContainsKey(name))
                {
                    resultsLookup.Add(name, new SwaggerDefinition()
                    {
                        Name = name,
                        Schema = CreateSchema(type, typeQueue)
                    });
                }
            }
            
            return resultsLookup.Values.ToList();
        }

        private SwaggerSchema CreateSchema(Type apiActionType, Queue<Type> typeQueue)
        {
#if DEBUG
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));
            if (typeQueue == null) throw new ArgumentNullException(nameof(typeQueue));
#endif

            var schema = new SwaggerSchema() {
                Title = apiActionType.Name,
                Type = _swaggerTypeConverter.GetSwaggerType(apiActionType)
            };

            if (schema.Type == SwaggerType.Object)
            {
                schema.Properties = new SwaggerObjectCollectionFacade<SwaggerProperty>(apiActionType.GetTypeDetails().PropertyReaders.Select(r => CreateProperty(r, typeQueue)));
            }

            return schema;
        }

        private SwaggerProperty CreateProperty(IPropertyReader propertyReader, Queue<Type> typeQueue)
        {
#if DEBUG
            if (propertyReader == null) throw new ArgumentNullException(nameof(propertyReader));
            if (typeQueue == null) throw new ArgumentNullException(nameof(typeQueue));
#endif

            var swaggerType = _swaggerTypeConverter.GetSwaggerType(propertyReader.PropertyType);

            if (swaggerType == SwaggerType.Object)
            {
                typeQueue.Enqueue(propertyReader.PropertyType);
                return new ReferencedSwaggerProperty() { Name = propertyReader.Name, Reference = _definitionNameProvider.GetDefinitionName(propertyReader.PropertyType)};
            }

            if (swaggerType == SwaggerType.Array)
            {
                var propertyType = _swaggerTypeConverter.GetTypeToDocument(propertyReader.PropertyType);

                typeQueue.Enqueue(propertyType);

                return new ArraySwaggerProperty()
                {
                    Name = propertyReader.Name,
                    Reference = _definitionNameProvider.GetDefinitionName(propertyType)
                };
            }

            return new TypedSwaggerProperty() {Name = propertyReader.Name, Type = swaggerType};
        }
    }
}