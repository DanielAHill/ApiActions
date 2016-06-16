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
using Microsoft.AspNet.Http;
using Microsoft.Extensions.OptionsModel;

namespace DanielAHill.AspNet.ApiActions.Versioning
{
    public class RequestVersionProvider : IRequestVersionProvider
    {
        private const string RequestCacheKey = "_RequestApiActionVersion";
        
        private readonly IRequestVersionParser[] _versionParsers;
        private readonly ApiActionVersion _defaultVersion;

        public RequestVersionProvider(IEnumerable<IRequestVersionParser> versionParsers, IOptions<ApiActionConfiguration> configurationAccessor, IVersionEdgeProvider versionEdgeProvider, IApiActionRegistrationProvider apiActionRegistrationProvider)
        {
            if (versionParsers == null) throw new ArgumentNullException(nameof(versionParsers));
            if (configurationAccessor == null) throw new ArgumentNullException(nameof(configurationAccessor));
            if (versionEdgeProvider == null) throw new ArgumentNullException(nameof(versionEdgeProvider));
            if (apiActionRegistrationProvider == null) throw new ArgumentNullException(nameof(apiActionRegistrationProvider));

            _versionParsers = versionParsers.ToArray();
            if (!ApiActionVersion.TryParse(configurationAccessor.Value.DefaultRequestVersion, out _defaultVersion))
            {
                _defaultVersion = versionEdgeProvider.GetVersionEdges(apiActionRegistrationProvider.Registrations.Select(r => r.ApiActionType).ToList()).OrderByDescending(v => v).FirstOrDefault()
                                    ?? new ApiActionVersion(1, 0);
            }
        }

        public ApiActionVersion Get(HttpContext context, IDictionary<string, object> routeValues)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (routeValues == null) throw new ArgumentNullException(nameof(routeValues));

            object requestCachedVersion;
            context.Items.TryGetValue(RequestCacheKey, out requestCachedVersion);

            var version = requestCachedVersion as ApiActionVersion;

            if (version == null)
            {
                foreach (var versionParser in _versionParsers)
                {
                    var versionString = versionParser.Parse(context, routeValues);

                    if (versionString != null && ApiActionVersion.TryParse(versionString, out version))
                    {
                       break;
                    }
                }

                if (version == null)
                {
                    // If version not found, use configuration provided value, then default
                    version = _defaultVersion;
                }

                context.Items[RequestCacheKey] = version;
            }

            return version;
        }
    }
}