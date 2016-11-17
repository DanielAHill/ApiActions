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
using System.Reflection;
using DanielAHill.AspNetCore.ApiActions.Versioning;

namespace DanielAHill.AspNetCore.ApiActions.Swagger.Creation
{
    public class SwaggerApiActionRegistrationProvider : ISwaggerApiActionRegistrationProvider
    {
        private readonly IApiActionRegistrationProvider _registrationProvider;
        private readonly IVersionEdgeProvider _versionEdgeProvider;
        private IReadOnlyCollection<Tuple<ApiActionVersion, ApiActionVersion, IApiActionRegistration>> _registrationPreCalcs;

        public SwaggerApiActionRegistrationProvider(IApiActionRegistrationProvider registrationProvider, IVersionEdgeProvider versionEdgeProvider)
        {
            _registrationProvider = registrationProvider;
            _versionEdgeProvider = versionEdgeProvider;
            if (registrationProvider == null) throw new ArgumentNullException(nameof(registrationProvider));
            if (versionEdgeProvider == null) throw new ArgumentNullException(nameof(versionEdgeProvider));
        }

        public IReadOnlyCollection<IApiActionRegistration> Get(ApiActionVersion version)
        {
            if (_registrationPreCalcs == null)
            {
                _registrationPreCalcs = GeneratePreCalcs(_registrationProvider, _versionEdgeProvider);
            }

            return _registrationPreCalcs.Where(i => (i.Item1 == null || i.Item1 <= version) && (i.Item2 == null || i.Item2 >= version)).Select(i => i.Item3).ToList();
        }

        protected virtual IReadOnlyCollection<Tuple<ApiActionVersion, ApiActionVersion, IApiActionRegistration>> GeneratePreCalcs(IApiActionRegistrationProvider registrationProvider, IVersionEdgeProvider versionEdgeProvider)
        {
            var preCalc = new List<Tuple<ApiActionVersion, ApiActionVersion, IApiActionRegistration>>();
            var registrations = registrationProvider.Registrations;
            var walkingArray = new Type[1];
            
            foreach (var reg in registrations)
            {
                if (reg.ApiActionType.GetTypeInfo().GetCustomAttributes(true).OfType<NoDocAttribute>().Any(a => a.HideFromDocumentation))
                {
                    continue;
                }

                walkingArray[0] = reg.ApiActionType;
                var edges = versionEdgeProvider.GetVersionEdges(walkingArray);

                if (edges == null || edges.Length == 0)
                {
                    preCalc.Add(new Tuple<ApiActionVersion, ApiActionVersion, IApiActionRegistration>(null, null, reg));
                }
                else if (edges.Length == 1)
                {
                    preCalc.Add(new Tuple<ApiActionVersion, ApiActionVersion, IApiActionRegistration>(edges[0], edges[0], reg));
                }
                else if (edges.Length == 2)
                {
                    var minEdge = edges[0];
                    var maxEdge = edges[1];

                    if (minEdge > maxEdge)
                    {
                        minEdge = edges[1];
                        maxEdge = edges[0];
                    }

                    preCalc.Add(new Tuple<ApiActionVersion, ApiActionVersion, IApiActionRegistration>(minEdge, maxEdge, reg));
                }
                else
                {
                    // TODO?: Log warning
                }
            }

            return preCalc.ToArray();
        }
    }
}