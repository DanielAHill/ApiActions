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
namespace DanielAHill.AspNet.ApiActions.Swagger.Specification
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>http://swagger.io/specification/#licenseObject</remarks>
    public class SwaggerLicense
    {
        /// <summary>
        /// Gets or Sets the license name used for the API. [Required]
        /// </summary>
        /// <value>
        /// License name used for the API.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL to the license used for the API. MUST be in the format of a URL.
        /// </summary>
        /// <value>
        /// URL to the license used for the API. MUST be in the format of a URL.
        /// </value>
        public string Url { get; set; }
    }
}