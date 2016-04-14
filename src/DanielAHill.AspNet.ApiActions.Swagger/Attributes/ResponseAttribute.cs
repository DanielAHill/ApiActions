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

// ReSharper disable once CheckNamespace - Attributes should be in the namespace they target
namespace DanielAHill.AspNet.ApiActions
{
    /// <summary>
    /// Describes a potential response to executing ApiAction with the goal of providing helpful information to consuming clients.
    /// </summary>
    /// <remarks>Information provided may be used by code-gen clients to generate code</remarks>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ResponseAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the Http Status Code.
        /// </summary>
        /// <value>The HTTP Status Code.</value>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the human readible description.
        /// </summary>
        /// <value>The human readible description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Type of object returned as data in the body.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseAttribute"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP Status Code.</param>
        public ResponseAttribute(int statusCode)
        {
            StatusCode = statusCode;
        }
    }
}