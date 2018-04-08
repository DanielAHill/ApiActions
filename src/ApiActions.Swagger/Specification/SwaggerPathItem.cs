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
using System.Text;

namespace ApiActions.Swagger.Specification
{
    /// <summary>
    /// Describes the operations available on a single path. A Path Item may be empty, due to ACL constraints. The path itself is still exposed to the documentation viewer but they will not know which operations and parameters are available
    /// </summary>
    /// <remarks>http://swagger.io/specification/#pathItemObject</remarks>
    public class SwaggerPathItem : ICustomSwaggerJsonSerializable
    {
        /// <summary>
        /// Allows for an external definition of this path item. The referenced structure MUST be in the format of a Path Item Object. If there are conflicts between the referenced definition and this Path Item's definition, the behavior is undefined.
        /// </summary>
        /// <value>
        /// An external definition of this path item.
        /// </value>
        public string Ref { get; set; }

        public SwaggerObjectCollectionFacade<UnofficialPathItemMethod> Methods { get; set; }

        public SwaggerParameter[] Parameters { get; set; }

        public void SerializeJson(StringBuilder builder, Action<object, StringBuilder, int> serializeChild,
            int recursionsLeft)
        {
            builder.Append('{');

            if (Ref != null)
            {
                builder.Append("\"$ref\":");
                serializeChild(Ref, builder, recursionsLeft);
                builder.Append(',');
            }

            if (Parameters != null)
            {
                builder.Append("\"parameters\":");
                serializeChild(Parameters, builder, recursionsLeft);
                builder.Append(',');
            }

            var writeComma = false;

            foreach (var method in Methods)
            {
                if (writeComma)
                {
                    builder.Append(',');
                }
                else
                {
                    writeComma = true;
                }

                serializeChild(method, builder, recursionsLeft);
            }

            builder.Append('}');
        }
    }
}