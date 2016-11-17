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

namespace DanielAHill.AspNetCore.ApiActions
{
    /// <summary>
    /// Creates an <see cref="ApiActionResponse"/> responsible for rendering data to the client, based on the type of data desired to render.
    /// </summary>
    public interface IApiActionResponseFactory
    {
        /// <summary>
        /// Determines if the instance can construct an <see cref="ApiActionResponse"/> responsible for rendering data to the client.
        /// </summary>
        /// <param name="scopedType">Data type that should be considered when constructing a response.</param>
        /// <returns>True if factory can create an <see cref="ApiActionResponse"/>, False otherwise.</returns>
        bool Handles(Type scopedType);

        /// <summary>
        /// Creates an <see cref="ApiActionResponse"/> for the provided data and scoped type.
        /// </summary>
        /// <param name="data">The data to be included in the response</param>
        /// <param name="scopedType">Data type that should be considered when constructing a response.</param>
        /// <returns><see cref="ApiActionResponse"/> responsible for rendering data to the client</returns>
        /// <remarks><param name="scopedType">Scoped Type</param> may not always be the type returned by <c>data.GetType()</c>. This allows data to be rendered as inherited types such as base classes and interfaces.</remarks>
        ApiActionResponse Create(object data, Type scopedType);

        /// <summary>
        /// Creates an <see cref="ApiActionResponse"/> for the provided status code, data and scoped type.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="data">The data to be included in the response.</param>
        /// <param name="scopedType">Data type that should be considered when constructing a response.</param>
        /// <returns><see cref="ApiActionResponse"/> responsible for rendering data to the client.</returns>
        /// <remarks><param name="scopedType">Scoped Type</param> may not always be the type returned by <c>data.GetType()</c>. This allows data to be rendered as inherited types such as base classes and interfaces.</remarks>
        ApiActionResponse Create(int statusCode, object data, Type scopedType);
    }
}