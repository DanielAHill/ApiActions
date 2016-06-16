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
using System.Text;

namespace DanielAHill.AspNet.ApiActions.Swagger.Specification
{
    public class SwaggerSchema : ICustomSwaggerSerializable
    {
        public string Title { get; set; }
        public SwaggerType? Type { get; set; }
        public SwaggerObjectCollectionFacade<SwaggerProperty> Properties { get; set; }

        public void Serialize(StringBuilder builder, Action<object, StringBuilder, int> serializeChild, int recursionsLeft)
        {
            builder.Append("{\"title\":\"");
            builder.Append(Title);
            builder.Append("\",");

            if (Type.HasValue)
            {
                builder.Append("\"type\":\"");
                builder.Append(Type.Value.ToString().ToLowerInvariant());
                builder.Append("\",");
            }

            builder.Append("\"properties\":");
            serializeChild(Properties, builder, recursionsLeft);
            builder.Append("}");
        }
    }
}