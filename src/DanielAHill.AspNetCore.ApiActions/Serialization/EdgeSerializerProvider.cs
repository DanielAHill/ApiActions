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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace DanielAHill.AspNetCore.ApiActions.Serialization
{
    public class EdgeSerializerProvider : IEdgeSerializerProvider
    {
        private readonly IOptions<ApiActionConfiguration> _configurationAccessor;
        private readonly IEdgeSerializer[] _serializers;

        public EdgeSerializerProvider(IEnumerable<IEdgeSerializer> serializers, IOptions<ApiActionConfiguration> configurationAccessor)
        {
            if (serializers == null) throw new ArgumentNullException(nameof(serializers));
            if (configurationAccessor == null) throw new ArgumentNullException(nameof(configurationAccessor));
            _configurationAccessor = configurationAccessor;
            _serializers = serializers.ToArray();
        }

        public IEdgeSerializer Get(HttpContext context)
        {
            return Get(context.Request.GetTypedHeaders().Accept ?? new MediaTypeHeaderValue[0]);
        }

        private IEdgeSerializer Get(IEnumerable<MediaTypeHeaderValue> acceptHeader)
        {
#if DEBUG
            if (acceptHeader == null) throw new ArgumentNullException(nameof(acceptHeader));
#endif
            MediaTypeHeaderValue defaultAccepts;

            foreach (var accepts in acceptHeader)
            {
                if (accepts.MatchesAllTypes && MediaTypeHeaderValue.TryParse(_configurationAccessor.Value.AcceptAllHeaderDefaultMediaType, out defaultAccepts) && !defaultAccepts.MatchesAllTypes)
                {   // When accepts all, use configuration default
                    return Get(new[] {defaultAccepts});
                }

                // ReSharper disable once ForCanBeConvertedToForeach (Array Iteration is faster)
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var s = 0; s < _serializers.Length; s++)
                {
                    if (_serializers[s].Handles(accepts))
                    {
                        return _serializers[s];
                    }
                }
            }

            // ReSharper disable once TailRecursiveCall
            return MediaTypeHeaderValue.TryParse(_configurationAccessor.Value.AcceptsHeaderNotMatchedDefaultMediaType, out defaultAccepts) ? Get(new[] { defaultAccepts }) : null;
        }
    }
}