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
namespace DanielAHill.AspNetCore.ApiActions.Swagger.Specification
{
    /// <summary>
    /// Describes a single API operation on a path.
    /// </summary>
    /// <remarks>
    /// http://swagger.io/specification/#operationObject
    /// </remarks>
    public class SwaggerOperation
    {
        /// <summary>
        /// A list of tags for API documentation control. Tags can be used for logical grouping of operations by resources or any other qualifier.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public string[] Tags { get; set; }

        /// <summary>
        /// A short summary of what the operation does. For maximum readability in the swagger-ui, this field SHOULD be less than 120 characters.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public string Summary { get; set; }

        /// <summary>
        /// A verbose explanation of the operation behavior. GFM syntax can be used for rich text representation.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this operation.
        /// </summary>
        /// <value>
        /// Additional external documentation for this operation.
        /// </value>
        public SwaggerExternalDocumentation ExternalDocs { get; set; }

        /// <summary>
        /// Unique string used to identify the operation. The id MUST be unique among all operations described in the API. Tools and libraries MAY use the operationId to uniquely identify an operation, therefore, it is recommended to follow common programming naming conventions.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public string OperationId { get; set; }

        /// <summary>
        /// A list of MIME types the operation can consume. This overrides the consumes definition at the Swagger Object. An empty value MAY be used to clear the global definition. Value MUST be as described under Mime Types.
        /// </summary>
        /// <value>
        /// A list of MIME types the operation can consume.
        /// </value>
        public string[] Consumes { get; set; }

        /// <summary>
        /// A list of MIME types the operation can produce. This overrides the produces definition at the Swagger Object. An empty value MAY be used to clear the global definition. Value MUST be as described under Mime Types.
        /// </summary>
        /// <value>
        /// A list of MIME types the operation can produce.
        /// </value>
        public string[] Produces { get; set; }

        /// <summary>
        /// A list of parameters that are applicable for this operation. If a parameter is already defined at the Path Item, the new definition will override it, but can never remove it. The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a name and location. The list can use the Reference Object to link to parameters that are defined at the Swagger Object's parameters. There can be one "body" parameter at most.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public object[] Parameters { get; set; }

        /// <summary>
        /// Required. The list of possible responses as they are returned from executing this operation.
        /// </summary>
        /// <value>
        /// The responses.
        /// </value>
        public SwaggerObjectCollectionFacade<SwaggerResponse> Responses { get; set; }

        /// <summary>
        /// The transfer protocol(s) for the operation. Values MUST be from the list: "http", "https", "ws", "wss". The value overrides the Swagger Object schemes definition.
        /// </summary>
        /// <value>
        /// The transfer protocol(s) for the operation.
        /// </value>
        public string[] Schemes { get; set; }

        /// <summary>
        /// Declares this operation to be deprecated. Usage of the declared operation should be refrained. Default value is false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deprecated; otherwise, <c>false</c>.
        /// </value>
        public bool Deprecated { get; set; }

        /// <summary>
        /// A declaration of which security schemes are applied for this operation. The list of values describes alternative security schemes that can be used (that is, there is a logical OR between the security requirements). This definition overrides any declared top-level security. To remove a top-level security declaration, an empty array can be used.
        /// </summary>
        /// <value>
        /// List of values describes alternative security schemes that can be used.
        /// </value>
        public object[] Security { get; set; }
    }
}