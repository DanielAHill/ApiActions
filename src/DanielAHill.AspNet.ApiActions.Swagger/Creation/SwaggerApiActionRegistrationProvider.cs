using System;
using System.Collections.Generic;
using System.Linq;
using DanielAHill.AspNet.ApiActions.Versioning;

namespace DanielAHill.AspNet.ApiActions.Swagger.Creation
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