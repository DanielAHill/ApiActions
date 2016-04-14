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
    /// <remarks>http://swagger.io/specification/#contactObject</remarks>
    public class SwaggerContact
    {
        /// <summary>
        /// Gets or Sets the identifying name of the contact person/organization.
        /// </summary>
        /// <value>
        /// Identifying name of the contact person/organization.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL pointing to the contact information. MUST be in the format of a URL.
        /// </summary>
        /// <value>
        /// URL pointing to the contact information. MUST be in the format of a URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the email address of the contact person/organization. MUST be in the format of an email address.
        /// </summary>
        /// <value>
        /// Email address of the contact person/organization. MUST be in the format of an email address.
        /// </value>
        public string Email { get; set; }
    }
}