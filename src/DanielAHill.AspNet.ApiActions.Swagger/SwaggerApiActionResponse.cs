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
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using DanielAHill.AspNet.ApiActions.Serialization;
using DanielAHill.AspNet.ApiActions.Swagger.Specification;
using Microsoft.AspNet.Http;

namespace DanielAHill.AspNet.ApiActions.Swagger
{
    internal class SwaggerApiActionResponse : ApiActionResponse
    {
        private readonly SwaggerSchema _root;

        internal SwaggerApiActionResponse(SwaggerSchema root)
        {
#if DEBUG
            if (root == null) throw new ArgumentNullException(nameof(root));
#endif

            _root = root;
        }

        public override Task WriteAsync(HttpContext httpContext, IEdgeSerializer edgeSerializer, CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();
            Serialize(_root, builder, 50);

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());

            var response = httpContext.Response;
            
            return response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        private static void Serialize(object item, StringBuilder builder, int recursionsLeft)
        {
            if (item == null)
            {
                builder.Append("null");
                return;
            }

            // Perform custom serialization, if supported
            var customSerializable = item as ICustomSwaggerSerializable;
            if (customSerializable != null)
            {
                customSerializable.Serialize(builder, Serialize, recursionsLeft - 1);
                return;
            }

            var propertyReaders = item.GetType().GetTypeDetails().PropertyReaders;

            // Serialize value types and any items without readable properties
            var itemTypeDetails = item.GetTypeDetails();
            if (itemTypeDetails.IsValue || !propertyReaders.Any())
            {
                if (itemTypeDetails.IsNumeric)
                {
                    builder.Append(item);
                }
                else
                {
                    builder.Append('"');
                    builder.AppendJsonDelimited(item.ToString());
                    builder.Append('"');
                }
                return;
            }

            builder.Append("{");

            // Serialize classes with properties
            var addComma = false;
            foreach (var reader in propertyReaders)
            {
                var value = reader.Read(item);
                var valueTypeDetails = reader.PropertyType.GetTypeDetails();

                if (value != valueTypeDetails.DefaultValue)
                {
                    if (addComma)
                    {
                        builder.Append(',');
                    }
                    else
                    {
                        addComma = true;
                    }

                    builder.Append('"');
                    builder.Append(char.ToLowerInvariant(reader.Name[0]));
                    builder.Append(reader.Name.Substring(1));
                    builder.Append("\":");
                    Serialize(value, builder, recursionsLeft - 1);
                }
            }

            builder.Append("}");
        }

    }
}