// Copyright (c) 2018 Daniel A Hill. All rights reserved.
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
using System.Collections.Generic;
using ApiActions.Introspection;

// ReSharper disable once CheckNamespace - Attributes should be in the namespace they target
namespace ApiActions
{
    /// <summary>
    /// Describes a potential response to executing ApiAction with the goal of providing helpful information to consuming clients.
    /// </summary>
    /// <remarks>Information provided may be used by code-gen clients to generate code</remarks>
    /// <seealso cref="T:System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseAttribute : Attribute, IHasApiActionResponseInfo
    {
        public IReadOnlyCollection<IApiActionResponseInfo> Responses => new[]
        {
            new ApiActionResponseInfo
            {
                StatusCode = StatusCode,
                Description = Description,
                ResponseData = Type
            }
        };

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

        public ResponseAttribute(int statusCode) : this(statusCode, null, null)
        {
        }

        public ResponseAttribute(int statusCode, Type type) : this(statusCode, type, null)
        {
        }

        public ResponseAttribute(int statusCode, string description) : this(statusCode, null, description)
        {
        }

        public ResponseAttribute(int statusCode, Type type, string description)
        {
            StatusCode = statusCode;
            Type = type;
            Description = description;
        }
    }
}