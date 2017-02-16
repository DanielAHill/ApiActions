﻿#region Copyright
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
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace DanielAHill.AspNetCore.ApiActions.Serialization
{
    public class JsonEdgeSerializer : IEdgeSerializer
    {
        private const string ContentType = "application/json";

        public string[] ContentTypes { get; } = { ContentType };

        public bool Handles(MediaTypeHeaderValue mediaType)
        {
            return "application".Equals(mediaType.Type, StringComparison.OrdinalIgnoreCase)
                   && ("json".Equals(mediaType.SubType, StringComparison.OrdinalIgnoreCase)
                        || mediaType.MatchesAllSubTypes);
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="response">The response.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public async Task SerializeAsync(object value, HttpResponse response, CancellationToken cancellationToken)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (response == null) throw new ArgumentNullException(nameof(response));

            // Set Content Type
            response.ContentType = ContentType;
            var serializedValue = JsonConvert.SerializeObject(value);
            response.ContentLength = serializedValue.Length;

            await response.WriteAsync(serializedValue, Encoding.UTF8, cancellationToken);
        }
    }
}