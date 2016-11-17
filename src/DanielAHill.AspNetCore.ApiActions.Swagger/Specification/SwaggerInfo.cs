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
    /// 
    /// </summary>
    /// <remarks>http://swagger.io/specification/#infoObject</remarks>
    public class SwaggerInfo
    {
        /// <summary>
        /// Required. The title of the application.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A short description of the application. GFM syntax can be used for rich text representation
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Terms of Service for the API.
        /// </summary>
        public string TermsOfService { get; set; }

        /// <summary>
        /// The contact information for the exposed API.
        /// </summary>
        public SwaggerContact Contact { get; set; }

        /// <summary>
        /// The license information for the exposed API.
        /// </summary>
        public SwaggerLicense License { get; set; }

        /// <summary>
        /// Required Provides the version of the application API (not to be confused with the specification version).
        /// </summary>
        public string Version { get; set; }
    }
}