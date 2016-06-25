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
namespace DanielAHill.AspNet.ApiActions.Swagger.Specification
{
    // Specification at: https://github.com/OAI/OpenAPI-Specification/blob/master/versions/2.0.md
    public class SwaggerBase
    {
        /// <summary>
        /// Required. Specifies the Swagger Specification version being used. It can be used by the Swagger UI and other clients to interpret the API listing. The value MUST be "2.0".
        /// </summary>
        public string Swagger { get; } = "2.0";

        /// <summary>
        /// Required. Provides metadata about the API. The metadata can be used by the clients if needed.
        /// </summary>
        public SwaggerInfo Info { get; set; }

        /// <summary>
        /// The host (name or ip) serving the API. This MUST be the host only and does not include the scheme nor sub-paths. It MAY include a port. If the host is not included, the host serving the documentation is to be used (including the port). The host does not support path templating.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The base path on which the API is served, which is relative to the host. If it is not included, the API is served directly under the host. The value MUST start with a leading slash (/). The basePath does not support path templating.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// The transfer protocol of the API. Values MUST be from the list: "http", "https", "ws", "wss". If the schemes is not included, the default scheme to be used is the one used to access the Swagger definition itself.
        /// </summary>
        public string[] Schemes { get; set; }

        /// <summary>
        /// A list of MIME types the APIs can consume. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under Mime Types.
        /// </summary>
        public string[] Consumes { get; set; }

        /// <summary>
        /// A list of MIME types the APIs can produce. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under Mime Types.
        /// </summary>
        public string[] Produces { get; set; }

        /// <summary>
        /// Required. The available paths and operations for the API.
        /// </summary>
        public SwaggerObjectCollectionFacade<SwaggerPath> Paths { get; set; }

        /// <summary>
        /// An object to hold data types produced and consumed by operations.
        /// </summary>
        public SwaggerObjectCollectionFacade<SwaggerDefinition> Definitions { get; set; }

        /// <summary>
        /// An object to hold parameters that can be used across operations. This property does not define global parameters for all operations.
        /// </summary>s
        public object Parameters { get; set; }

        /// <summary>
        /// An object to hold responses that can be used across operations. This property does not define global responses for all operations.
        /// </summary>
        public object Responses { get; set; }

        /// <summary>
        /// Security scheme definitions that can be used across the specification.
        /// </summary>
        public object SecurityDefinitions { get; set; }

        /// <summary>
        /// A declaration of which security schemes are applied for the API as a whole. The list of values describes alternative security schemes that can be used (that is, there is a logical OR between the security requirements). Individual operations can override this definition.
        /// </summary>
        public object Security { get; set; }

        /// <summary>
        /// A list of tags used by the specification with additional metadata. The order of the tags can be used to reflect on their order by the parsing tools. Not all tags that are used by the Operation Object must be declared. The tags that are not declared may be organized randomly or based on the tools' logic. Each tag name in the list MUST be unique.
        /// </summary>
        public object Tags { get; set; }

        /// <summary>
        /// Additional external documentation.
        /// </summary>
        public SwaggerExternalDocumentation ExternalDocs { get; set; }
    }
}