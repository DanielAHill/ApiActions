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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ApiActions.AbstractModeling.Application
{
    public class JsonAbstractModelApplicator : IAbstractModelApplicator
    {
        public string[] ContentTypes { get; } = {"application/json"};

        public bool Handles(IAbstractModelApplicationRequestContext context)
        {
            return context.ContentType != null
                   && context.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
        }

        public async Task ApplyAsync(IAbstractModelApplicationRequestContext context, AbstractModel abstractModel,
            CancellationToken cancellationToken)
        {
            InnerApply(JToken.Parse(await new StreamReader(context.Stream).ReadToEndAsync()), abstractModel);
        }

        private static void InnerApply(JToken item, AbstractModel abstractModel)
        {
            switch (item.Type)
            {
                case JTokenType.Array:
                    foreach (var arrayValue in item.Values())
                    {
                        InnerApply(arrayValue, abstractModel);
                    }

                    break;
                case JTokenType.Property:
                    var property = (JProperty) item;
                    var newModel = new AbstractModel(property.Name);
                    abstractModel.Add(newModel);
                    foreach (var child in item.Children())
                    {
                        InnerApply(child, newModel);
                    }

                    break;
                case JTokenType.Boolean:
                    abstractModel.AddValue(item.Value<bool>());
                    break;
                case JTokenType.Integer:
                    abstractModel.AddValue(item.Value<long>());
                    break;
                case JTokenType.Float:
                    abstractModel.AddValue(item.Value<decimal>());
                    break;
                case JTokenType.Object:
                    foreach (var child in item.Children())
                    {
                        InnerApply(child, abstractModel);
                    }

                    break;
                case JTokenType.Comment:
                case JTokenType.Constructor:
                    break;
                default:
                    abstractModel.AddValue(item.Value<string>());
                    break;
            }
        }
    }
}