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

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
{
    public class SwaggerDefinitionsFactory : ISwaggerDefinitionsFactory
    {
        private readonly IApiActionResponseInfoFactory _responseInfoFactory;
        private readonly ISwaggerSchemaFactory _schemaFactory;
        private readonly ISwaggerDefinitionNameProvider _definitionNameProvider;
        private readonly ISwaggerTypeProvider _typeProvider;

        public SwaggerDefinitionsFactory(IApiActionResponseInfoFactory responseInfoFactory, ISwaggerSchemaFactory schemaFactory, ISwaggerDefinitionNameProvider definitionNameProvider, ISwaggerTypeProvider typeProvider)
        {
            if (responseInfoFactory == null) throw new ArgumentNullException(nameof(responseInfoFactory));
            if (schemaFactory == null) throw new ArgumentNullException(nameof(schemaFactory));
            if (definitionNameProvider == null) throw new ArgumentNullException(nameof(definitionNameProvider));
            if (typeProvider == null) throw new ArgumentNullException(nameof(typeProvider));
            _responseInfoFactory = responseInfoFactory;
            _schemaFactory = schemaFactory;
            _definitionNameProvider = definitionNameProvider;
            _typeProvider = typeProvider;
        }

        public IReadOnlyCollection<SwaggerDefinition> Create(IReadOnlyCollection<IApiActionRegistration> registrations)
        {
            var typeQueue = new Queue<Type>(registrations.SelectMany(r => _responseInfoFactory.CreateResponses(r.ApiActionType)).Select(ri => ri.ResponseData).Where(t => t != null).Select(t => _typeProvider.GetTypeToDocument(t)));
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
                        Schema = _schemaFactory.Create(type, type.GetTypeDetails().PropertyReaders, typeQueue)
                    });
                }
            }
            
            return resultsLookup.Values.ToList();
        }
    }
}