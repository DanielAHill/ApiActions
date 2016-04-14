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
using System.Collections.Generic;
using System.Linq;

namespace DanielAHill.AspNet.ApiActions.Responses.Construction
{
    public class ApiActionResponseAbstractFactory : IApiActionResponseAbstractFactory
    {
        private readonly IApiActionResponseFactory[] _responseFactories;

        public ApiActionResponseAbstractFactory(IEnumerable<IApiActionResponseFactory> responseFactories)
        {
            // Append the default response factory at the end of the response factories registered into dependency injection. This allows for 
            // DI injection of more pointed factories, but still provides default behavior for types no factories have been registered.
            _responseFactories = (responseFactories ?? new IApiActionResponseFactory[0]).Concat(new [] { new DefaultResponseFactory() }).ToArray();
        }

        /// <summary>
        /// Identifies and provides an <see cref="T:DanielAHill.AspNet.ApiActions.IApiActionResponseFactory" /> that can create a response corresponding with the desired type of data.
        /// </summary>
        /// <param name="scopedType">Data type that should be considered when constructing a response.</param>
        /// <returns>
        /// An <see cref="T:DanielAHill.AspNet.ApiActions.IApiActionResponseFactory" />
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <remarks>
        ///   <param name="scopedType">Scoped Type</param> may not always be the type returned by <c>data.GetType()</c>. This allows data to be rendered as inherited types such as base classes and interfaces.
        /// </remarks>
        public IApiActionResponseFactory Create(Type scopedType)
        {
            if (scopedType == null) throw new ArgumentNullException(nameof(scopedType));
            // ReSharper disable once ForCanBeConvertedToForeach - For loop is faster
            // ReSharper disable once LoopCanBeConvertedToQuery - For loop is faster
            for (var x = 0; x < _responseFactories.Length; x++)
            {
                if (_responseFactories[x].Handles(scopedType))
                {
                    return _responseFactories[x];
                }
            }

            return null;
        }
    }
}