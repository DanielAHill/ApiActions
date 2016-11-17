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
using System.Collections.Concurrent;
using System.Reflection;

namespace DanielAHill.AspNetCore.ApiActions.Introspection
{
    internal class ApiActionInfoProvider : IApiActionInfoProvider
    {
        private readonly IApiActionSummaryFactory _summaryFactory;
        private readonly IApiActionDescriptionFactory _descriptionFactory;
        private readonly IApiActionRequestMethodsFactory _requestMethodsFactory;
        private readonly IApiActionRequestTypeFactory _requestTypeFactory;
        private readonly IApiActionCategoryFactory _tagFactory;
        private readonly IApiActionResponseInfoFactory _responseInfoFactory;
        private readonly IApiActionDeprecationFactory _deprecationFactory;
        private readonly ConcurrentDictionary<Type, IApiActionInfo> _actionInfoCache = new ConcurrentDictionary<Type, IApiActionInfo>();

        public ApiActionInfoProvider(IApiActionSummaryFactory summaryFactory, IApiActionDescriptionFactory descriptionFactory, 
            IApiActionRequestMethodsFactory requestMethodsFactory, IApiActionRequestTypeFactory requestTypeFactory, 
            IApiActionCategoryFactory tagFactory, IApiActionResponseInfoFactory responseInfoFactory,
            IApiActionDeprecationFactory deprecationFactory)
        {
            if (summaryFactory == null) throw new ArgumentNullException(nameof(summaryFactory));
            if (descriptionFactory == null) throw new ArgumentNullException(nameof(descriptionFactory));
            if (requestMethodsFactory == null) throw new ArgumentNullException(nameof(requestMethodsFactory));
            if (requestTypeFactory == null) throw new ArgumentNullException(nameof(requestTypeFactory));
            if (tagFactory == null) throw new ArgumentNullException(nameof(tagFactory));
            if (responseInfoFactory == null) throw new ArgumentNullException(nameof(responseInfoFactory));
            if (deprecationFactory == null) throw new ArgumentNullException(nameof(deprecationFactory));
            _summaryFactory = summaryFactory;
            _descriptionFactory = descriptionFactory;
            _requestMethodsFactory = requestMethodsFactory;
            _requestTypeFactory = requestTypeFactory;
            _tagFactory = tagFactory;
            _responseInfoFactory = responseInfoFactory;
            _deprecationFactory = deprecationFactory;
        }

        public IApiActionInfo GetInfo(Type apiActionType)
        {
            if (apiActionType == null) throw new ArgumentNullException(nameof(apiActionType));
            if (!typeof(IApiAction).GetTypeInfo().IsAssignableFrom(apiActionType.GetTypeInfo())) throw new ArgumentException("Must implement type IApiAction", nameof(apiActionType));

            return _actionInfoCache.GetOrAdd(apiActionType, CreateInfo);
        }

        private IApiActionInfo CreateInfo(Type apiActionType)
        {
            return new ApiActionInfo
            {
                Summary = _summaryFactory.CreateSummary(apiActionType),
                Description = _descriptionFactory.CreateDescription(apiActionType),
                Methods = _requestMethodsFactory.CreateRequestMethods(apiActionType),
                RequestType = _requestTypeFactory.CreateRequestType(apiActionType),
                Responses = _responseInfoFactory.CreateResponses(apiActionType),
                Categories = _tagFactory.CreateCategories(apiActionType),
                IsDeprecated = _deprecationFactory.CreateIsDeprecated(apiActionType)
            };
        }
    }
}