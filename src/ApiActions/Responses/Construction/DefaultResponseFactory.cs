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
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ApiActions.Responses.Construction
{
    /// <summary>
    /// Provides all built-in responses based on object type. Register custom <see cref="IApiActionResponseFactory"/> to add or override behavior.
    /// </summary>
    /// <seealso cref="IApiActionResponseFactory" />
    public class DefaultResponseFactory : IApiActionResponseFactory
    {
        /// <summary>
        /// Determines if the instance can construct an <see cref="T:ApiActions.ApiActionResponse" /> responsible for rendering data to the client.
        /// </summary>
        /// <param name="scopedType">Data type that should be considered when constructing a response.</param>
        /// <returns>
        /// True if factory can create an <see cref="T:ApiActions.ApiActionResponse" />, False otherwise.
        /// </returns>
        public bool Handles(Type scopedType)
        {
            if (scopedType == null) throw new ArgumentNullException(nameof(scopedType));
            return GetDelegates(scopedType) != null;
        }

        /// <summary>
        /// Creates an <see cref="T:ApiActions.ApiActionResponse" /> for the provided data and scoped type.
        /// </summary>
        /// <param name="data">The data to be included in the response</param>
        /// <param name="scopedType">Data type that should be considered when constructing a response.</param>
        /// <returns>
        ///   <see cref="T:ApiActions.ApiActionResponse" /> responsible for rendering data to the client
        /// </returns>
        /// <remarks>
        ///   <param name="scopedType">Scoped Type</param> may not always be the type returned by <c>data.GetType()</c>. This allows data to be rendered as inherited types such as base classes and interfaces.
        /// </remarks>
        public ApiActionResponse Create(object data, Type scopedType)
        {
            if (scopedType == null) throw new ArgumentNullException(nameof(scopedType));
            return data == null ? NoContentResponse.Singleton : GetDelegates(scopedType)?.NoStatusCodeDelegate(data);
        }

        /// <summary>
        /// Creates an <see cref="T:ApiActions.ApiActionResponse" /> for the provided status code, data and scoped type.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="data">The data to be included in the response.</param>
        /// <param name="scopedType">Data type that should be considered when constructing a response.</param>
        /// <returns>
        ///   <see cref="T:ApiActions.ApiActionResponse" /> responsible for rendering data to the client.
        /// </returns>
        /// <remarks>
        ///   <param name="scopedType">Scoped Type</param> may not always be the type returned by <c>data.GetType()</c>. This allows data to be rendered as inherited types such as base classes and interfaces.
        /// </remarks>
        public ApiActionResponse Create(int statusCode, object data, Type scopedType)
        {
            if (scopedType == null) throw new ArgumentNullException(nameof(scopedType));
            return data == null
                ? new StatusCodeResponse(statusCode)
                : GetDelegates(scopedType)?.StatusCodeDelegate(statusCode, data);
        }

        private static readonly ApiActionResponseDelegates StringResponseDelegates;
        private static readonly ApiActionResponseDelegates InvalidModelResponseDelegates;
        private static readonly ApiActionResponseDelegates StreamResponseDelegates;
        private static readonly ApiActionResponseDelegates ExceptionResponseDelegates;
        private static readonly ApiActionResponseDelegates ObjectResponseDelegates;

        static DefaultResponseFactory()
        {
            StringResponseDelegates = new ApiActionResponseDelegates
            {
                NoStatusCodeDelegate = o => new StringResponse(200, o?.ToString()),
                StatusCodeDelegate = (c, o) => new StringResponse(c, o?.ToString())
            };

            InvalidModelResponseDelegates = new ApiActionResponseDelegates
            {
                NoStatusCodeDelegate = o => new InvalidModelResponse((IEnumerable<ValidationResult>) o),
                StatusCodeDelegate = (c, o) => new InvalidModelResponse(c, (IEnumerable<ValidationResult>) o)
            };

            StreamResponseDelegates = new ApiActionResponseDelegates
            {
                NoStatusCodeDelegate = o => new StreamResponse(200, (Stream) o, null),
                StatusCodeDelegate = (c, o) => new StreamResponse(c, (Stream) o, null)
            };

            ExceptionResponseDelegates = new ApiActionResponseDelegates
            {
                NoStatusCodeDelegate = o => new ObjectResponse(500, new ExecutionException((Exception) o)),
                StatusCodeDelegate = (c, o) => new ObjectResponse(c, new ExecutionException((Exception) o))
            };

            ObjectResponseDelegates = new ApiActionResponseDelegates
            {
                NoStatusCodeDelegate = o => new ObjectResponse(o),
                StatusCodeDelegate = (c, o) => new ObjectResponse(c, o)
            };
        }

        private static ApiActionResponseDelegates GetDelegates(Type scopedType)
        {
            // TODO: Reduce if statements

            if (scopedType == typeof(string))
            {
                return StringResponseDelegates;
            }

            if (typeof(IEnumerable<ValidationResult>).IsAssignableFrom(scopedType))
            {
                return InvalidModelResponseDelegates;
            }

            if (typeof(Stream).IsAssignableFrom(scopedType))
            {
                return StreamResponseDelegates;
            }

            if (typeof(Exception).IsAssignableFrom(scopedType))
            {
                return ExceptionResponseDelegates;
            }

            return typeof(object).IsAssignableFrom(scopedType) ? ObjectResponseDelegates : null;
        }
    }
}