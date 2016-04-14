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

namespace DanielAHill.AspNet.ApiActions.Introspection
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ApiActionIntrospector : IApiActionSummaryFactory, IApiActionDescriptionFactory, ApiActionResponseInfoFactory, IApiActionRequestMethodsFactory, IApiActionRequestTypeFactory, IApiActionTagFactory
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

        public virtual ApiActionResponseInfo[] CreateResponses(Type apiActionType)
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
            return GetAttributes<IHasRequestType>(apiActionType).Select(a => a.RequestType).FirstOrDefault(v => v != null);
        }

        public virtual string[] CreateTags(Type apiActionType)
        {
            return GetAttributes<IHasTags>(apiActionType).SelectMany(a => a.Tags).Distinct().OrderBy(t => t).ToArray();
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