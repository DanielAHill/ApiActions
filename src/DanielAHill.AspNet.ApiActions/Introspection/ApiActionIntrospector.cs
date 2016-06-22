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
using System.Reflection;
using DanielAHill.AspNet.ApiActions.Versioning;

namespace DanielAHill.AspNet.ApiActions.Introspection
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ApiActionIntrospector : IApiActionSummaryFactory, IApiActionDescriptionFactory, IApiActionResponseInfoFactory, IApiActionRequestMethodsFactory, IApiActionRequestTypeFactory, IApiActionCategoryFactory, IApiActionDeprecationFactory
    {
        public string CreateSummary(Type apiActionType)
        {
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));

            return GetAttributes<IHasSummary>(apiActionType).Select(a => a.Summary).FirstOrDefault(v => v != null);
        }

        public virtual string CreateDescription(Type apiActionType)
        {
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));
#if DNX451
            var componentDescription = GetAttribute<System.ComponentModel.DescriptionAttribute>(apiActionType)?.Description;

            // ReSharper disable once ConvertIfStatementToReturnStatement - Recommendation is accross compiler directive
            if (componentDescription != null)
            {
                return componentDescription;
            }
#endif

            return GetAttributes<IHasDescription>(apiActionType).Select(a => a.Description).FirstOrDefault(v => v != null);
        }

        public virtual IApiActionResponseInfo[] CreateResponses(Type apiActionType)
        {
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));
            
            var classLevelResponseTypes = GetAttributes<IHasApiActionResponseInfo>(apiActionType).SelectMany(a => a.Responses);
            // ReSharper disable once SuspiciousTypeConversion.Global
            var methodLevelResponseTypes = apiActionType.GetMethods().SelectMany(mi => mi.GetCustomAttributes()).OfType<IHasApiActionResponseInfo>().SelectMany(a => a.Responses);

            return classLevelResponseTypes.Concat(methodLevelResponseTypes).OrderBy(r => r.StatusCode).ToArray();
        }

        public virtual string[] CreateRequestMethods(Type apiActionType)
        {
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));
            return GetAttributes<IHasRequestMethods>(apiActionType).SelectMany(a => a.RequestMethods).Select(m => m.ToUpperInvariant()).Distinct().OrderBy(m => m).ToArray();
        }

        public virtual Type CreateRequestType(Type apiActionType)
        {
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));

            var walkingType = apiActionType;

            while (walkingType != null)
            {
                if (walkingType.GenericTypeArguments != null
                    && walkingType.GenericTypeArguments.Length == 1
                    && walkingType.GetGenericTypeDefinition() == typeof (ApiAction<>))
                {
                    return walkingType.GenericTypeArguments[0];
                }

                walkingType = walkingType.GetTypeInfo().BaseType;
            }

            return null;
        }

        public virtual bool CreateIsDeprecated(Type apiActionType)
        {
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));
            if (GetAttributes<ObsoleteAttribute>(apiActionType).Any())
            {
                // Explicitly marked as obsolete
                return true;
            }

            // If ending version is specified, then deprecation has been detected
            return (GetAttributes<IVersionEdgeFactory>(apiActionType).SelectMany(f => f.GetVersionEdges()).Distinct().Count() > 1);
        }

        public virtual string[] CreateCategories(Type apiActionType)
        {
            return GetAttributes<IHasCategories>(apiActionType).SelectMany(a => a.Categories).Distinct().OrderBy(t => t).ToArray();
        }

        private static T GetAttribute<T>(Type apiActionType)
        {
            return GetAttributes<T>(apiActionType).FirstOrDefault();
        }

        private static IEnumerable<T> GetAttributes<T>(Type apiActionType)
        {
            return apiActionType.GetTypeInfo().GetCustomAttributes(true).OfType<T>();
        }
    }
}