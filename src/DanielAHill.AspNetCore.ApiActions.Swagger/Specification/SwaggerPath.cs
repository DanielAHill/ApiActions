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
using System.Text;

namespace DanielAHill.AspNetCore.ApiActions.Swagger.Specification
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>http://swagger.io/specification/#pathsObject</remarks>
    public class SwaggerPath: ICustomSwaggerJsonSerializable
    {
        /// <summary>
        /// Gets or sets the relative path to an individual endpoint. The field name MUST begin with a slash. The path is appended to the basePath in order to construct the full URL. Path templating is allowed.
        /// </summary>
        /// <value>
        /// The relative path to an individual endpoint.
        /// </value>
        public string Path { get; set; }

        public SwaggerPathItem Item { get; set; }


        public void SerializeJson(StringBuilder builder, Action<object, StringBuilder, int> serializeChild, int recursionsLeft)
        {
            builder.Append('"');
            builder.Append(Path);
            builder.Append("\":");

            serializeChild(Item, builder, recursionsLeft);
        }
    }
}