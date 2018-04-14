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

namespace ApiActions
{
    /// <summary>
    /// Identifies and provides an <see cref="IApiActionResponseFactory"/> that can create a response corresponding with the desired type of data.
    /// </summary>
    public interface IApiActionResponseAbstractFactory
    {
        /// <summary>
        /// Identifies and provides an <see cref="IApiActionResponseFactory" /> that can create a response corresponding with the desired type of data.
        /// </summary>
        /// <param name="scopedType">Data type that should be considered when constructing a response.</param>
        /// <returns>
        /// An <see cref="IApiActionResponseFactory" />
        /// </returns>
        /// /// <remarks><param name="scopedType">Scoped Type</param> may not always be the type returned by <c>data.GetType()</c>. This allows data to be rendered as inherited types such as base classes and interfaces.</remarks>
        IApiActionResponseFactory Create(Type scopedType);
    }
}